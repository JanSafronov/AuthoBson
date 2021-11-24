using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings;
using System.Text.Encodings.Web;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthoBson.Shared.Services
{
    public class GenericHash
    {
        private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();
        public static readonly int DefaultSaltSize = 8; // 64-bit salt
        public readonly byte[] Salt;
        public readonly byte[] Passhash;

        /// <summary>
        /// Internal GenericHash constructor
        /// </summary>
        /// <param name="salt">Salt property of the instance</param>
        /// <param name="passhash">Passhash property of the instance</param>
        internal GenericHash(byte[] salt, byte[] passhash)
        {
            Salt = salt;
            Passhash = passhash;
        }

        public override string ToString()
        {
            return string.Format("{{'salt': '{0}', 'passhash': '{1}'}}",
                                 Convert.ToBase64String(Salt),
                                 Convert.ToBase64String(Passhash));
        }

        public static GenericHash Encode<HA>(string password) where HA : HashAlgorithm
        {
            return Encode<HA>(password, DefaultSaltSize);
        }

        public static GenericHash Encode<HA>(string password, int saltSize) where HA : HashAlgorithm
        {
            return Encode<HA>(password, GenerateSalt(saltSize));
        }

        public static GenericHash Encode<HA>(string password, byte[] salt) where HA : HashAlgorithm
        {
            var publicStatic = BindingFlags.Public | BindingFlags.Static;
            var hasher_factory = typeof(HA).GetMethod("Create", publicStatic, Type.DefaultBinder, Type.EmptyTypes, null);
            using (var hasher = (HashAlgorithm)hasher_factory.Invoke(null, null))
            {
                using (MemoryStream hashInput = new())
                {
                    hashInput.Write(salt, 0, salt.Length);
                    var passwordBytes = Encoding.UTF8.GetBytes(password);
                    hashInput.Write(passwordBytes, 0, passwordBytes.Length);
                    hashInput.Seek(0, SeekOrigin.Begin);
                    var passhash = hasher.ComputeHash(hashInput);
                    return new GenericHash(salt, passhash);
                }
            }
        }
        /// <summary>
        /// Salt generator
        /// </summary>
        /// <param name="saltSize">Size of the salt to generate</param>
        /// <returns>The salt</returns>
        private static byte[] GenerateSalt(int saltSize)
        {
            var salt = new byte[saltSize];
            rng.GetBytes(salt);
            return salt;
        }

        /// <summary>
        /// Verifies that a tuple (password, salt) generate into a hash of (salt, passhash)
        /// </summary>
        /// <param name="password">Password to encode</param>
        /// <param name="salt">Salt to encode and generate</param>
        /// <param name="passhash">Hashed password to compare</param>
        /// <typeparam name="HA">Hashing Algorithm</typeparam>
        /// <returns>Verification of generated hashes being the same</returns>
        public static bool Verify<HA>(string password, byte[] salt, byte[] passhash) where HA : HashAlgorithm
        {
            return Encode<HA>(password, salt).ToString() == new GenericHash(salt, passhash).ToString();
        }
    }
}
