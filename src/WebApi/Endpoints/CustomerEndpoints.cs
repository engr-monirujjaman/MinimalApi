using WebApi.Extensions;
using WebApi.Requests;

namespace WebApi.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this WebApplication app)
    {
        app.MediateGet<GetCustomerRequest>("customers/{id}");
        app.MediateGet<GetCustomersRequest>("customers").RequireAuthorization();
        app.MediateDelete<DeleteCustomerRequest>("customers/{id}");
        app.MediatePost<CreateCustomerRequest>("customers");
    }
}