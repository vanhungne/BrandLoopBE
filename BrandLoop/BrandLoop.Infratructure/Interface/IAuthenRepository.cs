using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Models.Authen;

namespace BrandLoop.Infratructure.Interface
{
    public interface IAuthenRepository
    {
        Task<(string, string)> Login(LoginModel model);
        Task<(string, string)> LoginWithRefreshToken(string refreshToken);
        Task<string> Register(RegisterBaseModel model, IFormFile avatarFile);
        Task<string> RegisterBrand(RegisterBrandModel model, IFormFile avatarFile, IFormFile logoFile);
        Task<string> RegisterKOL(RegisterKOLModel model, IFormFile avatarFile);
        Task<bool> ApproveRegistration(string username);
        Task<bool> RejectRegistration(string username, string reason);
        Task<List<PendingRegistrationDto>> GetPendingRegistrations();
        string GenerateJwtToken(User user);
        Task<bool> Logout(string username);
        Task<User> GetUserByEmail(string email);

    }
}
