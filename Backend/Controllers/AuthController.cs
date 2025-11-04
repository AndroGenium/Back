using Azure.Core;
using Backend.Data;
using Backend.Models;
using Backend.Requests;
using Backend.Responses;
using Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Baza _db;
        private readonly ProcessDateInfo _dateformater;
        private readonly EmailService _emailService;

        public AuthController(Baza db, ProcessDateInfo dateformater, EmailService emailService)
        {
            _db = db;
            _dateformater = dateformater;
            _emailService = emailService;
        }

        [HttpPost("register-user")]
        public ActionResult AddUser([FromBody] AddUser RequestInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_db.Users.Any(u => u.Email == RequestInfo.Email))
            {
                return Conflict(new ApiResponse(ErrorCodes.EmailInUse, "Email Is In Use", ModelState));
            }

            var verificationCode = new Random().Next(100000, 999999).ToString();

            var UserToAdd = new User
            {
                FirstName = RequestInfo.FirstName,
                LastName = RequestInfo.LastName,
                Password = RequestInfo.password,
                Email = RequestInfo.Email,
                DateOfBirth = _dateformater.ConvertFormatToTime(RequestInfo.BirthDate),
                DateCreated = DateTime.Now,
                VerificationCode = verificationCode,
                VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(30)
            };

            _db.Users.Add(UserToAdd);
            _db.SaveChanges();

            _emailService.SendEMailConfirmation(UserToAdd.Email, verificationCode);

            return Ok(new ApiResponse("USR401", "\"User created. Check your email to verify your account.\"", UserToAdd));

        }

        [HttpPost("login-user")]
        public ActionResult LoginUser([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email and password are required.");

            var user = _db.Users.FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized("Invalid credentials.");

            if (!user.VerifyPass(request.Password))
                return Unauthorized("Invalid credentials.");

            if (user.IsBanned)
                return Forbid("This account is banned.");

            // Update last login time
            user.LastLogin = DateTime.UtcNow;
            _db.SaveChanges();


            return Ok(user);
        }

        [HttpPost("verify-email")]

        public ActionResult VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == request.Email);
            if (user == null)
                return NotFound("User not found");

            if (user.IsVerified)
                return BadRequest("User already verified");

            if (user.VerificationCode != request.Code || user.VerificationCodeExpiry < DateTime.UtcNow)
                return BadRequest("Invalid or expired code");

            user.IsVerified = true;
            user.VerificationCode = null;
            user.VerificationCodeExpiry = null;

            _db.SaveChanges();

            return Ok(new { Message = "Email verified successfully. You can now log in." });
        }
    }
}
