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
        public AuthenticationController(
            IAuthService authService
        )
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterDto dto
        )
        {
            var user = await _authService.RegisterAsync(dto);
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

        [HttpPost("login")]
        public async Task<IActionResult> UserLogin(
            [FromBody] LoginDto dto
        )
        {
            var user = await _authService.LoginAsync(dto);
            if (user == null)
            {
                return Unauthorized(
                    ApiResponse<object>.ErrorResponse(
                        new List<string>
                        {
                            "Invalid username/email or password."
                        },
                        401,
                        "Login failed."
                    )
                );
            }
            return Ok(
                ApiResponse<AuthResponseDto>.SuccessResponse(
                    user,
                    200,
                    "Login successful."
                )
            );
        }

        // Refresh tokan
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(
            [FromBody] TokenRequestDto dto
        )
        {
            var response = await _authService.RefreshTokenAsync(
                dto
            );
            return Ok(
                ApiResponse<AuthResponseDto>.SuccessResponse(
                    response,
                    200,
                    "Token refreshed successfully."
                )
            );
        }
    }
}
