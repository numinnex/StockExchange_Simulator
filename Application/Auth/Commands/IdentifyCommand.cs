using System.IdentityModel.Tokens.Jwt;
using Domain.Auth;
using MediatR;

namespace Application.Auth.Commands;

public sealed record IdentifyCommand(string Token ) : IRequest<IdentifyResult>;
public sealed class IdentifyCommandHandler: IRequestHandler<IdentifyCommand,IdentifyResult >
{
    public async Task<IdentifyResult> Handle(IdentifyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var jwtToken = new JwtSecurityToken(request.Token);
            var username = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value;
            return new IdentifyResult()
            {
                IsSuccess = username != null,
                UserName = username
            };
        }
        catch
        {
            return new IdentifyResult()
            {
                IsSuccess = false,
                UserName = ""
            };
        }

    }
}