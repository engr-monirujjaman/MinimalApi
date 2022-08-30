using Microsoft.AspNetCore.Identity;
using WebApi.Requests;

namespace WebApi.Handlers;

public class AccountRegisterHandler : IHttpRequestHandler<AccountRegisterRequest>
{
    private readonly UserManager<IdentityUser> _userManager;

    public AccountRegisterHandler(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IResult> Handle(AccountRegisterRequest request, CancellationToken cancellationToken)
    {
        var user = new IdentityUser(request.Email);
        var result = await _userManager.CreateAsync(user, request.Password);

        return result.Succeeded is false
            ? Results.Fail(StatusCodes.Status400BadRequest, result.Errors.Select(x => x.Description))
            : Results.Success();
    }
}