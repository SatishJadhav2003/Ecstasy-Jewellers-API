using ECSTASYJEWELS.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECSTASYJEWELS.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly UserRepository _repository;
        private readonly TokenService _tokenService;

        public UserController(UserRepository repository, TokenService tokenService)
        {
            _repository = repository;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUser loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Invalid login data.");
            }

            var user = await _repository.ValidateUserCredentials(loginDto.Email, loginDto.Password);

            if (user == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            // Generate JWT token
            var token = _tokenService.GenerateToken(user);

            return Ok(new
            {
                Token = token,
                User = new
                {
                    user.User_ID,
                    user.First_Name,
                    user.Last_Name,
                    user.Email,
                    user.Phone_Number
                }
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserInfo registerUser,string password)
        {

            var user = await _repository.Register(registerUser,password);

            if (user == null)
            {
                return BadRequest("Not Registered");
            }
            return Ok(user);
        }

        [HttpGet("{User_ID}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUserInfo(decimal User_ID)
        {
            try
            {
                var response = await _repository.GetUserInfo(User_ID);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Internal server error
            }
        }


    }

}