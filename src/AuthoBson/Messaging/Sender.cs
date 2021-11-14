using MimeKit;
using MailKit;
using MailKit.Net;
using MailKit.Net.Smtp;

namespace AuthoBson.Email
{
    public interface ISender
    {
        MailboxAddress Address { get; set; }

        string Password { get; set; }
    }

    public interface ISenderBin : ISender
    {
        string Username { get; set; }
    }

    public class Sender : ISender
    {
        public MailboxAddress Address { get; set; }

        public string Password { get; set; }

        public Sender(MailboxAddress Address, string Password)
        {
            this.Address = Address;
            this.Password = Password;
        }
    }
}
