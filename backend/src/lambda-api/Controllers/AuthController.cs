using Constants;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController(IOptions<JwtSettings> jwtSettings,
        IOptions<GoogleSettings> googleSettings, 
        UserManager<IdentityUser<Guid>> userManager) : ControllerBase
    {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;
        private readonly GoogleSettings _googleSettings = googleSettings.Value;
        private readonly int _tokenExpiryMinutes = 30;
        private readonly UserManager<IdentityUser<Guid>> _userManager = userManager;

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

                // Check if user exists, create if not
                var user = await GetOrCreateUserAsync(payload.Email, payload.Name);
                if (user == null)
                {
                    return StatusCode(500, new { message = "Failed to create or retrieve user" });
                }

                // Generate JWT token
                var accessToken = GenerateJwtToken(user.Id.ToString());

                // Set the token in an HTTP-only cookie
                SetHttpOnlyCookie(CookieNames.JwtToken, accessToken, _tokenExpiryMinutes);

                return Ok(new {});
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

        private async Task<IdentityUser<Guid>?> GetOrCreateUserAsync(string email, string name)
        {
            // Try to find existing user by email
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return existingUser;
            }

            // Create new user if not found
            var newUser = new IdentityUser<Guid>
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                EmailConfirmed = true, // Since it's from Google OAuth, we trust the email
            };

            var result = await _userManager.CreateAsync(newUser);
            if (result.Succeeded)
            {
                return newUser;
            }

            // Log the errors for debugging
            foreach (var error in result.Errors)
            {
                // use ILogger here instead
                Console.WriteLine($"User creation error: {error.Description}");
            }

            return null;
        }

        private string GenerateJwtToken( string userId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(JwtRegisteredClaimNames.Sub, userId),
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

            HttpContext.Response.Cookies.Append(key, value, cookieOptions);
        }
    }
}