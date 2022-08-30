using WebApi.DomainModels;

namespace WebApi.Requests;

public record GetCustomerRequest(int Id) : IHttpRequest;

public record GetCustomersRequest : IHttpRequest;

public record DeleteCustomerRequest(int Id) : IHttpRequest;

public record CreateCustomerRequest : IHttpRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public int Age { get; set; }

    public Customer Prepare() => new()
    {
        FistName = FirstName,
        LastName = LastName,
        Age = Age,
        EmailAddress = Email
    };
}