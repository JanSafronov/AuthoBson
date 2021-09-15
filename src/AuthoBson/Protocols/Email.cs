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


namespace AuthoBson.Protocols {

    public interface Sender {
        MailboxAddress address { get; set; }

        string password { get; set; }
    }

    public interface SenderBin : Sender {
        string username { get; set; }
    }

    public class Mail {

        public MimeMessage message { get; set; }

        public Sender sender { get; set; }

        public Sender recipient { get; set; }
        
        public void Send() {
            SmtpClient client = new SmtpClient();

            client.Connect("smtp.gmail.com", 465, true);

            client.Authenticate(new SaslMechanismScramSha256(sender.address.Name, sender.password));

            client.Send(message);
        }
    }
}