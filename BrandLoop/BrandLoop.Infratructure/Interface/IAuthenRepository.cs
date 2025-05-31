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
        Task<(string, string, int)> Login(LoginModel model);
        Task<(string, string, int)> LoginWithRefreshToken(string refreshToken);
        Task<string> Register(RegisterBaseModel model, IFormFile avatarFile);
        Task<string> RegisterBrand(RegisterBrandModel model, IFormFile avatarFile, IFormFile logoFile);
        Task<string> RegisterKOL(RegisterKOLModel model, IFormFile avatarFile);
        Task<bool> ApproveRegistration(string uid);
        Task<bool> RejectRegistration(string uid, string reason);
        Task<List<PendingRegistrationDto>> GetPendingRegistrations();
        Task<List<PendingRegistrationDto>> GetApproveRegistrations();
        string GenerateJwtToken(User user);
        Task<bool> Logout(string uid);
        Task<User> GetUserByEmail(string email);


    }
}
