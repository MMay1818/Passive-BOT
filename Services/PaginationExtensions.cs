﻿using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace PassiveBOT.Services
{
    public static class PaginationExtensions
    {
        public static IServiceCollection AddPaginator(this IServiceCollection collection, DiscordSocketClient client)
        {
            collection.AddSingleton(new PaginationService(client));
            return collection;
        }

        public static IServiceCollection AddPaginator(this IServiceCollection collection)
        {
            collection.AddSingleton<PaginationService>();
            return collection;
        }
    }
}