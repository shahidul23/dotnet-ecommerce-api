using System;

namespace dotnet_ecommerce_api.Exceptions;

public class UnauthorizedException:AppException
{
    public UnauthorizedException(string message):base(message, StatusCodes.Status401Unauthorized)
    {
        
    }
}
