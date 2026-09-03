using dotnet_ecommerce_api.Common;
using dotnet_ecommerce_api.DTOs.Auth;
using dotnet_ecommerce_api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_ecommerce_api.Controllers.Auth
{
    [ApiController]
    [Route("api")]
    public class AuthenticationController : ControllerBase
    {
        private IAuthService _authService;
        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterDto payload
        )
        {
            Console.WriteLine("Start Register");
            var user = await _authService.RegisterAsync(payload);
            if (user == null)
            {
                return BadRequest(
                    ApiResponse<object>.ErrorResponse(
                        new List<string>
                        {
                            "User could not be created."
                        },
                        400,
                        "Registration failed."
                    )
                );
            }
            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<AuthResponseDto>.SuccessResponse(
                    user,
                    201,
                    "User created successfully."
                )
            );
        }


    }
}
