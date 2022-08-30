using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Requests;

namespace WebApi.Handlers;

public class GetCustomerHandler : IHttpRequestHandler<GetCustomerRequest>
{
    private readonly AppDbContext _dbContext;

    public GetCustomerHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IResult> Handle(GetCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer =
            await _dbContext.Customers
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        
        return customer is null ? Results.Fail(new []{"Customer not found"}) : Results.Success(customer);
    }
}