using Microsoft.AspNetCore.Mvc;
using DevHouse1.Services;

namespace DevHouse1.Controllers
{
    /// <summary>
    /// Controller for handling authentication and generating JWT tokens.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authService">The authentication service for managing JWT tokens.</param>
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Generates a JWT token for authentication.
        /// </summary>
        /// <remarks>
        /// **Sample Request:**
        /// 
        ///     POST /api/Auth/token
        /// 
        /// **Example Response:**
        /// 
        /// ```json
        /// {
        ///   "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
        /// }
        /// ```
        /// </remarks>
        /// <response code="200">JWT token generated successfully</response>
        [HttpPost("token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GenerateToken()
        {
            var token = _authService.GenerateToken(); // 🔹 Token is generated in AuthService
            return Ok(new { Token = token });
        }
    }
}
