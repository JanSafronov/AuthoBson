using System;
using System.Collections;
using System.Security;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using AuthoBson.Shared.Services;
using Xunit;

namespace AuthoBson.Test
{
    public class GenericHashTest
    {

        private readonly GenericHash defaulthash = GenericHash.Encode<SHA256>("Password");
        private readonly GenericHash hash = GenericHash.Encode<SHA256>("Password", 6);

        [Fact]
        public void GenericHash_IsSchematic()
        {
            var proof = GenericHash.DefaultSaltSize == 8;

            Assert.True(proof, "GenericHash default salt size should be 8");

            proof = hash.Salt is not null && hash.Passhash is not null;

            Assert.True(proof, "GenericHash salt and passhash should be an array of bytes");

            proof = defaulthash.Salt.Length == 8 && hash.Salt.Length == 6;

            Assert.True(proof, "default encoded generic hash and different sized salted generic hash should be of size 8 and different size respectively");

            Regex pattern = new(@"{'salt':\s'\S+',\s'passhash':\s'\S+'}");

            Assert.Matches(pattern, hash.ToString());
        }

        [Fact]
        public void GenericHash_IsVerifiable()
        {
            Assert.True(GenericHash.Verify<SHA256>("password", hash.Salt, hash.Passhash), "Hashed password should be verified");
            Assert.False(GenericHash.Verify<SHA256>("different", hash.Salt, hash.Passhash), "Different password should not be verified");
        }
    }
}
