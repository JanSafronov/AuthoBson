using System;
using System.Collections;
using System.Threading;
using System.Threading.Channels;
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
using AuthoBson.Protocols.Settings;


namespace AuthoBson.Protocols {

    public interface ISender {
        MailboxAddress Address { get; set; }

        string Password { get; set; }
    }

    public interface ISenderBin : ISender {
        string Username { get; set; }
    }

    public class Sender : ISender {
        public MailboxAddress Address { get; set; }

        public string Password { get; set; }

        public Sender(MailboxAddress Address, string Password) {
            this.Address = Address;
            this.Password = Password;
        }
    }

    public class Mail {

        public MimeMessage Message { get; set; }

        public string Password { get; set; }

        public Mail(IDomainSettings settings, InternetAddress to, string subject, string body) {
            this.Message = new MimeMessage(settings.Address, to, subject, body);
            this.Password = settings.Password;
        }
        
        public void Send() {
            SmtpClient client = new SmtpClient();

            client.Connect("smtp.gmail.com", 465, true);

            client.Authenticate(new SaslMechanismScramSha256(Message.Sender.Address, Password));

            client.Send(Message);
        }
    }
}