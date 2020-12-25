using System;
using System.IO;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public async Task SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            await SendEmailMessage(emailMessage);
        }

        private async Task SendEmailMessage(MimeMessage emailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_emailConfig.Username, _emailConfig.Password);
                    await client.SendAsync(emailMessage);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(_emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            var messageBody = new BodyBuilder { HtmlBody = message.Content };

            if (!string.IsNullOrWhiteSpace(message.Attachment))
            {
                var file = new FileInfo(message.Attachment);
                messageBody.Attachments.Add(file.Name, File.ReadAllBytes(message.Attachment));
            }
            emailMessage.Body = messageBody.ToMessageBody();

            return emailMessage;
        }
    }
}
