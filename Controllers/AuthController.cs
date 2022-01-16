using JWTAuthProject.DataModels;
using JWTAuthProject.DomainModels;
using JWTAuthProject.Helpers;
using JWTAuthProject.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IJwtService jwtService;

        public AuthController(IUserRepository userRepository, IJwtService jwtService)
        {
            this.userRepository = userRepository;
            this.jwtService = jwtService;
        }


        [HttpPost("register")]
        public IActionResult Register([FromForm] Register request)
        {
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            return Created("success", userRepository.Create(user));

        }

        [HttpPost("Login")]
        public IActionResult Login([FromForm] Login request)
        {
            var user = userRepository.GetByEmail(request.Email);

            if (user == null) return BadRequest(new { message = "Invalid Credentials" });

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return BadRequest(new { message = "Invalid Credentials" });
            }

            var jwt = jwtService.Generate(user.Id);

            Response.Cookies.Append("jwt", jwt, new CookieOptions
            {
                HttpOnly = true
            });

            return Ok(new { message = "Success" });
        }

        [HttpGet("user")]

        public IActionResult User()
        {
            try
            {
                var jwt = Request.Cookies["jwt"];

                var token = jwtService.Verify(jwt);

                int userId = int.Parse(token.Issuer);

                var user = userRepository.GetById(userId);

                return Ok(user);
            }
            catch (Exception ) 
            {
                return Unauthorized();
            }
            
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");

            return Ok(new {message = "success"});
        }
    }

}
