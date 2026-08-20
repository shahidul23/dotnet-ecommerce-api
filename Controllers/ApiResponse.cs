using System;

namespace dotnet_ecommerce_api.Controllers;

public class ApiResponse<T>
{
    public bool Success { set; get;}
    public string Message{set;get;} = string.Empty;
    public T? Data { get; set;}
    public List<string>? Errors {set; get;}
    public int StatusCode {set; get;}
    public DateTime TimeStamp {get; set;}

    // Constactor for response
    private ApiResponse(bool success, string message, T? data ,List<string>? errors, int statusCode)
    {
        Success = success;
        Message = message;
        Data = data;
        Errors = errors;
        StatusCode = statusCode;
        TimeStamp = DateTime.UtcNow;
    }

    // for creating successfull message
    public static ApiResponse<T> SuccessResponse (T? data, int statusCode, string message = "")
    {
        return new ApiResponse<T> (true, message, data, null, statusCode);
    }
    public static ApiResponse<T> ErrorResponse (List<string> errors, int statusCode, string message = "")
    {
        return new ApiResponse<T> (false, message, default(T), errors, statusCode);
    }

}
