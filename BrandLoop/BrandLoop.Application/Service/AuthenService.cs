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

        public async Task<bool> ApproveRegistration(string username)
        {
            return await _repository.ApproveRegistration(username);
        }

        public async Task<string> ConfirmEmailAsync(string? username)
        {
            return await _emailSender.ConfirmEmailAsync(username);
        }

        public string GenerateJwtToken(User user)
        {
            return _repository.GenerateJwtToken(user);
        }

        public async Task<List<PendingRegistrationDto>> GetPendingRegistrations()
        {
            return await _repository.GetPendingRegistrations();
        }

        public async Task<(string, string)> Login(LoginModel model)
        {
            return await _repository.Login(model);
        }

        public async Task<(string, string)> LoginWithRefreshToken(string refreshToken)
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

        public async Task<bool> RejectRegistration(string username, string reason)
        {
            return await _repository.RejectRegistration(username, reason);
        }
    }
}
