using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Uni.Scan.Application.Interfaces.Services;
using Uni.Scan.Shared.Configurations;
using Uni.Scan.Transfer.Requests.Mail;

namespace Uni.Scan.Infrastructure.Services.Email
{
    public class SmtpMailService : IMailService
    {
        private readonly MailConfiguration _config;
        private readonly ILogger<SmtpMailService> _logger;

        public SmtpMailService(IOptions<MailConfiguration> config, ILogger<SmtpMailService> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        public async Task SendAsync(MailRequest request)
        {
            try
            {
                MimeMessage email = new MimeMessage();
                if (false)
                {
                    email.From.Add(new MailboxAddress(_config.DisplayName, _config.From));
                    email.To.Add(new MailboxAddress(_config.DevTo, _config.DevTo));
                }
                else
                {
                    email.From.Add(new MailboxAddress(_config.DisplayName, _config.From));
                    email.To.Add(new MailboxAddress(request.To, request.To));
                }

                email.Subject = request.Subject;

                BodyBuilder bodyBuilder = new BodyBuilder()
                {
                    HtmlBody = request.Body // ,TextBody = "Hello World!"
                };

                //IHostingEnvironment env
                //bodyBuilder.Attachments.Add(env.WebRootPath + "\\file.png");

                email.Body = bodyBuilder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_config.Host, _config.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_config.UserName, _config.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }
        }
    }
}