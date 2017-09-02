﻿using System;
using System.Collections.Generic;
using System.IO;
using Discord;
using Newtonsoft.Json;
using PassiveBOT.Commands;

namespace PassiveBOT.Configuration
{
    public class GuildConfig
    {
        [JsonIgnore] public static readonly string Appdir = AppContext.BaseDirectory;


        public ulong GuildId { get; set; } //
        public string GuildName { get; set; } //

        public ulong DjRoleId { get; set; } // restrict the music module to a specific role

        public List<ulong> Roles { get; set; } = new List<ulong>(); // a list of roles that users can join via command

        public string Rss { get; set; } = "0"; // rss feed url
        public ulong RssChannel { get; set; } // channel to post custom rss feeds to

        public List<Tags.Tagging> Dict { get; set; } = new List<Tags.Tagging>(); // tags module

        public List<string> Blacklist { get; set; } = new List<string>(); // keyword blacklist
        public bool Invite { get; set; } = false; // blacklist for discord invites
        public bool MentionAll { get; set; } = false; //blacklist for @everyone and @here 

        public bool ErrorLog { get; set; } // allows for responses with errors 

        public bool GoodbyeEvent { get; set; } = false;
        public string GoodbyeMessage { get; set; } = "Has Left the Server :(";
        public ulong GoodByeChannel { get; set; } = 0;
        public bool WelcomeEvent { get; set; } // toggles welcome messages for new users
        public string WelcomeMessage { get; set; } = "Welcome to Our Server!!!"; // the welcome message
        public ulong WelcomeChannel { get; set; } // welcome messages in a channel
        public bool EventLogging { get; set; } = false;
        public ulong EventChannel { get; set; } = 0;

        public List<Warns> Warnings { get; set; } = new List<Warns>();
        public List<Kicks> Kicking { get; set; } = new List<Kicks>();
        public List<Bans> Banning { get; set; } = new List<Bans>();


        public void Save(ulong id)
        {
            var file = Path.Combine(Appdir, $"setup/server/{id}/config.json");
            File.WriteAllText(file, ToJson());
        }

        public static void SaveServer(GuildConfig config, IGuild guild)
        {
            var file = Path.Combine(Appdir, $"setup/server/{guild.Id}/config.json");
            var output = JsonConvert.SerializeObject(config);
            File.WriteAllText(file, output);
        }

        public static GuildConfig Load(ulong id)
        {
            var file = Path.Combine(Appdir, $"setup/server/{id}/config.json");
            return JsonConvert.DeserializeObject<GuildConfig>(File.ReadAllText(file));
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static void Setup(IGuild guild)
        {
            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, $"setup/server/{guild.Id}")))
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, $"setup/server/{guild.Id}"));

            if (File.Exists(Path.Combine(Appdir, $"setup/server/{guild.Id}/config.json"))) return;
            var cfg = new GuildConfig
            {
                GuildId = guild.Id,
                GuildName = guild.Name
            };

            cfg.Save(guild.Id);
        }

        public static string SetWMessage(ulong id, string input)
        {
            var file = Path.Combine(Appdir, $"setup/server/{id}/config.json");
            if (File.Exists(file))
            {
                var jsonObj = JsonConvert.DeserializeObject<GuildConfig>(File.ReadAllText(file));
                jsonObj.WelcomeMessage = input;
                jsonObj.WelcomeEvent = true;
                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText(file, output);
            }
            else
            {
                return "please run the setup command before using configuration commands";
            }
            return null;
        }

        public static string SetWChannel(ulong id, ulong channel)
        {
            var file = Path.Combine(Appdir, $"setup/server/{id}/config.json");
            if (File.Exists(file))
            {
                var jsonObj = JsonConvert.DeserializeObject<GuildConfig>(File.ReadAllText(file));
                jsonObj.WelcomeChannel = channel;
                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText(file, output);
            }
            else
            {
                return "please run the setup command before using configuration commands";
            }
            return null;
        }

        public static string SetError(ulong id, bool status)
        {
            var file = Path.Combine(Appdir, $"setup/server/{id}/config.json");
            if (File.Exists(file))
            {
                var jsonObj = JsonConvert.DeserializeObject<GuildConfig>(File.ReadAllText(file));
                jsonObj.ErrorLog = status;
                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText(file, output);
            }
            else
            {
                return "please run the setup command before using configuration commands";
            }
            return null;
        }


        public static string SetWelcomeStatus(ulong id, bool status)
        {
            var file = Path.Combine(Appdir, $"setup/server/{id}/config.json");
            if (File.Exists(file))
            {
                var jsonObj = JsonConvert.DeserializeObject<GuildConfig>(File.ReadAllText(file));
                jsonObj.WelcomeEvent = status;
                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText(file, output);
            }
            else
            {
                return "please run the setup command before using configuration commands";
            }
            return null;
        }

        public static string SetDj(ulong id, ulong role)
        {
            var file = Path.Combine(Appdir, $"setup/server/{id}/config.json");
            if (File.Exists(file))
            {
                var jsonObj = JsonConvert.DeserializeObject<GuildConfig>(File.ReadAllText(file));
                jsonObj.DjRoleId = role;
                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText(file, output);
            }
            else
            {
                return "please run the setup command before using configuration commands";
            }
            return null;
        }

        public static string RssSet(ulong id, ulong chan, string url, bool add)
        {
            var file = Path.Combine(Appdir, $"setup/server/{id}/config.json");
            if (File.Exists(file))
            {
                var jsonObj = JsonConvert.DeserializeObject<GuildConfig>(File.ReadAllText(file));

                if (add)
                {
                    jsonObj.Rss = url;
                    jsonObj.RssChannel = chan;
                }
                else
                {
                    jsonObj.Rss = "0";
                }

                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText(file, output);
            }
            else
            {
                return "please run the setup command before using configuration commands";
            }
            return null;
        }

        public class Warns
        {
            public string User { get; set; }
            public string Reason { get; set; }
            public string Moderator { get; set; }
            public ulong UserId { get; set; }
        }

        public class Kicks
        {
            public string User { get; set; }
            public string Reason { get; set; }
            public string Moderator { get; set; }
            public ulong UserId { get; set; }
        }

        public class Bans
        {
            public string User { get; set; }
            public string Reason { get; set; }
            public string Moderator { get; set; }
            public ulong UserId { get; set; }
        }
    }
}