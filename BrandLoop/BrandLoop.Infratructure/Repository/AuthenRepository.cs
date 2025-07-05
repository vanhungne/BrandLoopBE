using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.Authen;
using BrandLoop.Infratructure.Persistence;
using BrandLoop.Infratructure.Reporitory;
using BrandLoop.Shared.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BrandLoop.Infratructure.ReporitorY
{
    public class AuthenRepository : IAuthenRepository
    {
        private readonly BLDBContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;
        private readonly INotificationRepository _notificationRepository;

        public AuthenRepository(BLDBContext context, IConfiguration configuration, IEmailSender emailSender, IMapper mapper, INotificationRepository notificationRepository)
        {
            _context = context;
            _configuration = configuration;
            _emailSender = emailSender;
            _mapper = mapper;
            _notificationRepository = notificationRepository;
        }

        public string GenerateJwtToken(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            var claims = new[]
            {
                    new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
                    new Claim(ClaimTypes.NameIdentifier, user.UID?.ToString() ?? string.Empty),
                    new Claim(ClaimTypes.Role, user.RoleId == 1 ? "Admin" : user.RoleId == 2 ? "Brand":  user.RoleId == 3 ? "Influencer" :"Guest"),
                    new Claim("RoleId", user.RoleId.ToString() ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    new Claim("Avatar", user.ProfileImage?.ToString() ?? string.Empty),
                };



            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTimeHelper.GetVietnamNow().AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<string> GenerateRefreshToken(string uid)
        {
            var refreshToken = Guid.NewGuid().ToString();
            var expriDate = DateTimeHelper.GetVietnamNow().AddDays(7);

            var newToken = new RefreshTokens
            {
                Token = refreshToken,
                UID = uid,
                Expires = expriDate,
            };
            _context.RefreshTokens.Add(newToken);
            await _context.SaveChangesAsync();
            return refreshToken;
        }

        public async Task<RefreshTokens> ValidateRefreshToken(string token)
        {
            var refreshToken = await _context.RefreshTokens
                .SingleOrDefaultAsync(rt => rt.Token == token && rt.Expires > DateTimeHelper.GetVietnamNow());

            return refreshToken;
        }

        public async Task RevokeRefreshToken(string token)
        {
            var refreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.Token == token);

            if (refreshToken != null)
            {
                _context.RefreshTokens.Remove(refreshToken);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> GenerateAccessToken(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.UID),
        new Claim(ClaimTypes.Role, user.Role.RoleName),
        new Claim("RoleId", user.RoleId.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
    };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTimeHelper.GetVietnamNow().AddMinutes(300),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(z => z.Email.Equals(email));
        }

        public async Task<(string, string, int)> Login(LoginModel model)
        {
            var user = await _context.Users.Include(r => r.Role).Where(x => x.Status == UserStatus.Active)
                .SingleOrDefaultAsync(x => x.Email.Equals(model.Email));
            if (user == null)
            {
                return (null, null ,0);
            }
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                return (null, null,0);
            }
            var accessToken = await GenerateAccessToken(user);

            var refreshToken = await GenerateRefreshToken(user.UID);
            var roleId = user.RoleId;
            user.LastLogin = DateTimeHelper.GetVietnamNow();
            await _context.SaveChangesAsync();

            return (accessToken, refreshToken, roleId);
        }

        public async Task<(string, string, int)> LoginWithRefreshToken(string refreshToken)
        {
            var refreshTokenEntity = await ValidateRefreshToken(refreshToken);

            if (refreshTokenEntity == null)
                return (null, null,0);

            var user = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.UID == refreshTokenEntity.UID && u.Status == UserStatus.Active)
                .SingleOrDefaultAsync();

            if (user == null)
                return (null, null,0);

            var accessToken = await GenerateAccessToken(user);
            var newRefreshToken = await GenerateRefreshToken(user.Email);
            var roleId = user.RoleId;
            user.LastLogin = DateTimeHelper.GetVietnamNow();
            await _context.SaveChangesAsync();
            return (accessToken, newRefreshToken, roleId);
        }

        public async Task<bool> Logout(string email)
        {
            return await Task.FromResult(true);
        }

        public async Task<string> Register(RegisterBaseModel model, IFormFile avatarFile)
        {
            var validationResult = await ValidateBaseRegistration(model);
            if (validationResult != null)
                return validationResult;
            string avatarUrl = null;
            if (avatarFile != null)
            {
                var cloudinaryService = new CloundinaryRepository(_configuration);
                avatarUrl = await
                    cloudinaryService.UploadImage(avatarFile);
            }

            var id = GenerateCompactUid();
            id = await _context.Users.FirstOrDefaultAsync(u => u.UID == id) != null ? GenerateCompactUid() : id;
            var user = new User
            {
                UID = id,
                Email = model.Email,
                Phone = model.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                FullName = model.FullName,
                ProfileImage = avatarUrl ?? "Null avatar",
                RoleId = 1,
                Status = UserStatus.Active,
                CreatedAt = DateTimeHelper.GetVietnamNow(),
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            string emailBody = _emailSender.GetMailBody(model);
            bool emailSent = await _emailSender.EmailSendAsync(model.Email, "Member Account Created", emailBody);

            return emailSent ? "Please check email to verify your account." : "There was an error sending the email. Please try again later.";
        }
        private async Task<string> ValidateBaseRegistration(RegisterBaseModel model)
        {
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                return "Email already exists";

            if (await _context.Users.AnyAsync(u => u.Phone == model.PhoneNumber))
                return "Phone number already exists";

            return null;
        }

        public async Task<string> RegisterBrand(RegisterBrandModel model, IFormFile avatarFile, IFormFile logoFile)
        {
            var validationResult = await ValidateBaseRegistration(model);
            if (validationResult != null)
                return validationResult;

            string avatarUrl = null;
            if (avatarFile != null)
            {
                var cloudinaryService = new CloundinaryRepository(_configuration);
                avatarUrl = await cloudinaryService.UploadImage(avatarFile);
            }

            string logoUrl = null;
            if (logoFile != null)
            {
                var cloudinaryService = new CloundinaryRepository(_configuration);
                logoUrl = await cloudinaryService.UploadImage(logoFile);
            }

            // Create user with pending status (Status = 0)
            var id = GenerateCompactUid();
            id = await _context.Users.FirstOrDefaultAsync(u => u.UID == id) != null ? GenerateCompactUid() : id;

            var user = new User
            {
                UID = id,
                Email = model.Email,
                Phone = model.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                FullName = model.FullName,
                ProfileImage = avatarUrl ?? "default-avatar.jpg",
                RoleId = 2, // Role for Brand
                Status = 0, // Pending approval
                CreatedAt = DateTimeHelper.GetVietnamNow(),
                UpdatedAt = DateTimeHelper.GetVietnamNow()
            };

            // Create brand profile
            var brandProfile = new BrandProfile
            {
                UID = id,
                CompanyName = model.CompanyName,
                Industry = model.Industry,
                Website = model.Website,
                Logo = logoUrl ?? "default-logo.jpg",
                CompanySize = model.CompanySize,
                Description = model.Description,
                Address = model.Address,
                TaxCode = model.TaxCode,
                EstablishedYear = model.EstablishedYear,
                Facebook = model.Facebook,
                Instagram = model.Instagram,
                Tiktok = model.Tiktok,
                CreatedAt = DateTimeHelper.GetVietnamNow(),
                UpdatedAt = DateTimeHelper.GetVietnamNow()
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                    await _context.BrandProfiles.AddAsync(brandProfile);
                    await _context.SaveChangesAsync();

                    // Notify admin about new registration
                    await NotifyAdminsAboutRegistration(user.Email, "Brand");

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return "Registration failed: " + ex.Message;
                }
            }

            //string emailBody = _emailSender.GetMailBody(model, "Brand");
            //bool emailSent = await _emailSender.EmailSendAsync(model.Email, "Brand Registration Submitted", emailBody);

            return "Your brand registration has been submitted.";
        }

        // New methods for KOL registration
        public async Task<string> RegisterKOL(RegisterKOLModel model, IFormFile avatarFile)
        {
            var validationResult = await ValidateBaseRegistration(model);
            if (validationResult != null)
                return validationResult;

            string avatarUrl = null;
            if (avatarFile != null)
            {
                var cloudinaryService = new CloundinaryRepository(_configuration);
                avatarUrl = await cloudinaryService.UploadImage(avatarFile);
            }

            // Tìm phân khúc influencer phù hợp dua vào số lượng người theo dõi
            int? typeId = 5;
            if (model.FollowerCount.HasValue)
            {
                var type = await _context.InfluencerTypes
                    .FirstOrDefaultAsync(p =>
                        model.FollowerCount >= p.MinFollower &&
                        model.FollowerCount < p.MaxFollower);

                typeId = type?.Id;
            }


            // Create user with pending status (Status = 0)
            var id = GenerateCompactUid();
            id = await _context.Users.FirstOrDefaultAsync(u => u.UID == id) != null ? GenerateCompactUid() : id;

            var user = new User
            {
                UID = id,
                Email = model.Email,
                Phone = model.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                FullName = model.FullName,
                ProfileImage = avatarUrl ?? "default-avatar.jpg",
                RoleId = 3, // Role for KOL
                Status = 0, // Pending approval
                CreatedAt = DateTimeHelper.GetVietnamNow(),
                UpdatedAt = DateTimeHelper.GetVietnamNow()
            };

            // Create KOL profile
            var kolProfile = new InfluenceProfile
            {
                UID = id,
                Nickname = model.Nickname,
                Bio = model.Bio,
                ContentCategory = model.ContentCategory,
                Location = model.Location,
                Languages = model.Languages,
                PortfolioUrl = model.PortfolioUrl,
                Verified = false,
                Facebook = model.Facebook,
                Instagram = model.Instagram,
                Tiktok = model.Tiktok,
                Youtube = model.Youtube,
                FollowerCount = model.FollowerCount,
                Gender = model.Gender,
                DayOfBirth = model.DateOfBirth,
                CreatedAt = DateTimeHelper.GetVietnamNow(),
                UpdatedAt = DateTimeHelper.GetVietnamNow(),
                InfluencerTypeId = typeId
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                    await _context.InfluenceProfiles.AddAsync(kolProfile);
                    await _context.SaveChangesAsync();

                    // Notify admin about new registration
                    await NotifyAdminsAboutRegistration(user.Email, "Influencer");

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return "Registration failed: " + ex.Message;
                }
            }

            //string emailBody = _emailSender.GetMailBody(model, "Influencer");
            //bool emailSent = await _emailSender.EmailSendAsync(model.Email, "Influencer Registration Submitted", emailBody);

            return "Your Influencer registration has been submitted";
        }

        // Method to get all pending registrations for admin
        public async Task<List<PendingRegistrationDto>> GetPendingRegistrations()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.BrandProfile)
                .Include(u => u.InfluenceProfile)
                .Where(u => u.Status == UserStatus.Inactive)
                .ToListAsync();

            var result = _mapper.Map<List<PendingRegistrationDto>>(users);
            return result;
        }
        public async Task<List<PendingRegistrationDto>> GetApproveRegistrations()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.BrandProfile)
                .Include(u => u.InfluenceProfile)
                .Where(u => u.Status == UserStatus.Active || u.Status == UserStatus.Banned)
                .ToListAsync();

            var result = _mapper.Map<List<PendingRegistrationDto>>(users);
            return result;
        }

        // Method to approve registration
        public async Task<bool> ApproveRegistration(string uid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UID == uid && u.Status == 0);

            if (user == null)
                return false;

            user.Status = UserStatus.Active; // Set to active
            user.UpdatedAt = DateTimeHelper.GetVietnamNow();

            await _context.SaveChangesAsync();

            // Send approval email
            string emailBody = $"Dear {user.FullName},<br><br>" +
                               $"Your registration has been approved. You can now log in to your account.<br><br>" +
                               $"Best regards,<br>BrandLoop Team";

            await _emailSender.EmailSendAsync(user.Email, "Registration Approved", emailBody);

            // Send notification to user
            string notificationType = user.RoleId == 2 ? "Brand" : "Influencer";
            string notificationMessage = $"Your {notificationType} registration has been approved.";

            await _notificationRepository.CreateNotification(
                new Notification
                {
                    UID = uid,
                    Content = notificationMessage,
                    NotificationType = "Registration",
                    IsRead = false,
                    CreatedAt = DateTimeHelper.GetVietnamNow()
                });

            return true;
        }

        // Method to reject registration
        public async Task<bool> RejectRegistration(string uid, string reason)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UID == uid && u.Status == 0);

            if (user == null)
                return false;

            // Store rejection reason somewhere if needed
            user.Status = UserStatus.Banned; // Set to rejected
            user.UpdatedAt = DateTimeHelper.GetVietnamNow();

            await _context.SaveChangesAsync();

            // Send rejection email
            string emailBody = $"Dear {user.FullName},<br><br>" +
                               $"We regret to inform you that your registration has been rejected.<br><br>" +
                               $"Reason: {reason}<br><br>" +
                               $"If you believe this was a mistake or need further assistance, please contact our support team.<br><br>" +
                               $"Best regards,<br>BrandLoop Team";

            await _emailSender.EmailSendAsync(user.Email, "Registration Rejected", emailBody);

            return true;
        }

        // Helper method to notify admins about new registrations
        private async Task NotifyAdminsAboutRegistration(string uid, string accountType)
        {
            // Get user registration
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UID == uid);
            if (user == null)
                return;

            // Get all admin users
            var admins = await _context.Users
                .Where(u => u.RoleId == 1 && u.Status == UserStatus.Active)
                .ToListAsync();

            foreach (var admin in admins)
            {
                await _notificationRepository.CreateNotification(
                    new Notification
                    {
                        UID = admin.UID,
                        Content = $"New {accountType} registration: {user.Email} requires approval",
                        NotificationType = "AdminRegistration",
                        IsRead = false,
                        CreatedAt = DateTimeHelper.GetVietnamNow()
                    });
            }
        }
        public static string GenerateCompactUid()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
