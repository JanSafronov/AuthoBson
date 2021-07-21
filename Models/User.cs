using System;
using System.Collections;
using System.Collections.Generic;

namespace Models {
    public interface User {
        string username { get; set; }

        string password { get; set; }

        string email { get; set; }

        bool notification { get; set; }
    }

    public class Administrator : User {
        public string username { get; set; }

        public string password { get; set; }

        public string email { get; set; }

        public bool notification { get; set; }

        public void User (string username, string password, string email, bool notification) {
            this.username = username;
            this.password = password;
            this.email = email;
            this.notification = notification;
        }

        public string Suspend (string username, DateTime final, string reason) {
            
        }

        public void Ban (string username, string reason) {
            
        }
    }

    public class Moderator : User {
        public string username { get; set; }

        public string password { get; set; }

        public string email { get; set; }

        public bool notification { get; set; }

        public void User (string username, string password, string email, bool notification) {
            this.username = username;
            this.password = password;
            this.email = email;
            this.notification = notification;
        }

        public 
    }

    public class Senior : User {
        public string username { get; set; }

        public string password { get; set; }

        public string email { get; set; }

        public bool notification { get; set; }

        public void User (string username, string password, string email, bool notification) {
            this.username = username;
            this.password = password;
            this.email = email;
            this.notification = notification;
        }

        public 
    }

    public class GenericUser : User {
        public string username { get; set; }

        public string password { get; set; }

        public string email { get; set; }

        public bool notification { get; set; }

        public void User (string username, string password, string email, bool notification) {
            this.username = username;
            this.password = password;
            this.email = email;
            this.notification = notification;
        }

        public 
    }
}