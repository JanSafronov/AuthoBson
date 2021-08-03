using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.IO.Compression;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings;
using System.Text.Encodings.Web;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using AuthoBson.Models;

namespace AuthoBson.Services.Security {
    public class GenericHash {
        private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();
        public static readonly int DefaultSaltSize = 8; // 64-bit salt
        public readonly byte[] Salt;
        public readonly byte[] Passhash;

        internal GenericHash(byte[] salt, byte[] passhash) {
            Salt = salt;
            Passhash = passhash;
        }

        public override String ToString() {
            return String.Format("{{'salt': '{0}', 'passhash': '{1}'}}",
                                 Convert.ToBase64String(Salt),
                                 Convert.ToBase64String(Passhash));
        }

        public static GenericHash Encode<HA>(String password) where HA : HashAlgorithm {
            return Encode<HA>(password, DefaultSaltSize);
        }

        public static GenericHash Encode<HA>(String password, int saltSize) where HA : HashAlgorithm {
            return Encode<HA>(password, GenerateSalt(saltSize));
        }

        private static GenericHash Encode<HA>(string password, byte[] salt) where HA : HashAlgorithm {
            BindingFlags publicStatic = BindingFlags.Public | BindingFlags.Static;
            MethodInfo hasher_factory = typeof (HA).GetMethod("Create", publicStatic, Type.DefaultBinder, Type.EmptyTypes, null);
            using (HashAlgorithm hasher = (HashAlgorithm) hasher_factory.Invoke(null, null))
            {
                using (MemoryStream hashInput = new MemoryStream())
                {
                    hashInput.Write(salt, 0, salt.Length);
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                    hashInput.Write(passwordBytes, 0, passwordBytes.Length);
                    hashInput.Seek(0, SeekOrigin.Begin);
                    byte[] passhash = hasher.ComputeHash(hashInput);
                    return new GenericHash(salt, passhash);
                }
            }
        }

        private static byte[] GenerateSalt(int saltSize) {
            // This generates salt.
            // Rephrasing Schneier:
            // "salt" is a random string of bytes that is
            // combined with password bytes before being
            // operated by the one-way function.
            byte[] salt = new byte[saltSize];
            rng.GetBytes(salt);
            return salt;
        }

        public static bool Verify<HA>(string password, byte[] salt, byte[] passhash) where HA : HashAlgorithm {
            // OMG: I don't know how to compare byte arrays in C#.
            return Encode<HA>(password, salt).ToString() == new GenericHash(salt, passhash).ToString();
        }
    }
}