using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MimeKit;
using MimeKit.Cryptography;
using MimeKit.IO;
using MimeKit.Text;
using MailKit;
using MailKit.Net;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;


namespace AuthoBson.Messaging
{

    public abstract class MailSender
    {
        private MimeMessage Message { get; set; }

        private string Address { get; set; }

        private string Password { get; set; }

        public MailSender(IDomainSettings settings, InternetAddress to, string subject, string body)
        {
            Message = new(settings.Address, to, subject, body);

            Encoding.UTF8.GetBytes(settings.Address);

            Password = settings.Password;
        }

        public MailSender(IDomainSettings settings)
        {
            Message = new();

            Address = settings.Address;

            Password = settings.Password;

        }

        public void Send(string receiver, string subject, string body)
        {
            SmtpClient client = new();

            Message.From.Add(InternetAddress.Parse(Address));
            Message.To.Add(InternetAddress.Parse(receiver));
            Message.Subject = subject;
            BodyBuilder bbody = new();
            bbody.TextBody = body;
            Message.Body = bbody.ToMessageBody();

            client.ConnectAsync("smtp.gmail.com", 465);

            client.AuthenticateAsync(Address, Password);

            client.SendAsync(Message);
        }

        public async Task SendAsync(string receiver, string subject, string body)
        {
            SmtpClient client = new();

            Message.From.Add(InternetAddress.Parse(Address));
            Message.To.Add(InternetAddress.Parse(receiver));
            Message.Subject = subject;
            BodyBuilder bbody = new();
            bbody.TextBody = body;
            Message.Body = bbody.ToMessageBody();


            client.ConnectAsync("smtp.gmail.com", 465).Wait();

            client.AuthenticateAsync(Address, Password).Wait();

            await client.SendAsync(Message);
        }
    }

    public sealed class SMTPMail : MailSender
    {

        private MimeMessage Message { get; set; }

        private string Address { get; set; }

        private string Password { get; set; }

        public SMTPMail(IDomainSettings settings, InternetAddress to, string subject, string body) :
        base(settings, to, subject, body)
        { }

        public SMTPMail(IDomainSettings settings) :
        base(settings)
        { }
    }
}
