using Constants;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController(IOptions<JwtSettings> jwtSettings,
        IOptions<GoogleSettings> googleSettings, IWebHostEnvironment env) : ControllerBase
    {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;
        private readonly GoogleSettings _googleSettings = googleSettings.Value;
        private readonly int _tokenExpiryMinutes = 30;
        private readonly IWebHostEnvironment _env = env;

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            try
            {
                // Validate the token
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [_googleSettings.ClientId]
                });

                var user = new
                {
                    payload.Email,
                    payload.Name,
                    payload.Picture
                };

                // Generate JWT token
                var accessToken = GenerateJwtToken(payload.Email);

                // Set the token in an HTTP-only cookie
                SetHttpOnlyCookie(CookieNames.JwtToken, accessToken, _tokenExpiryMinutes);

                // Return user info without the token
                return Ok(new
                {
                    user
                });
            }
            catch (InvalidJwtException)
            {
                return Unauthorized(new { message = "Invalid token" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        private string GenerateJwtToken(string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_tokenExpiryMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = creds
            };
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }

        [Authorize]
        [HttpGet("validate")]
        public IActionResult ValidateAuth()
        {
            return Ok(new { isAuthenticated = true });
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Remove the JWT token by setting the cookie expiration to a past date
            SetHttpOnlyCookie(CookieNames.JwtToken, string.Empty, -1);

            return Ok(new { message = "Logged out successfully" });
        }
        private void SetHttpOnlyCookie(string key, string value, int expiryMinutes)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes)
            };

            // Only set the Domain attribute if the environment is not Development
            if (!_env.IsDevelopment())
            {
                cookieOptions.Domain = "h5ctyejqkwfoxn6d72fr24osxe0swjjx.lambda-url.eu-west-2.on.aws";
            }

            HttpContext.Response.Cookies.Append(key, value, cookieOptions);
        }
    }

    public class LoginRequest
    {
        public required string Token { get; set; }
    }
}