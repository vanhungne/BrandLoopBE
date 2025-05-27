using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Persistence;
using BrandLoop.Infratructure.Models.Authen;
using BrandLoop.Shared.Helper;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;

namespace BrandLoop.Infratructure.Reporitory
{
    public class EmailSenderRepository : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSenderRepository> _logger;
        private readonly BLDBContext  _dbcontext;
        public EmailSenderRepository(IConfiguration configuration, ILogger<EmailSenderRepository> logger, BLDBContext dbcontext)
        {
            _configuration = configuration;
            _logger = logger;
            _dbcontext = dbcontext;
        }

        public async Task<string> ConfirmEmailAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null)
            {
                return "Invalid confirmation request.";
            }

            if (user.Status == UserStatus.Active)
            {
                return "Your email has already been verified.";
            }

            user.Status = UserStatus.Active;
            await _dbcontext.SaveChangesAsync();

            return "Your account has been successfully confirmed.";
        }

        public async Task<bool> EmailSendAsync(string email, string subject, string message)
        {
            bool status = false;
            try
            {
                var secretKey = _configuration["AppSettings:SecretKey"];
                var from = _configuration["AppSettings:EmailSettings:From"];
                var smtpServer = _configuration["AppSettings:EmailSettings:SmtpServer"];
                var port = int.Parse(_configuration["AppSettings:EmailSettings:Port"]);
                var enableSSL = bool.Parse(_configuration["AppSettings:EmailSettings:EnablSSL"]);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(from),
                    Subject = subject,
                    Body = message,
                    BodyEncoding = Encoding.UTF8,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = port,
                    Credentials = new NetworkCredential(from, secretKey),
                    EnableSsl = enableSSL
                };

                // Gửi email
                await smtpClient.SendMailAsync(mailMessage);
                status = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending the email.");
                status = false;
            }
            return status;
        }

        public string GetMailBody(RegisterBaseModel model)
        {
            string apiUrl = _configuration["Host:https"];
            string token = GenerateVerificationToken(model.Email);

            // Liên kết xác nhận sử dụng phương thức GET
            string url = $"{apiUrl}/api/Authen/confirm-email?token={token}";
            return string.Format(@"
    <div style='text-align: center; font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
        <div style='max-width: 500px; margin: auto; background: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
            <h1 style='color: #333;'>Welcome to <span style='color: #008CBA;'>Tutoring</span></h1>
            <p style='font-size: 16px; color: #555;'>Click the button below to verify your email and start your journey with us.</p>
            <a href='{0}' style='display: inline-block; 
                                 text-decoration: none;
                                 background-color: #008CBA;
                                 color: #ffffff;
                                 font-size: 18px;
                                 font-weight: bold;
                                 padding: 12px 24px;
                                 border-radius: 8px;
                                 box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                                 transition: background-color 0.3s ease;'
               onmouseover='this.style.backgroundColor=""#0077A8""'
               onmouseout='this.style.backgroundColor=""#008CBA""'>
                Confirm Email
            </a>
            <p style='margin-top: 20px; font-size: 14px; color: #777;'>If you did not sign up for SmokeFreeHub, you can ignore this email.</p>
        </div>
    </div>", url);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email), "Username cannot be null or empty."); // Ném ngoại lệ nếu username không hợp lệ
            }
            return await _dbcontext.Users?.FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public string GenerateVerificationToken(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Username cannot be null or empty.", nameof(email));
            }

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, email),
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTimeHelper.GetVietnamNow().AddMinutes(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetMailBody(RegisterBaseModel model, string accountType = "User")
        {
            if (accountType == "Brand" || accountType == "KOL")
            {
                // Custom template for Brand/KOL registration pending approval
                return string.Format(@"
<div style='text-align: center; font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
    <div style='max-width: 500px; margin: auto; background: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
        <h1 style='color: #333;'>Welcome to <span style='color: #008CBA;'>BrandLoop</span></h1>
        <p style='font-size: 16px; color: #555;'>Thank you for registering as a {0}!</p>
        <p style='font-size: 16px; color: #555;'>Your registration is currently pending administrator approval.</p>
        <div style='background-color: #f5f5f5; padding: 15px; border-radius: 5px; margin: 20px 0;'>
            <p style='margin: 5px 0; color: #666;'><strong>Username:</strong> {1}</p>
            <p style='margin: 5px 0; color: #666;'><strong>Email:</strong> {2}</p>
            <p style='margin: 5px 0; color: #666;'><strong>Full Name:</strong> {3}</p>
        </div>
        <p style='margin-top: 20px; font-size: 14px; color: #777;'>If you did not register with BrandLoop, please ignore this email.</p>
    </div>
</div>", accountType, model.Email, model.Email, model.FullName);
            }
            else
            {
                // Original verification email for regular users
                string apiUrl = _configuration["Host:https"];
                string token = GenerateVerificationToken(model.Email);
                string url = $"{apiUrl}/api/Authen/confirm-email?token={token}";

                return string.Format(@"
    <div style='text-align: center; font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
        <div style='max-width: 500px; margin: auto; background: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
            <h1 style='color: #333;'>Welcome to <span style='color: #008CBA;'>BrandLoop</span></h1>
            <p style='font-size: 16px; color: #555;'>Click the button below to verify your email and start your journey with us.</p>
            <a href='{0}' style='display: inline-block; 
                                 text-decoration: none;
                                 background-color: #008CBA;
                                 color: #ffffff;
                                 font-size: 18px;
                                 font-weight: bold;
                                 padding: 12px 24px;
                                 border-radius: 8px;
                                 box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                                 transition: background-color 0.3s ease;'
               onmouseover='this.style.backgroundColor=""#0077A8""'
               onmouseout='this.style.backgroundColor=""#008CBA""'>
                Confirm Email
            </a>
            <p style='margin-top: 20px; font-size: 14px; color: #777;'>If you did not sign up for BrandLoop, you can ignore this email.</p>
        </div>
    </div>", url);
            }
        }
        public string GetApprovalEmailBody(User user, string accountType)
        {
            string loginUrl = _configuration["Host:https"] + "/login";

            return string.Format(@"
    <div style='text-align: center; font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
        <div style='max-width: 500px; margin: auto; background: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
            <h1 style='color: #333;'>Good News!</h1>
            <p style='font-size: 16px; color: #555;'>Your {0} account has been approved!</p>
            <p style='font-size: 16px; color: #555;'>You can now log in to your account and start using BrandLoop's features.</p>
            <a href='{1}' style='display: inline-block; 
                                 text-decoration: none;
                                 background-color: #4CAF50;
                                 color: #ffffff;
                                 font-size: 18px;
                                 font-weight: bold;
                                 padding: 12px 24px;
                                 border-radius: 8px;
                                 box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                                 transition: background-color 0.3s ease;'
               onmouseover='this.style.backgroundColor=""#45a049""'
               onmouseout='this.style.backgroundColor=""#4CAF50""'>
                Log In Now
            </a>
            <p style='margin-top: 20px; font-size: 14px; color: #777;'>Thank you for joining BrandLoop!</p>
        </div>
    </div>", accountType, loginUrl);
        }

        // Email template for account rejection
        public string GetRejectionEmailBody(User user, string accountType, string reason)
        {
            string supportEmail = _configuration["AppSettings:SupportEmail"] ?? "support@brandloop.com";

            return string.Format(@"
    <div style='text-align: center; font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
        <div style='max-width: 500px; margin: auto; background: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
            <h1 style='color: #333;'>Registration Update</h1>
            <p style='font-size: 16px; color: #555;'>We've reviewed your {0} account registration.</p>
            <p style='font-size: 16px; color: #555;'>Unfortunately, we are unable to approve your registration at this time.</p>
            <div style='background-color: #f5f5f5; padding: 15px; border-radius: 5px; margin: 20px 0; text-align: left;'>
                <p style='margin: 5px 0; color: #666;'><strong>Reason:</strong> {1}</p>
            </div>
            <p style='font-size: 16px; color: #555;'>If you think this is a mistake or need further assistance, please contact our support team at {2}.</p>
            <p style='margin-top: 20px; font-size: 14px; color: #777;'>Thank you for your interest in BrandLoop.</p>
        </div>
    </div>", accountType, reason, supportEmail);
        }
    }
}
