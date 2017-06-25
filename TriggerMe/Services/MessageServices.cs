using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

using System.Threading.Tasks;
using TriggerMe.Models;

namespace WebApplicationBasic.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link https://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public AuthMessageSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager
        public Task SendEmailAsync(string email, string subject, string message)
        {
            // Plug in your email service here to send an email.
            Execute( subject, message, email).Wait();
            return Task.FromResult(0);
        }

        public async Task Execute( string subject, string message, string email)
        {
       

            var mail = new MimeMessage();
            mail.From.Add(new MailboxAddress("TriggerME", "noreply@triggerme.net"));
            mail.To.Add(new MailboxAddress(email.Split('@')[0], email));
            mail.Subject = subject;
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = message;
            mail.Body = bodyBuilder.ToMessageBody();
           

            using (var client = new SmtpClient())
            {
              await  client.ConnectAsync("mail.triggerme.net", 25, false);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
              
               await client.AuthenticateAsync("noreply@triggerme.net", "TriggerMe@2017");
               await  client.SendAsync(mail);
                client.Disconnect(true);
            }
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
