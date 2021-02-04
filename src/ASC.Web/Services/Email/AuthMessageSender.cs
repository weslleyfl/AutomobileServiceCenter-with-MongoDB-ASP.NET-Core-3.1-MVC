using ASC.Web.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace ASC.Web.Services.Email
{
    public class AuthMessageSender : IEmailSender
    {
        private readonly IOptions<ApplicationSettings> _settings;
        public AuthMessageSender(IOptions<ApplicationSettings> settings)
        {
            _settings = settings;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // TODO: Email padrao inserido manualmente - SendEmailAsync para testes
            email = "weslleylopes@fundep.com.br";

            await Execute(email, subject, htmlMessage);
        }

        private async Task Execute(string email, string subject, string htmlMessage)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Servico Email", _settings.Value.SMTPAccount));
            emailMessage.To.Add(new MailboxAddress("Mr. Weslley", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("html") { Text = htmlMessage };

            using (var client = new SmtpClient())
            {
                
                await client.ConnectAsync(_settings.Value.SMTPServer, _settings.Value.SMTPPort, MailKit.Security.SecureSocketOptions.None, default);

                // Note: only needed if the SMTP server requires authentication
                //await client.AuthenticateAsync(_settings.Value.SMTPAccount, _settings.Value.SMTPPassword);

                // TODO: Email nao enviado - desabilitado
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        //private void ConfigurationEmail()
        //{
        //    // *************** SEND EMAIL *******************
        //    using (var client = new MailKit.Net.Smtp.SmtpClient(new ProtocolLogger("smtp.log")))
        //    {
        //        client.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;

        //        //accept all SSL certificates
        //        client.ServerCertificateValidationCallback = (s, c, h, e) => true;

        //        // Note: since we don't have an OAuth2 token, disable
        //        // the XOAUTH2 authentication mechanism.
        //        client.AuthenticationMechanisms.Remove("XOAUTH2");

        //        // client.Connect(emailSettings.SmtpServer, emailSettings.SmtpPort, emailSettings.IsSslEnabled);
        //        client.Connect(emailSettings.SmtpServer, emailSettings.SmtpPort, emailSettings.AuthType);

        //        if (emailSettings.IsAuthenticationRequired)
        //        {
        //            // Note: only needed if the SMTP server requires authentication
        //            client.Authenticate(emailSettings.SmtpUsername, emailSettings.SmtpPassword);
        //        }

        //        if (emailSettings.TimeOut == 0)
        //            emailSettings.TimeOut = 10;
        //        client.Timeout = emailSettings.TimeOut * 1000;

        //        client.Send(message);
        //        client.Disconnect(true);
        //    }
        //}
    }
}
