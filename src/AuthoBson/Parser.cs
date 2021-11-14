﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.IO.Compression;
using Microsoft.Extensions.Configuration;

namespace AuthoBson
{
    public enum Commands
    {
        File,
        Raw
    }

    public class Parser
    {
        public static string[] infos = new string[] { "path to json \"configs.json\" file as configuration source for the api.", 
            "Raw json string as a confguration source for the api." };

        public static void ParseConfig(string[] args, IConfigurationBuilder config)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            
            config.AddJsonFile("configs.json");

            string[] parsed = ParseasCommands(args);
            config.AddJsonFile(parsed[(int)Commands.File]);
            config.AddJsonFile(parsed[(int)Commands.File] + "\\configs.json");

            writer.Write(parsed[(int)Commands.Raw]);
            writer.Flush();
            stream.Position = 0;

            config.AddJsonStream(stream);
            
        }

        public static string[] ParseasCommands(string[] commands, Stream stream = null)
        {
            string[] args = new string[0];
            for (int i = 0; i < args.Length; i++)
            {
                if (commands[i] == "-j" || commands[i] == "--json-config")
                {
                    args[(int)Commands.File] = commands[i + 1];
                    i += 2;
                    commands[i] = ((int)Commands.File).ToString();
                }

                if (commands[i] == "-r" || commands[i] == "--raw-config")
                {
                    args[(int)Commands.Raw] = commands[i + 1];
                    i += 2;
                    commands[i] = ((int)Commands.Raw).ToString();
                }

                if (commands[i] == "-h" || commands[i] == "--help")
                {
                    Console.WriteLine(infos[int.Parse(commands[i - 2])]);
                }
            }
            return args;
        }
    }
}
