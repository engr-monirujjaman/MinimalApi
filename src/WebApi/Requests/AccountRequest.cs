namespace WebApi.Requests;

public record AccountLoginRequest(string Email, string Password) : IHttpRequest;
public record AccountRegisterRequest(string Email, string Password, string ConfirmPassword) : IHttpRequest;