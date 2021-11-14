using System.IO;
using Microsoft.Extensions.Configuration;
using AuthoBson.Shared;

namespace AuthoBson.Messaging
{
    public static class ParserExtensions
    {
        public static void ParseConfig(this Parser parser, string[] args, IConfigurationBuilder config)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);


            config.AddJsonFile("configs.json");

            config.AddJsonFile(args[(int)Commands.File]);
            config.AddJsonFile(args[(int)Commands.File] + "\\configs.json");

            writer.Write(args[(int)Commands.Raw]);
            writer.Flush();
            stream.Position = 0;

            config.AddJsonStream(stream);
        }
    }
}
