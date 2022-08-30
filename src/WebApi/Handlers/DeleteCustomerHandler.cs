using WebApi.Data;
using WebApi.Requests;

namespace WebApi.Handlers;

public class DeleteCustomerHandler : IHttpRequestHandler<DeleteCustomerRequest>
{
    private readonly AppDbContext _dbContext;

    public DeleteCustomerHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IResult> Handle(DeleteCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers.FindAsync(request.Id);

        if (customer is not null)
        {
            _dbContext.Customers.Remove(customer);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Results.Success();
        }

        return Results.Fail(new []{"Customer not found"});
    }
}