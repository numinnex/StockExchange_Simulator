using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace Presentation;

[AttributeUsage( AttributeTargets.Method)]
public sealed class AuthFilter : Attribute, IAsyncAuthorizationFilter 
{
    private readonly TokenValidationParameters _tokenValidationParameters;

    public AuthFilter(TokenValidationParameters tokenValidationParameters)
    {
        _tokenValidationParameters = tokenValidationParameters;
    }
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {

        if (!UserIsAuthorized(context.HttpContext))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        await Task.CompletedTask;
    }

    private bool UserIsAuthorized(HttpContext contextHttpContext)
    {
        var authHeader = contextHttpContext.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader))
        {
            return false;
        }
        var token = authHeader.Split("Bearer ")[1];
        var principal = GetPrincipalFromToken(token);
        if (principal == null)
        {
            return false;
        }

        return true;
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
        catch(Exception e)
        {
            Console.WriteLine(e);
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
}