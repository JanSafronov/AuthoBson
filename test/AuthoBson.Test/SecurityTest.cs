using System;
using System.Collections;
using System.Security;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using AuthoBson.Services;
using AuthoBson.Services.Security;
using Xunit;

namespace AuthoBson.Test.Services.Security {
    public class GenericHashTest<HA> where HA : HashAlgorithm {
        GenericHash defaulthash = GenericHash.Encode<HA>("password");
        GenericHash hash = GenericHash.Encode<HA>("password", 6);

        [Fact]
        public void GenericHash_IsSchematic() {
            bool proof = GenericHash.DefaultSaltSize == 8;

            Assert.True(proof, "GenericHash default salt size should be 8");

            proof = hash.Salt is byte[] && hash.Passhash is byte[];

            Assert.True(proof, "GenericHash salt and passhash should be an array of bytes");

            Regex pattern = new Regex(@"[{]{2}'salt'[:]\s'\w+', 'passhash': '\w+'$");

            Assert.Matches(pattern, hash.ToString());
        }
        
    }
}