using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Models.Authen;
using Microsoft.AspNetCore.Http;

namespace BrandLoop.Application.Interfaces
{
    public interface IAuthenService
    {
        Task<(string, string)> Login(LoginModel model);
        Task<(string, string)> LoginWithRefreshToken(string refreshToken);
        Task<string> Register(RegisterBaseModel model, IFormFile avatarFile);
        Task<string> RegisterBrand(RegisterBrandModel model, IFormFile avatarFile, IFormFile logoFile);
        Task<string> RegisterKOL(RegisterKOLModel model, IFormFile avatarFile);
        Task<bool> ApproveRegistration(string uid);
        Task<bool> RejectRegistration(string uid, string reason);
        Task<List<PendingRegistrationDto>> GetPendingRegistrations();
        string GenerateJwtToken(User user);
        Task<string> ConfirmEmailAsync(string? email);
    }
}
