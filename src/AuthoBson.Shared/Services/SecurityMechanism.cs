using System;
using System.Security;
using System.Security.Cryptography;
using AuthoBson.Shared.Data.Models;

namespace AuthoBson.Shared.Services
{
    public class SecurityMechanism<M, HA> where M : ModelBase where HA : HashAlgorithm
    {
        public void HashCredential(M M, string passProperty, string saltProperty)
        {
            var password = typeof(M).GetProperty(passProperty).GetValue(M) as string;
            var hash = GenericHash.Encode<HA>(password, 8);

            typeof(M).GetProperty(passProperty).SetValue(M, Convert.ToBase64String(hash.Passhash));
            typeof(M).GetProperty(saltProperty).SetValue(M, Convert.ToBase64String(hash.Salt));
        }

        public bool VerifyCredential(M M, string password, string passProperty, string saltProperty)
        {
            var M_password = typeof(M).GetProperty(passProperty).GetValue(M) as string;
            var M_salt = typeof(M).GetProperty(saltProperty).GetValue(M) as string;

            return GenericHash.Verify<HA>(password, Convert.FromBase64String(M_salt), Convert.FromBase64String(M_password));
        }
    }
}
