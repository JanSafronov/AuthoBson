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
        public static string[] infos = new string[] { "Raw json string as a configuration source for the api.", 
            "path to json \"configs.json\" file as configuration source for the api." };

        public static IDictionary<int, string> ParseasCommands(string[] commands)
        {
            Dictionary<int, string> data = new();
            for (int i = 0; i < commands.Length; i++)
            {
                if (commands[i] == "-j" || commands[i] == "--json-config")
                {
                    data.Add((int)Commands.File, commands[i + 1]);
                    commands[i] = ((int)Commands.File).ToString();
                    i += 2;
                    continue;
                }

                if (commands[i] == "-r" || commands[i] == "--raw-config")
                {
                    data.Add((int)Commands.Raw, commands[i + 1]);
                    commands[i] = ((int)Commands.Raw).ToString();
                    i += 2;
                    continue;
                }
            }
            return data;
        }
    }
}
