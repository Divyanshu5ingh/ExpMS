using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Domain.Entities;
using System.Net.Mail;

namespace EMS.Users
{
    public class UserAppService : ApplicationService, IUserAppService
    {
        private readonly IIdentityUserRepository _userRepository;
        private readonly ISendGridClient _sendGridClient;
        private readonly string _fromEmail;

        public UserAppService(IIdentityUserRepository userRepository, ISendGridClient sendGridClient, string fromEmail)
        {
            _userRepository = userRepository;
            _sendGridClient = sendGridClient;
            _fromEmail = fromEmail;
        }

        public async Task<bool> CheckEmailExists(string email)
        {
            var users = await _userRepository.GetListAsync();
            bool emailExists = users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (!emailExists)
            {
                var message = new SendGridMessage();
                message.SetFrom(new EmailAddress(_fromEmail));
                message.AddTo(new EmailAddress(email));
                message.SetSubject("Welcome to My Website");
                message.AddContent(MimeType.Text, "Thank you for joining our website!");

                var response = await _sendGridClient.SendEmailAsync(message);
            }
            return emailExists;
        }
    }
}
