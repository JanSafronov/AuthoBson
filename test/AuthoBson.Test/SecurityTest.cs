using System;
using System.Collections;
using System.Security;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using AuthoBson.Services;
using AuthoBson.Services.Security;
using Xunit;

namespace AuthoBson.Test.Services.Security {
    public class GenericHashTest {
        GenericHash defaulthash = GenericHash.Encode<SHA256>("password");
        GenericHash hash = GenericHash.Encode<SHA256>("password", 6);

        [Fact]
        public void GenericHash_IsSchematic() {
            bool proof = GenericHash.DefaultSaltSize == 8;

            Assert.True(proof, "GenericHash default salt size should be 8");

            proof = hash.Salt is byte[] && hash.Passhash is byte[];

            Assert.True(proof, "GenericHash salt and passhash should be an array of bytes");

            Regex pattern = new Regex(@"{'salt':\s'\S+',\s'passhash':\s'\S+'}");

            Assert.Matches(pattern, hash.ToString());
        }
        
    }
}