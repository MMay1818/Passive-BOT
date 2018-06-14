﻿namespace PassiveBOT.Discord.Context
{
    using System;

    using global::Discord.Commands;

    using global::Discord.WebSocket;

    using Microsoft.Extensions.DependencyInjection;

    using PassiveBOT.Handlers;
    using PassiveBOT.Models;

    /// <summary>
    /// The context.
    /// </summary>
    public class Context : ShardedCommandContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="client">
        /// The client param.
        /// </param>
        /// <param name="message">
        /// The Message param.
        /// </param>
        /// <param name="serviceProvider">
        /// The service provider.
        /// </param>
        public Context(DiscordShardedClient client, SocketUserMessage message, IServiceProvider serviceProvider) : base(client, message)
        {
            ShardedClient = client;

            // These are our custom additions to the context, giving access to the server object and all server objects through Context.
            Server = serviceProvider.GetRequiredService<DatabaseHandler>().Execute<GuildModel>(DatabaseHandler.Operation.LOAD, null, Guild.Id);
            Provider = serviceProvider;
            Prefix = Server?.Settings.Prefix.CustomPrefix ?? Provider.GetRequiredService<ConfigModel>().Prefix;
        }

        /// <summary>
        /// Gets the custom server object.
        /// </summary>
        public GuildModel Server { get; }

        /// <summary>
        /// Gets the sharded client.
        /// </summary>
        public DiscordShardedClient ShardedClient { get; }

        /// <summary>
        /// Gets the prefix.
        /// </summary>
        public string Prefix { get; }

        /// <summary>
        /// Gets the provider.
        /// </summary>
        public IServiceProvider Provider { get; }
    }
}