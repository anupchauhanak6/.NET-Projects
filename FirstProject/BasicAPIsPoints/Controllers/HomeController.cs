using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BasicAPIsPoints.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        // In-memory user database for demonstration purposes
        private static readonly List<UserAccount> Users = new();

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            // 1. Validate required fields
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Username and password are required." });
            }

            string normalizedUsername = request.Username.Trim().ToLower();

            // 2. Check if username already exists (Case-insensitive check)
            if (Users.Any(u => u.Username == normalizedUsername))
            {
                return Conflict(new { message = "Username already exists." });
            }

            // 3. Securely hash password using BCrypt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            string generatedUserId;

            // Generate unique ID using a retry loop
            do
            {
                generatedUserId = "USR-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            } 
            while (Users.Any(u => u.UserId == generatedUserId)); // Loop till a unique ID is found

            var newUser = new UserAccount
            {
                UserId = generatedUserId,
                Username = normalizedUsername,
                PasswordHash = hashedPassword
            };

            Users.Add(newUser);

            return Ok(new
            {
                message = "Registration successful.",
                user = new
                {
                    userId = newUser.UserId,
                    username = newUser.Username
                }
            });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            string normalizedUsername = request.Username.Trim().ToLower();

            // 1. Retrieve user by username
            var user = Users.FirstOrDefault(u => u.Username == normalizedUsername);

            // 2. Verify existence and validate password hash
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            return Ok(new
            {
                message = "Login successful.",
                user = new
                {
                    userId = user.UserId,
                    username = user.Username
                }
            });
        }

        [HttpPatch("user/{id}")]
        public IActionResult Edit(string id, [FromBody] Edit request)
        {
            // validate route parameter
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { message = "User ID is required." });
            }

            // find user by UserId
            var user = Users.FirstOrDefault(u => string.Equals(u.UserId, id, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                return NotFound(new { Message = "User was not found" });
            }
            // for message
            List<string> updateMessages = new List<string>();
            // update username
            if (!string.IsNullOrWhiteSpace(request.Username))
            {
                bool usernameExist = Users.Any(u =>
                    !string.Equals(u.UserId, id, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(u.Username, request.Username, StringComparison.OrdinalIgnoreCase)
                );

                if (usernameExist)
                {
                    return Conflict(new { message = "Username is already taken by another user." });
                }
                string normalizedUsername = request.Username.Trim().ToLower();
                user.Username = normalizedUsername;
                updateMessages.Add("Username Updated Successfully.");
            }

            // update the password
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                updateMessages.Add("Password Updated Successfully.");
            }

            return Ok(new
            {
                status = "Success",
                message = updateMessages,
                user = new
                {
                    userId = user.UserId,
                    username = user.Username
                }
            });
        }

        [HttpGet("user/data")]
        public IActionResult GetData()
        {
            var users = Users.Select(user => new
            {
                user.UserId,
                user.Username
            }).ToList();

            return Ok(users);
        }
    }

    // Data Transfer Objects (DTOs)
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class Edit
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UserAccount
    {
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}