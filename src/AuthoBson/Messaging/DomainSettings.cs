using System;

namespace AuthoBson.Email.Settings {
    public interface IDomainSettings {
        string Address { get; set; }

        string Username { get; set; }

        string Password { get; set; }
    }

    public class DomainSettings : IDomainSettings {
        public string Address { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
