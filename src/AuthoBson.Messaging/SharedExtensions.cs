using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using AuthoBson.Shared;

namespace AuthoBson.Messaging
{
    public static class ParserExtensions
    {
        public static void ParseConfig(this Parser parser, string[] args, IConfigurationBuilder config)
        {
            if (args.Length > 0)
            {
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);

                writer.Write(args[(int)Commands.Raw]);
                writer.Flush();
                stream.Position = 0;

                config.AddJsonStream(stream);
            }

            config.AddJsonFile("configs.json");

            if (args.Length > 1)
            {
                config.AddJsonFile(args[(int)Commands.File]);
                config.AddJsonFile(args[(int)Commands.File] + "\\configs.json");
            }
        }
    }
}
