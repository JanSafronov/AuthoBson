using System;
using System.Collections;
using System.Collections.Generic;

namespace Models {

    public enum UserT
    {
        Administrator,
        Moderator,
        Senior,
        GenericUser
    }

    public interface User {
        string username { get; set; }

        string password { get; set; }

        string email { get; set; }

        bool notification { get; set; }

        UserT usertype { get; set; }
    }

    public class Administrator : User {
        public string username { get; set; }

        public string password { get; set; }

        public string email { get; set; }

        public bool notification { get; set; }

        public UserT usertype { get; set; }
        

        public Administrator (string username, string password, string email, bool notification) {
            this.username = username;
            this.password = password;
            this.email = email;
            this.notification = notification;
            this.usertype = UserT.Administrator;
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

        public UserT usertype { get; set; }

        public Moderator (string username, string password, string email, bool notification) {
            this.username = username;
            this.password = password;
            this.email = email;
            this.notification = notification;
            this.usertype = UserT.Moderator;
        }

        public 
    }

    public class Senior : User {
        public string username { get; set; }

        public string password { get; set; }

        public string email { get; set; }

        public bool notification { get; set; }

        public UserT usertype { get; set; }

        public Senior (string username, string password, string email, bool notification) {
            this.username = username;
            this.password = password;
            this.email = email;
            this.notification = notification;
            this.usertype = UserT.Senior;
        }

        public 
    }

    public class GenericUser : User {
        public string username { get; set; }

        public string password { get; set; }

        public string email { get; set; }

        public bool notification { get; set; }

        public UserT usertype { get; set; }

        public GenericUser (string username, string password, string email, bool notification) {
            this.username = username;
            this.password = password;
            this.email = email;
            this.notification = notification;
            this.usertype = UserT.GenericUser;
        }
    }
}