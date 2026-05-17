using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using SchoolManagementSystem.Api.Data;
using SchoolManagementSystem.Api.Models;
using SchoolManagementSystem.Api.Dtos;

namespace SchoolManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("sms")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public UserController(
            AppDbContext context,
            JwtService jwtService
        )
        {
            _context = context;
            _jwtService = jwtService;
        }

        // =========================================
        // GET ALL USERS
        // Protected Route
        // =========================================

        [Authorize]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();

            return Ok(users);
        }

        // =========================================
        // GET USER BY ID
        // =========================================

        [Authorize]
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        // =========================================
        // ADD USER
        // =========================================

        [HttpPost("user")]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            user.CreatedAt = DateTime.UtcNow;

            // TEMPORARY plain password storage
            // Later replace with BCrypt hashing

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User Added Successfully",
                data = user
            });
        }

        // =========================================
        // UPDATE USER
        // =========================================

        [Authorize]
        [HttpPut("user/{id}")]
        public async Task<IActionResult> UpdateUser(
            int id,
            [FromBody] User updatedUser
        )
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("User not found");

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.PasswordHash = updatedUser.PasswordHash;
            user.Role = updatedUser.Role;
            user.Age = updatedUser.Age;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"User {id} Updated Successfully",
                data = user
            });
        }

        // =========================================
        // DELETE USER
        // Admin Only
        // =========================================

        [Authorize(Roles = "Principal")]
        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("User not found");

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"User {id} Deleted Successfully"
            });
        }

        // =========================================
        // LOGIN
        // =========================================

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginDto loginDto
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Email == loginDto.Email
                );

            if (user == null)
                return Unauthorized("Invalid email or password");

            // TEMPORARY plain text comparison
            // Replace later with BCrypt.Verify()

            if (user.PasswordHash != loginDto.Password)
                return Unauthorized("Invalid email or password");

            // =====================================
            // Generate JWT Token
            // =====================================

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                message = "Login successful",

                token = token,

                user = new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Role
                }
            });
        }
    }
}