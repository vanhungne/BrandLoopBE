using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.Authen;
using Microsoft.AspNetCore.Http;


namespace BrandLoop.Application.Service
{
    public class AuthenService :IAuthenService
    {
        private readonly IAuthenRepository _repository;
        private readonly IEmailSender _emailSender;

        public AuthenService(IAuthenRepository repository, IEmailSender emailSender)
        {
            _repository = repository;
            _emailSender = emailSender;
        }

        public async Task<bool> ApproveRegistration(string uid)
        {
            return await _repository.ApproveRegistration(uid);
        }

        public async Task<string> ConfirmEmailAsync(string? email)
        {
            return await _emailSender.ConfirmEmailAsync(email);
        }

        public Task<bool> ForgotPasswordAsync(string email)
        {
            return _repository.ForgotPasswordAsync(email);
        }

        public string GenerateJwtToken(User user)
        {
            return _repository.GenerateJwtToken(user);
        }

        public async Task<List<PendingRegistrationDto>> GetApproveRegistrations()
        {
            return await _repository.GetApproveRegistrations();
        }

        public async Task<List<PendingRegistrationDto>> GetPendingRegistrations()
        {
            return await _repository.GetPendingRegistrations();
        }

        public async Task<(string, string, int)> Login(LoginModel model)
        {
            return await _repository.Login(model);
        }

        public async Task<(string, string, int)> LoginWithRefreshToken(string refreshToken)
        {
            return await _repository.LoginWithRefreshToken(refreshToken);
        }
        public async Task<string> Register(RegisterBaseModel model, IFormFile avatarFile)
        {
            return await _repository.Register(model, avatarFile);
        }

        public Task<string> RegisterBrand(RegisterBrandModel model, IFormFile avatarFile, IFormFile logoFile)
        {
            return _repository.RegisterBrand(model, avatarFile, logoFile);
        }

        public async Task<string> RegisterKOL(RegisterKOLModel model, IFormFile avatarFile)
        {
            return await _repository.RegisterKOL(model, avatarFile);
        }

        public async Task<bool> RejectRegistration(string uid, string reason)
        {
            return await _repository.RejectRegistration(uid, reason);
        }

        public Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            return _repository.ResetPasswordAsync(token, newPassword);
        }

        public async Task RevokeRefreshToken(string token)
        {
             await _repository.RevokeRefreshToken(token);
        }

        public Task<bool> ValidateResetTokenAsync(string token)
        {
            return _repository.ValidateResetTokenAsync(token);
        }
    }
}
