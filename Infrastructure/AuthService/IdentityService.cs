using System.Text;
using Application.Common.Interfaces;
using Domain.Auth;
using Domain.Identity;
using Infrastructure.Database;
using Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Infrastructure.AuthService;

public sealed class IdentityService :  IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOptions<JwtSettingsOptions> _jwtSettings;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly ApplicationDbContext _ctx;
    private readonly RoleManager<IdentityRole> _roleManager;

    public IdentityService(UserManager<ApplicationUser> userManager , IOptions<JwtSettingsOptions> jwtSettings , TokenValidationParameters 
        tokenValidationParameters , ApplicationDbContext ctx, RoleManager<IdentityRole> roleManager )
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings;
        _tokenValidationParameters = tokenValidationParameters;
        _ctx = ctx;
        _roleManager = roleManager;
    }
    
    public async Task<AuthenticationResult> RegisterAsync(string requestEmail, string requestPassword)
    {
        var user = await _userManager.FindByEmailAsync(requestEmail);

        if (user is not null)
        {
            return new AuthenticationResult
            {
                Errors = new[] {"User with this email already exists"},
                Success = false,
            };
        }

        var newUser = new ApplicationUser()
        {
            Id = Guid.NewGuid().ToString(),
            Email = requestEmail,
            UserName = requestEmail
        };
        var createdUser = await _userManager.CreateAsync(newUser, requestPassword);

        if (!createdUser.Succeeded)
        {
            return new AuthenticationResult()
            {
                Errors = createdUser.Errors.Select(x => x.Description),
                Success = false
            };
        }
        await _userManager.AddToRoleAsync(newUser, "User");
        return await GenerateAuthenticationResultForUserAsync(newUser);

    }

    public async Task<AuthenticationResult> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return new AuthenticationResult()
            {
                Errors = new[] { "User does not exist" },
                Success = false
            };
        }
        var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);
        if (!userHasValidPassword)
        {
            return new AuthenticationResult()
            {
                Errors = new[] { "Wrong credentials" },
                Success = false
            };
        }

        return await GenerateAuthenticationResultForUserAsync(user);
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
    {
        var validatedToken = GetPrincipalFromToken(token);
        if (validatedToken is null)
        {
            return new AuthenticationResult
            {
                Errors = new[] { "Invalid token error" },
                Success = false
            };
            
        }

        var expiryDateUnix =
            long.Parse(validatedToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)!.Value);
        var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            .AddSeconds(expiryDateUnix)
            .Subtract(_jwtSettings.Value.TokenLifetime);

        if (expiryDateTimeUtc > DateTime.UtcNow)
        {
            return new AuthenticationResult
            {
                Success = false,
                Errors = new[] { "This token hasn't expired yet" }
            };
        }
        
        var jti = validatedToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)!.Value;
        var storedRefreshToken = await _ctx.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken);

        if (storedRefreshToken is null)
        {
            return new AuthenticationResult()
            {
                Errors = new[] { "Error token doesnt exist" },
                Success = false
            };
        }

        if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
        {
            return new AuthenticationResult()
            {
                Errors = new[] { "This refresh token has expired" },
                Success = false
            };
        }
        if (storedRefreshToken.Invalidated)
        {
            return new AuthenticationResult()
            {
                Errors = new[] { "This refresh token has been invalidated" },
                Success = false
            };
        }
        
        if (storedRefreshToken.Used)
        {
            return new AuthenticationResult()
            {
                Errors = new[] { "This refresh token has been used" },
                Success = false
            };
        }

        if (storedRefreshToken.JwtId != jti)
        {
            return new AuthenticationResult()
            {
                Errors = new[] { "This refresh token does not match this JWT" },
                Success = false
            };
        }

        storedRefreshToken.Used = true;
        _ctx.RefreshTokens.Update(storedRefreshToken);

        await _ctx.SaveChangesAsync();

        var user = await _userManager.FindByIdAsync(validatedToken.Claims.FirstOrDefault(x => x.Type == "id")!.Value);
        
        return await GenerateAuthenticationResultForUserAsync(user!);

    }

    public async Task<bool> UserExistsAsync(string userId, CancellationToken token)
    {
        var result = await _userManager.FindByIdAsync(userId);
        return result is not null;
    }

    private ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token , _tokenValidationParameters , out var validToken);
            if (!IsJwtWithValidSecurityAlgorithm(validToken))
            {
                return null; 
            }

            return principal;

        }
        catch
        {
            return null;
        }
    }
    
    private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
    {
        bool isJwtToken = (validatedToken is JwtSecurityToken );
        var jwtSecurityToken = validatedToken as JwtSecurityToken;
                      
        bool IsRightAlgo = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
            StringComparison.InvariantCultureIgnoreCase);
        
        return isJwtToken && IsRightAlgo;
    }

    private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(ApplicationUser user)
    {

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.Secret));
        var claims = new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
        };
        
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: null, 
            audience: null, 
            claims: claims, 
            notBefore: null,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: credentials);
        

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        var refreshToken = new RefreshToken()
        {
            JwtId = Guid.NewGuid().ToString(),
            UserId = user.Id,
            CreationDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddMonths(6)
        };

        await _ctx.RefreshTokens.AddAsync(refreshToken);
        await _ctx.SaveChangesAsync();

        return new AuthenticationResult()
        {
            Success = true,
            Token = tokenValue ,
            RefreshToken = refreshToken.Token,
            Errors = Enumerable.Empty<string>()
        }; 
        
    }
    
}
