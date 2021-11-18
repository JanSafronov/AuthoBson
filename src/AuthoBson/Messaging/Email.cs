using System;
using System.Collections;
using System.Text;
using System.Text.Encodings;
using System.IO;
using System.IO.Compression;
using MimeKit;
using MimeKit.Cryptography;
using MimeKit.IO;
using MimeKit.Text;
using MailKit;
using MailKit.Net;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using AuthoBson.Email.Settings;


namespace AuthoBson.Email {

    public abstract class MailSender {
        private MimeMessage Message { get; set; }

        private string Address { get; set; }

        private string Password { get; set; }

        public MailSender(IDomainSettings settings, InternetAddress to, string subject, string body) {
            this.Message = new(settings.Address, to, subject, body);

            System.Text.Encoding.UTF8.GetBytes(settings.Address);

            this.Password = settings.Password;
        }

        public MailSender(IDomainSettings settings) {
            this.Message = new();

            this.Address = settings.Address;
            
            this.Password = settings.Password;
            
        }

        public void Send(string receiver, string subject, string body) { 
            SmtpClient client = new();

            Message.From.Add(InternetAddress.Parse(Address));
            Message.To.Add(InternetAddress.Parse(receiver));
            Message.Subject = subject;
            BodyBuilder bbody = new();
            bbody.TextBody = body;
            Message.Body = bbody.ToMessageBody();

            client.Connect("smtp.gmail.com", 465);

            client.Authenticate(Address, Password);

            client.Send(Message);
        }
    }

    public sealed class SMTPMail : MailSender {

        private MimeMessage Message { get; set; }

        private string Address { get; set; }

        private string Password { get; set; }

        public SMTPMail(IDomainSettings settings, InternetAddress to, string subject, string body) :
        base(settings, to, subject, body) { }

        public SMTPMail(IDomainSettings settings) :
        base(settings) { }
    }
}
