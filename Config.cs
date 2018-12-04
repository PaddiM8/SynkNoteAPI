using SynkNote.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SynkNote
{
    public static class Config
    {
        public static string DatabaseConnectionString { get; }
        public static int TokenLength { get;  }
        public static Env Environment { get; }
        public static int HashIterations { get; }
        public static int SaltLength { get; }

        static Config()
        {
            // Load config
            foreach (var line in File.ReadAllLines("config"))
            {
                string[] parts = line.Split('=');

                if (parts[0] == "databaseConnection")
                {
                    DatabaseConnectionString = parts[1];
                } else if (parts[0] == "tokenLength")
                {
                    if (int.TryParse(parts[1], out int tokenLength))
                        TokenLength = tokenLength;
                } else if (parts[0] == "environment")
                {
                    if (parts[1] == "dev") Environment = Env.Development;
                    else Environment = Env.Production;
                } else if (parts[0] == "hashIterations")
                {
                    HashIterations = int.Parse(parts[1]);
                } else if (parts[0] == "saltLength")
                {
                    SaltLength = int.Parse(parts[1]);
                }
            }
        }
    }
}
