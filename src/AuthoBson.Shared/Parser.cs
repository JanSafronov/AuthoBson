using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.IO.Compression;


namespace AuthoBson.Shared
{
    public enum Commands { Raw, File }

    public class Parser
    {
        public static string[] infos = new string[] { "path to json \"configs.json\" file as configuration source for the api.",
            "Raw json string as a confguration source for the api." };

        public static string[] ParseasCommands(string[] commands)
        {
            string[] args = new string[1];
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
