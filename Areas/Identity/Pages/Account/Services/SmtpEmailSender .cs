using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace HospitalManagement.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;

        // Constructor to bind the settings
        public SmtpEmailSender(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email), "Email address cannot be null or empty.");
            }

            var mailMessage = new MailMessage()
            {
                From = new MailAddress(_smtpSettings.From), // Use the 'From' email from settings
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mailMessage.To.Add(new MailAddress(email)); // Adding recipient email

            using (var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
            {
                smtpClient.Credentials = new NetworkCredential(_smtpSettings.User, _smtpSettings.Password);
                smtpClient.EnableSsl = _smtpSettings.EnableSsl;

                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}
