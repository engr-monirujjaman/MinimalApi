using WebApi.Data;
using WebApi.Requests;

namespace WebApi.Handlers;

public class CreateCustomerHandler : IHttpRequestHandler<CreateCustomerRequest>
{
    private readonly AppDbContext _dbContext;

    public CreateCustomerHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IResult> Handle(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
       await _dbContext.Customers.AddAsync(request.Prepare(), cancellationToken);
       await _dbContext.SaveChangesAsync(cancellationToken);
       return Results.Success();
    }
}