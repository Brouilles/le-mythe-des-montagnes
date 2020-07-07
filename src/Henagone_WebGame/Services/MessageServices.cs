using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;

namespace Henagone_WebGame.Services
{
    public class AuthMessageSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
           /* var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("Dezeiraud Gaëtan - Henagone", "email@dezeiraud.com"));
            mimeMessage.To.Add(new MailboxAddress(email, email));
            mimeMessage.Subject = subject;
            mimeMessage.Body = new TextPart("plain")
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                client.Connect("mail.firstheberg.net", 587, false);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate("email@dezeiraud.com", "84KSHsdq418qs");
                client.Send(mimeMessage);
                client.Disconnect(true);
            }*/

            return Task.FromResult(0);
        }
    }
}
