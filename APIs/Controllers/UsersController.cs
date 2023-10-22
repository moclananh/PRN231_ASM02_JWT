using APIs.Models.EF;
using APIs.Models.Entities;
using APIs.Models;
using APIs.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Linq;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly AppSetting _app;

        public UsersController(AppDbContext dbContext, IOptionsMonitor<AppSetting> optionsMonitor)
        {
            _dbContext = dbContext;
            _app = optionsMonitor.CurrentValue;
        }

        [HttpPost("Login")]
        public IActionResult Authentication(LoginVm request)
        {
            var user = _dbContext.Users.SingleOrDefault(x => x.Email == request.Email
            && x.Password == request.Password);

            if (user == null)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Email or password incorrect!"
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Login Success!",
                Data = GenerateToken(user)
            });
        }

        private string GenerateToken(User user)
        {
            var jwtTokenHandel = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_app.SecretKey);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("Id", user.Id.ToString()),
                    new Claim("TokenId", Guid.NewGuid().ToString())

                }),
                Expires = DateTime.UtcNow.AddMinutes(1), //set time token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandel.CreateToken(tokenDescription);
            return jwtTokenHandel.WriteToken(token);
        }


        [HttpPost("Register")]
        public IActionResult Register(RegisterVm request)
        {
            // Check if a user with the same email already exists in the database
            var existingUser = _dbContext.Users.SingleOrDefault(x => x.Email == request.Email);

            if (existingUser != null)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Email is already registered. Please use a different email."
                });
            }

            // Create a new user based on the registration request
            var newUser = new User
            {
                FirstName= request.FirstName,
                LastName= request.LastName,
                UserName= request.UserName,
                Email = request.Email,
                Password = request.Password,
                CreateDate= DateTime.UtcNow,
                                             
            };

            // Add the new user to the database
            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Registration Successful!",
              
            });
        }
    }
}
