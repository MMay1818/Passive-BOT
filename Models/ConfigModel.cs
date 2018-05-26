﻿using System;
using System.IO;
using Newtonsoft.Json;
using PassiveBOT.Handlers;

namespace PassiveBOT.Models
{
    public class ConfigModel
    {
        public static string ConfigPath = Path.Combine(AppContext.BaseDirectory, "setup/config.json");

        public string Prefix { get; set; } = ".p ";
        public string Token { get; set; } = "Token";
        public string Debug { get; set; } = "N";
        public string DBName { get; set; } = "PassiveBOT";
        public string DBUrl { get; set; } = "http://127.0.0.1:8080";
        public string SupportServer { get; set; } = "https://discord.me/passive";
        public bool AutoRun { get; set; }

        public void Save(string dir = "setup/config.json")
        {
            var file = Path.Combine(AppContext.BaseDirectory, dir);
            File.WriteAllText(file, ToJson());
        }

        public static ConfigModel Load(string dir = "setup/config.json")
        {
            var file = Path.Combine(AppContext.BaseDirectory, dir);
            return JsonConvert.DeserializeObject<ConfigModel>(File.ReadAllText(file));
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static void CheckExistence()
        {
            bool auto;
            try
            {
                auto = Load().AutoRun;
            }
            catch
            {
                auto = false;
            }

            if (!auto)
            {
                LogHandler.LogMessage("Run (Y for run, N for setup Config)");

                Console.Write("Y or N: ");
                var res = Console.ReadLine();
                if (res == "N" || res == "n")
                    File.Delete("setup/config.json");

                if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "setup/")))
                    Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "setup/"));
            }

            if (!File.Exists(ConfigPath))
            {
                var cfg = new ConfigModel();

                LogHandler.LogMessage(
                    @"Please enter a prefix for the bot eg. '+' (do not include the '' outside of the prefix)");
                Console.Write("Prefix: ");
                cfg.Prefix = Console.ReadLine();

                LogHandler.LogMessage("Would you like to log debug?");
                Console.Write("Yes or No: ");
                var type = Console.ReadLine();
                if (type != null && (type.StartsWith("y") || type.StartsWith("Y")))
                    type = "Y";
                else
                    type = "N";
                cfg.Debug = type;

                LogHandler.LogMessage(
                    @"After you input your token, a config will be generated at 'setup/config.json'");
                Console.Write("Token: ");
                cfg.Token = Console.ReadLine();


                LogHandler.LogMessage("Would you like to AutoRun the bot from now on? Y/N");
                var type2 = Console.ReadLine();
                if (type2 != null && (type2.StartsWith("y") || type2.StartsWith("Y")))
                    cfg.AutoRun = true;
                else
                    cfg.AutoRun = false;

                cfg.Save();
            }

            LogHandler.LogMessage("Config Loaded!");
            LogHandler.LogMessage($"Prefix: {Load().Prefix}");
            LogHandler.LogMessage($"Debug: {Load().Debug}");
            LogHandler.LogMessage($"Token Length: {Load().Token.Length} (should be 59)");
            LogHandler.LogMessage($"Autorun: {Load().AutoRun}");
        }
    }
}