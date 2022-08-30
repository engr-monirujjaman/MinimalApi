namespace WebApi.Requests;

public class SecretRequest : IHttpRequest
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }
}