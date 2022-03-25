using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Entities;

namespace armada
{
    internal class Commands : BaseCommandModule
    {
		[Command("roll")]
		[Hidden]
		public async Task Roll(CommandContext ctx, int count, int sides, int mod = 0)
		{
			// dice roll command
		}

		[Command("help")]
		public async Task Help(CommandContext ctx)
		{
			// basic help command
		}

		[Command("funny")]
		public async Task Funny(CommandContext ctx)
		{
			// posts random meme
		}

		[Command("info")]
		public async Task Info(CommandContext ctx)
		{
			// more detailed help command
		}

		[Command("perm")]
		[Hidden]
		public async Task Perm(CommandContext ctx, ulong id)
		{
			// change users who can !kill
		}

		[Command("actuallyfuckingdie")]
		[Hidden]
		public async Task ActuallyFuckingDie(CommandContext ctx)
		{
			// fully disconnects bot from discord
		}

		[Command("kill")]
		[Cooldown(1, 20000.0, CooldownBucketType.Guild)]
		public async Task KillCommand(CommandContext ctx)
		{
			// Makes the bot ignore a guild
		}

		[Command("unkill")]
		[Hidden]
		public async Task Unkill(CommandContext ctx)
		{
			// Makes the bot no longer ignore a guild
		}

		[Command("boom")]
		public async Task Boom(CommandContext ctx)
		{
			// plays vine boom in voice chat
		}

		[Command("kys")]
		public async Task KYS(CommandContext ctx, int volume = 50)
		{
			// plays LTG quote in voice chat
		}

		[Command("changefunny")]
		[Hidden]
		public async Task YouFunny(CommandContext ctx, ulong id)
		{
			// changes a users persission to use voice chat commands
		}

		[Command("delmsg")]
		[Hidden]
		public async Task DelMsg(CommandContext ctx, ulong id)
		{
			// deletes the specified message
		}

		[Command("youropinionisshit")]
		[Hidden]
		public async Task Opinion(CommandContext ctx, ulong id)
		{
			// server mute / unmutes a user
		}

		[Command("swearcount")]
		public async Task Swear(CommandContext ctx)
		{
			// Displays swear counter leaderboard
		}

		[Command("swearcount")]
		public async Task Swears(CommandContext ctx, ulong id)
		{
			// gives swear count of a specific user along with the ratio of messages with swears to messages without
		}

		[Command("unswear")]
		[Hidden]
		public async Task UnSwear(CommandContext ctx, ulong id)
		{
			// remove a swear from the specified user
		}

		[Command("talk")]
		public async Task Play(CommandContext ctx, [RemainingText] string search)
		{
			// plays music in voice chat from either search term or youtube link
		}

		[Command("shutup")]
		public async Task Leave(CommandContext ctx)
		{
			// disconnect from voice chat and clear queue
		}

		[Command("skip")]
		public async Task Skip(CommandContext ctx)
		{
			// skip current song
		}

		[Command("queue")]
		public async Task Queue(CommandContext ctx)
		{
			// displays the queue
		}

		[Command("nick")]
		public async Task Nick(CommandContext ctx, [RemainingText] string name)
		{
			// set bot nickname
		}

		[Command("makebalancedteams")]
		[Hidden]
		public async Task Teams(CommandContext ctx)
		{
			// groups users that react into 2 random teams
		}

		private Random random = new Random();
	}
}