using System.Text.Json;
using FluentValidation.Results;

namespace WebApi.Common;

public static class Results
{
    public static IResult Fail() =>
        new Result<object?>(StatusCodes.Status400BadRequest, default, Enumerable.Empty<string>());
    
    public static IResult Fail(IEnumerable<string>  errors) =>
        new Result<object?>(StatusCodes.Status400BadRequest, default, errors);
    
    public static IResult Fail(int statusCode, IEnumerable<string>  errors) =>
        new Result<object?>(statusCode, default, errors);
    
    public static IResult Fail(int statusCode) =>
        new Result<object?>(statusCode, default, Enumerable.Empty<string>());
    
    public static IResult Fail(int statusCode, object data, IEnumerable<string>  errors) =>
        new Result<object?>(statusCode, data, errors);
    
    public static IResult Fail(IEnumerable<ValidationFailure>  errors) =>
        new Result<object?>(StatusCodes.Status400BadRequest, default, errors.Select(x => x.ErrorMessage));
    
    public static IResult Success() =>
        new Result<object?>(StatusCodes.Status200OK, default, Enumerable.Empty<string>());
    
    public static IResult Success(int statusCode) =>
        new Result<object?>(statusCode, default, Enumerable.Empty<string>());    
    
    public static IResult Success(object data) =>
        new Result<object?>(StatusCodes.Status200OK, data, Enumerable.Empty<string>());
    
    public static IResult Success(int statusCode, object data) =>
        new Result<object?>(statusCode, data, Enumerable.Empty<string>());
}

internal class Result<T> : IResult<T>
{
    public Result(int statusCode, T data, IEnumerable<string> errors)
    {
        StatusCode = statusCode;
        Data = data;
        Errors = errors;
        IsSuccess = StatusCode is >= 200 and <= 299;
    }

    public bool IsSuccess { get; }
    public int StatusCode { get; set; }
    public IEnumerable<string> Errors { get; }

    public T? Data { get; }

    public override string ToString() => JsonSerializer.Serialize(this);
}

internal interface IResult<out T> : IResult
{
    T? Data { get; }
}

public interface IResult
{
    bool IsSuccess { get; }

    int StatusCode { get; }

    IEnumerable<string> Errors { get; }
}