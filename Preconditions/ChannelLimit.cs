﻿using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace PassiveBOT
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class NSFWchat : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider prov)
        {
            if (context.Channel.Name == "nsfw" || context.Channel.Name.StartsWith("nsfw-") || context.Channel is Discord.IDMChannel)
                return Task.FromResult(PreconditionResult.FromSuccess());
            else
                return Task.FromResult(PreconditionResult.FromError("Command does not function in channels without the title #nsfw"));
        }
    }
}