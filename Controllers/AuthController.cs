﻿using HTTL_ERP.DataAccess;
using HTTL_ERP.DTO;
using HTTL_ERP.Extensions;
using HTTL_ERP.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HTTL_ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly HttlerpContext _context;
        private readonly MailService _mailService;

        public AuthController(IConfiguration configuration, HttlerpContext context, MailService mailService)
        {
            _configuration = configuration;
            _context = context;
            _mailService = mailService;
        }

        private static readonly char[] Characters =
       "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
        private static readonly Random Random = new Random();

        public static string GenerateRandomString(int length = 32)
        {
            if (length <= 0)
            {
                throw new ArgumentException("Length must be greater than 0", nameof(length));
            }

            char[] result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = Characters[Random.Next(Characters.Length)];
            }

            return new string(result);
        }

        [HttpPost("ResetPassword/{email}")]
        public async Task<IActionResult> ResetPassword(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Generate a reset token
            string resetToken = GenerateRandomString();
            DateTime expiry = DateTime.Now.AddHours(1); // Token hết hạn sau 1 giờ

            // Save token and expiry to database
            user.VerifyToken = resetToken;
            user.VerifyTokenExpired = expiry;
            await _context.SaveChangesAsync();

            // Generate reset link with token and phone number
            string resetLink = $"http://127.0.0.1:5500/confirmResetPassword.html";

            var subject = "Reset Password";
            var message = $"Click the link below to reset your password:\n{resetLink}\n\nLink will expire in 10 minutes.";

            // Send email
            _mailService.SendEmailAsync(user.Email, subject, message);

            // Return result including phoneNumber and token
            return Ok(new
            {
                message = "Password reset link has been sent to your email.",
                phoneNumber = user.Email,
                token = resetToken
            });
        }

        [HttpPost("ConfirmResetPassword")]
        public async Task<IActionResult> ConfirmResetPassword([FromQuery] string email, [FromQuery] string token, [FromBody] ResetPasswordRequestDTO request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.VerifyTokenExpired > DateTime.UtcNow);

            if (user == null)
            {
                return BadRequest(new { message = "Invalid or expired request." });
            }

            // Check token validity
            if (user.VerifyToken != token)
            {
                return BadRequest(new { message = "Invalid or expired token." });
            }

            // Hash new password and save to DB
            user.Password = request.NewPassword.Hash();
            user.VerifyToken = null; // Remove token
            user.VerifyTokenExpired = null; // Remove token expiry
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password has been reset successfully." });
        }

        [HttpPost("ResendResetPasswordToken/{email}")]
        public async Task<IActionResult> ResendResetPasswordToken(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            if (user.VerifyToken == null || user.VerifyTokenExpired == null)
            {
                return BadRequest(new { message = "No reset token found. Please request a password reset first." });
            }

            // Kiểm tra mã hiện tại đã hết hạn hay chưa
            if (user.VerifyTokenExpired > DateTime.Now)
            {
                return BadRequest(new { message = "Current reset token is still valid. Please use the existing link." });
            }

            // Tạo mã mới và cập nhật thời gian hết hạn
            string newResetToken = GenerateRandomString();
            DateTime newExpiry = DateTime.Now.AddMinutes(10);

            user.VerifyToken = newResetToken;
            user.VerifyTokenExpired = newExpiry;
            await _context.SaveChangesAsync();

            // Tạo link khôi phục mới
            string resetLink = $"{Request.Scheme}://{Request.Host}/api/Profile/ConfirmResetPassword?phoneNumber={user.Email}&token={newResetToken}";

            var subject = "Resend Password Reset Token";
            var message = $"Your new password reset link:\n{resetLink}\n\nThis link will expire in 10 minutes.";

            // Gửi email
            _mailService.SendEmailAsync(user.Email, subject, message);

            return Ok(new { message = "A new reset link has been sent to your email." });
        }


        [HttpPost("CreateAdmin")]
        public async Task<IActionResult> CreateAdmin([FromBody] RegisterDTO registerDTO)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == registerDTO.Username);

            if (existingUser != null)
                return BadRequest(new { message = "Username is already exists" });

            // Tạo người dùng nhưng chưa kích hoạt
            User user = new User
            {
                Username = registerDTO.Username,
                Password = registerDTO.Password.Hash(),
                Role = 0,
                Dob =  registerDTO.Dob,
                Phone = registerDTO.Phone,
                Email = registerDTO.Email,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Create admin account successfully." });
        }

        //tạo tài khoản cho các role 1 2 3 
        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount([FromBody] RegisterDTO registerDTO)
        {
            if (registerDTO.Role == 0)
                return BadRequest(new { message = "Không thể tạo tài khoản với role = 0." });

            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == registerDTO.Username);
            if (existingUser != null)
                return BadRequest(new { message = "Username is already exists" });

            // Tạo người dùng nhưng chưa kích hoạt
            User user = new User
            {
                Username = registerDTO.Username,
                Password = registerDTO.Password.Hash(),
                Role = registerDTO.Role,
                Dob = registerDTO.Dob,
                Phone = registerDTO.Phone,
                Email = registerDTO.Email,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Create admin account successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == loginDTO.Username);

            if (user == null)
                return BadRequest(new { message = "Sai tên đăng nhập." });

            // Kiểm tra mật khẩu (so sánh đã băm)
            if (!loginDTO.Password.Verify(user.Password))
                return BadRequest(new { message = "Sai mật khẩu." });


            user.LoginTime = DateTime.Now;
            await _context.SaveChangesAsync();

            // Tạo JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);

            // Cập nhật Issuer và Audience trong SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Token hết hạn sau 1 giờ
                Issuer = _configuration["Jwt:Issuer"],  // Thêm Issuer
                Audience = _configuration["Jwt:Audience"],  // Thêm Audience
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Trả về JWT token
            return Ok(new
            {
                token = tokenString,
                userId = user.Id,
                username = user.Username,
                role = user.Role
            });
        }

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDTO dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.NewPassword) || string.IsNullOrEmpty(dto.OldPassword))
            {
                return BadRequest(new { message = "Invalid request data." });
            }

            if (dto.NewPassword == dto.OldPassword)
            {
                return BadRequest(new { message = "New password cannot equal with old password." });
            }

            var user = await _context.Users
                .Where(a => a.Email == dto.Email)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            if (!dto.OldPassword.Verify(user.Password))
            {
                return BadRequest("Wrong password.");
            }

            user.Password = dto.NewPassword.Hash();
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password updated successfully." });
        }





    }
}
