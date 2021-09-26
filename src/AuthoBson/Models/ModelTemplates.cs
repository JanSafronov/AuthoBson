using System;

namespace AuthoBson.Models.Templates {

    public interface IUserTemplate { 
        int[] Username { get; set; }

        int[] Password { get; set; }

        int[] Email { get; set; }

        int[] Verified { get; set; }

        int[] Reason { get; set; }

        bool Scheme(int[] Username, int[] Password, int[] Email, int[] Verified, int[] Reason);
    }
    
    public class UserTemplate : IUserTemplate {
        public int[] Username { get; set; }

        public int[] Password { get; set; }

        public int[] Email { get; set; }

        public int[] Verified { get; set; }

        public int[] Reason { get; set; }

        public bool Scheme(int[] Username, int[] Password, int[] Email, int[] Verified, int[] Reason) { 
            return Username.Length == 2 && Password.Length == 2 && Email.Length == 2 && Verified.Length == 2 && Reason.Length == 2
                   && Username[0] < Username[1] && Password[0] < Password[1] && Email[0] < Email[1] && Verified[0] < Verified[1] && Reason[0] < Reason[1];
        }

        public UserTemplate(int[] Username, int[] Password, int[] Email, int[] Verified, int[] Reason) {
            if (Scheme(Username, Password, Email, Verified, Reason)) {
                this.Username = Username;
                this.Password = Password;
                this.Email = Email;
                this.Verified = Verified;
                this.Reason = Reason;
            }
        }
    }
}