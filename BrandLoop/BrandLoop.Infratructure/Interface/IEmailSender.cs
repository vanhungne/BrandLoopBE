using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Models.Authen;

namespace BrandLoop.Infratructure.Interface
{
    public interface IEmailSender
    {
        Task<bool> EmailSendAsync(string email, string subject, string message);

        string GetMailBody(RegisterBaseModel model);
        Task<User> GetUserByEmailAsync(string email);
        Task UpdateUserAsync(User user);
        Task<string> ConfirmEmailAsync(string email);
        string GetMailBody(RegisterBaseModel model, string accountType = "User");
        string GetPasswordResetEmailBody(User user, string resetToken);
        string GetPasswordChangedEmailBody(User user);
    }
}
