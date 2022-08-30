using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Requests;

namespace WebApi.Handlers;

[Authorize]
public class GetCustomersHandler : IHttpRequestHandler<GetCustomersRequest>
{
    private readonly AppDbContext _dbContext;

    public GetCustomersHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IResult> Handle(GetCustomersRequest request, CancellationToken cancellationToken)
    {
        var customers = await _dbContext.Customers
            .AsNoTracking().ToListAsync(cancellationToken);

        return Results.Success(customers);
    }
}