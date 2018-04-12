﻿using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using PassiveBOT.Configuration;
using PassiveBOT.Handlers;
using PassiveBOT.Preconditions;
using PassiveBOT.strings;

namespace PassiveBOT.Commands.ServerSetup
{
    [CheckModerator]
    [RequireContext(ContextType.Guild)]
    public class Partner : ModuleBase
    {
        [Command("PartnerToggle")]
        [Summary("PartnerToggle")]
        [Remarks("Toggle the Partner Channel Service")]
        public async Task PToggle()
        {
            var guild = GuildConfig.GetServer(Context.Guild);
            guild.PartnerSetup.IsPartner = !guild.PartnerSetup.IsPartner;
            GuildConfig.SaveServer(guild);
            await ReplyAsync($"Partner service enabled: {guild.PartnerSetup.IsPartner}");
            if (guild.PartnerSetup.IsPartner)
            {
                if (!TimerService.AcceptedServers.Contains(Context.Guild.Id))
                    TimerService.AcceptedServers.Add(Context.Guild.Id);
            }
            else
            {
                if (TimerService.AcceptedServers.Contains(Context.Guild.Id))
                    TimerService.AcceptedServers.Remove(Context.Guild.Id);
            }

            var home = Homeserver.Load().PartnerUpdates;
            var chan = await Context.Client.GetChannelAsync(home);
            if (chan is IMessageChannel channel)
            {
                var embed = new EmbedBuilder
                {
                    Title = "Partner Toggled",
                    Description = $"{Context.Guild.Name}\n" +
                                  $"`{Context.Guild.Id}`\n" +
                                  $"Status: {guild.PartnerSetup.IsPartner}"
                };
                await channel.SendMessageAsync("", false, embed.Build());
            }
        }

        [Command("PartnerChannel")]
        [Summary("PartnerChannel")]
        [Remarks("Set the Partner Channel")]
        public async Task PChannel()
        {
            var guild = GuildConfig.GetServer(Context.Guild);
            guild.PartnerSetup.PartherChannel = Context.Channel.Id;
            GuildConfig.SaveServer(guild);
            await ReplyAsync($"Partner Channel set to {Context.Channel.Name}");

            var home = Homeserver.Load().PartnerUpdates;
            var chan = await Context.Client.GetChannelAsync(home);
            if (chan is IMessageChannel channel)
            {
                var embed = new EmbedBuilder
                {
                    Title = "Partner Channel Set",
                    Description = $"{Context.Guild.Name}\n" +
                                  $"`{Context.Guild.Id}`\n" +
                                  $"Channel: {Context.Channel.Name}"
                };
                await channel.SendMessageAsync("", false, embed.Build());
            }
        }

        [Command("PartnerHelp")]
        [Summary("PartnerHelp")]
        [Remarks("See the PartnerHelp tutorial")]
        public async Task PHelp()
        {
            var embed = new EmbedBuilder
            {
                Title = "Partner Help Tutorial",
                Description = $"```\r\n" +
                              $"What is the partner program?\r\n" +
                              $"```\r\n" +
                              $"-This is a part of PassiveBOT, different servers may sign up for this by enabling the partner program with the command\r\n" +
                              $"\r\n" +
                              $"```\r\n" +
                              $"Full Tutorial\r\n" +
                              $"```\r\n" +
                              $"`1.` Type `.p invite` to get an invite link to the bot, add it to your server with all permissions\r\n" +
                              $"`2.` Type `.p PartnerToggle` to enable to partner system in your server\r\n" +
                              $"`3.` Type `.p PartnerChannel` in your desired channel to set which channel other Partner messages will be sent to\r\n" +
                              $"`4.` Type `.p PartnerMessage <message>` to set the message you want to be sent to other servers, make sure to include your `discord.gg` or `discord.me` invite link for the server so people can join.\r\n" +
                              $"\r\n" +
                              $"That's it, your message will be sent to a server every hour and your server will receive a message every hour!!\r\n" +
                              $"ENJOY!!!"
            };
            await ReplyAsync("", false, embed.Build());
        }

