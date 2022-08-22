using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using WebApplicationIdentityWithDb.Services.interfaces;
using WebApplicationIdentityWithDb.Settings;

namespace WebApplicationIdentityWithDb.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;
        private readonly IOptions<Secrets> smtpSetting;

        public EmailService(IOptions<Secrets> smtpSetting, IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.smtpSetting = smtpSetting;
        }

        public async Task Send(string from, string to, string subject, string body)
        {
            var message = new MailMessage(
                    from,
                    to,
                    subject,
                    body);

            using var emailClient = new SmtpClient(smtpSetting.Value.EmailHost, smtpSetting.Value.EmailPort);
            emailClient.Credentials = new NetworkCredential(from, smtpSetting.Value.EmailKey);

            await emailClient.SendMailAsync(message);
        }
    }
}
