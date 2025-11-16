using Azure.Core;
using Backend.Extensions;
using Backend.Data;
using Backend.Enums;
using Backend.Models;
using Backend.Requests;
using Backend.Responses;
using Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegisterRequest = Backend.Requests.RegisterRequest;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Baza _db;
        private readonly ProcessDateInfo _dateformater;
        private readonly EmailService _emailService;
        private readonly JwtService _jwtService;

        public AuthController(Baza db, ProcessDateInfo dateformater, EmailService emailService, JwtService jwtService)
        {
            _db = db;
            _dateformater = dateformater;
            _emailService = emailService;
            _jwtService = jwtService;
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // 1. Validate input
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { Message = "Email and password are required" });

            // 2. Check if email already exists in verified users
            var existingUser = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsVerified);

            if (existingUser != null)
                return BadRequest(new { Message = "Email already registered" });

            // 3. Generate 6-digit code
            var verificationCode = new Random().Next(100000, 999999).ToString();

            // 4. Store PENDING user data temporarily (NOT in main Users table yet!)
            // Option A: Use a PendingUsers table
            var pendingUser = new PendingUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                BirthDate = _dateformater.ConvertFormatToTime(request.BirthDate),
            };

            pendingUser.SetPassword(request.Password);
            pendingUser.VerificationCode = verificationCode;
            pendingUser.VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(15);
            pendingUser.DateCreated = DateTime.UtcNow;


            await _db.PendingUsers.AddAsync(pendingUser);
            await _db.SaveChangesAsync();

            // 5. Send verification email
            await _emailService.SendEmailConfirmation(request.Email, verificationCode);

            // 6. Return success (NO account created yet!)
            return Ok(new { Message = "Verification code sent to your email" });
        }

        

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. Find user
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsVerified);

            if (user == null)
                return BadRequest(new { Message = "Invalid email or password" });

            // 2. Verify password
            if (!user.VerifyPass(request.Password))
                return BadRequest(new { Message = "Invalid email or password" });

            // 3. Generate JWT token
            var token = _jwtService.GenerateToken(user.Id, user.Email, user.Role);

            // 4. Return token
            return Ok(new
            {
                Success = true,
                Token = token,
                User = new
                {
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.Role
                }
            });
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            // 1. Find pending user
            var pendingUser = await _db.PendingUsers
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (pendingUser == null)
                return BadRequest(new { Message = "No pending registration found" });

            // 2. Check if code expired
            if (pendingUser.VerificationCodeExpiry < DateTime.UtcNow)
            {
                _db.PendingUsers.Remove(pendingUser);
                await _db.SaveChangesAsync();
                return BadRequest(new { Message = "Verification code expired. Please register again." });
            }

            // 3. Verify code
            if (pendingUser.VerificationCode != request.Code)
                return BadRequest(new { Message = "Invalid verification code" });

            // 4. Create user from pending user (no double-hashing!)
            var newUser = Models.User.CreateFromPendingUser(pendingUser);

            await _db.Users.AddAsync(newUser);

            // 5. Delete from pending users
            _db.PendingUsers.Remove(pendingUser);

            await _db.SaveChangesAsync();

            // 6. Generate JWT token
            var token = _jwtService.GenerateToken(newUser.Id, newUser.Email, newUser.Role);

            // 7. Return success with token
            return Ok(new
            {
                Success = true,
                Token = token,
                Message = "Email verified successfully"
            });
        }

        [HttpPost("resend-code")]
        public async Task<IActionResult> ResendCode([FromBody] ResendRequest request)
        {
            // 1. Find pending user
            var pendingUser = await _db.PendingUsers
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (pendingUser == null)
                return BadRequest(new { Message = "No pending registration found" });

            // 2. Generate new code
            var newCode = new Random().Next(100000, 999999).ToString();

            // 3. Update pending user
            pendingUser.VerificationCode = newCode;
            pendingUser.VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(15);

            await _db.SaveChangesAsync();

            // 4. Send new email
            await _emailService.SendEmailConfirmation(request.Email, newCode);

            return Ok(new { Success = true, Message = "New code sent" });
        }
    }
}