        [Command("PartnerReport")]
        [Summary("PartnerReport")]
        [Remarks("Report a partner message")]
        public async Task PReport([Remainder] string message = null)
        {
            if (message == null)
                await ReplyAsync(
                    "Please provide some information about the Partner message you are reporting, and we will do our best to remove it");
            else
                try
                {
                    var s = Homeserver.Load().Suggestion;
                    var c = await Context.Client.GetChannelAsync(s);
                    var embed = new EmbedBuilder();
                    embed.AddField($"Partner Message Report from {Context.User.Username}", message);
                    embed.WithFooter(x => { x.Text = $"{Context.Message.CreatedAt} || {Context.Guild.Name}"; });
                    embed.Color = Color.Blue;
                    await ((ITextChannel) c).SendMessageAsync("", false, embed.Build());
                    await ReplyAsync("Report Sent!!");
                }
                catch
                {
                    await ReplyAsync("The bots owner has not yet configured the Reports channel");
                }
        }

        [Command("PartnerMessage")]
        [Summary("PartnerMessage <message>")]
        [Remarks("Set your Servers PertnerMessage")]
        public async Task PMessage([Remainder] string input = null)
        {
            if (input == null)
            {
                await ReplyAsync("Please input a message");
                return;
            }

            if (input.Length > 1024)
            {
                await ReplyAsync($"Message is too long. Please limit it to 1024 characters or less. (Current = {input.Length})");
                return;
            }

            if (NsfwStr.Profanity.Any(x =>
                ProfanityFilter.doreplacements(ProfanityFilter.RemoveDiacritics(input.ToLower())).ToLower()
                    .Contains(x.ToLower())))
            {
                await ReplyAsync("Profanity Detected, unable to set message!");
                return;
            }

            if (Context.Message.MentionedRoleIds.Any() || Context.Message.MentionedUserIds.Any() ||
                Context.Message.MentionedChannelIds.Any() || Context.Message.Content.Contains("@everyone")
                || Context.Message.Content.Contains("@here"))
            {
                await ReplyAsync("There is no need to mention roles, users or channels in the partner " +
                                 "program as it shares to other servers which may not have access" +
                                 "to them!");
                return;
            }

            if (!input.Contains("discord.gg") && !input.Contains("discord.me"))
            {
                await ReplyAsync("You should include an invite link to your server in the Partner Message too!");
                return;
            }

            var guild = GuildConfig.GetServer(Context.Guild);
            guild.PartnerSetup.Message = input;
            GuildConfig.SaveServer(guild);
            var embed = new EmbedBuilder
            {
                Title = Context.Guild.Name,
                Description = input,
                ThumbnailUrl = Context.Guild.IconUrl,
                Color = Color.Green
            };

            await ReplyAsync("Success, here is your Partner Message:", false, embed.Build());

            var home = Homeserver.Load().PartnerUpdates;
            var chan = await Context.Client.GetChannelAsync(home);
            if (chan is IMessageChannel channel)
            {
                var embed2 = new EmbedBuilder
                {
                    Title = "Partner Msg. Updated",
                    Description = $"{Context.Guild.Name}\n" +
                                  $"`{Context.Guild.Id}`\n" +
                                  $"{guild.PartnerSetup.Message}",
                    Footer = new EmbedFooterBuilder
                    {
                        Text = $"{((SocketGuild)Context.Guild).Owner.Username}#{((SocketGuild)Context.Guild).Owner.Discriminator}"
                    }
                };
                await channel.SendMessageAsync("", false, embed2.Build());
            }
        }

        [Command("PartnerInfo")]
        [Summary("PartnerInfo")]
        [Remarks("See Partner Setup Info")]
        public async Task PInfo()
        {
            var embed = new EmbedBuilder();
            var guild = GuildConfig.GetServer(Context.Guild);
            embed.Description =
                $"Channel: {Context.Client.GetChannelAsync(guild.PartnerSetup.PartherChannel).Result?.Name}\n" +
                $"Enabled: {guild.PartnerSetup.IsPartner}\n" +
                $"Banned: {guild.PartnerSetup.banned}\n" +
                $"Message:\n{guild.PartnerSetup.Message}";
            embed.Color = Color.Blue;
            await ReplyAsync("", false, embed.Build());
        }
    }
}