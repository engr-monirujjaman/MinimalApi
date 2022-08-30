using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using WebApi.Requests;
using WebApi.Settings;
using JwtConstants = WebApi.Settings.JwtConstants;

namespace WebApi.Handlers;

public class AccountLoginHandler : IHttpRequestHandler<AccountLoginRequest>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountLoginHandler(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    public async Task<IResult> Handle(AccountLoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null) return Results.Fail(StatusCodes.Status401Unauthorized);
        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        
        if(signInResult.Succeeded is false) return Results.Fail(StatusCodes.Status401Unauthorized);
        
        var claims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        
        claims.Add(new Claim(ClaimTypes.Sid, user.Id));
        claims.Add(new Claim(ClaimTypes.Email, user.Email!));
        
        if (roles.Any())
        {
            roles.ToList().ForEach(x =>
            {
                claims.Add(new Claim(ClaimTypes.Role, x));
            });
        }

        var tokenBuilder = new TokenBuilder()
            .AddAudience(JwtConstants.Audience)
            .AddIssuer(JwtConstants.Issuer)
            .AddKey(JwtConstants.Key)
            .AddClaims(claims.ToList())
            .AddExpiry(TimeSpan.FromDays(7))
            .Build();
        
        var token = new JwtSecurityTokenHandler().WriteToken(tokenBuilder);

        return Results.Success(StatusCodes.Status202Accepted, new
        {
            AccessToken = token,
            ExpiresIn = tokenBuilder.ValidTo
        });
    }
}