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

		}

		[Command("help")]
		public async Task Help(CommandContext ctx)
		{

		}

		[Command("youalive")]
		public async Task AliveCommand(CommandContext ctx)
		{

		}

		[Command("funny")]
		public async Task Funny(CommandContext ctx)
		{

		}

		[Command("info")]
		public async Task Info(CommandContext ctx)
		{

		}

		[Command("perm")]
		[Hidden]
		public async Task Perm(CommandContext ctx, ulong id)
		{

		}

		[Command("actuallyfuckingdie")]
		[Hidden]
		public async Task ActuallyFuckingDie(CommandContext ctx)
		{

		}

		[Command("kill")]
		[Cooldown(1, 20000.0, CooldownBucketType.Guild)]
		public async Task KillCommand(CommandContext ctx)
		{

		}

		[Command("unkill")]
		[Hidden]
		public async Task Unkill(CommandContext ctx)
		{

		}

		[Command("ping")]
		[Hidden]
		public async Task Ping(CommandContext ctx, ulong channel, ulong role, [RemainingText] string text)
		{

		}

		[Command("reply")]
		[Hidden]
		public async Task Reply(CommandContext ctx, ulong guild, ulong channel, ulong message, [RemainingText] string contents)
		{

		}

		[Command("boom")]
		public async Task Boom(CommandContext ctx)
		{

		}

		[Command("kys")]
		public async Task KYS(CommandContext ctx, int volume = 50)
		{

		}

		[Command("slur")]
		[Hidden]
		public async Task Slur(CommandContext ctx)
		{

		}

		[Command("changefunny")]
		[Hidden]
		public async Task YouFunny(CommandContext ctx, ulong id)
		{

		}

		[Command("delmsg")]
		[Hidden]
		public async Task DelMsg(CommandContext ctx, ulong id)
		{

		}

		[Command("youropinionisshit")]
		[Hidden]
		public async Task Opinion(CommandContext ctx, ulong id)
		{

		}

		[Command("brazil")]
		[Hidden]
		public async Task DC(CommandContext ctx, ulong id)
		{

		}

		[Command("swearcount")]
		public async Task Swear(CommandContext ctx)
		{

		}

		[Command("swearcount")]
		public async Task Swears(CommandContext ctx, ulong id)
		{

		}

		[Command("unswear")]
		[Hidden]
		public async Task UnSwear(CommandContext ctx, ulong id)
		{

		}

		[Command("talk")]
		public async Task Play(CommandContext ctx, [RemainingText] string search)
		{

		}

		[Command("shutup")]
		public async Task Leave(CommandContext ctx)
		{

		}

		[Command("skip")]
		public async Task Skip(CommandContext ctx)
		{

		}

		[Command("queue")]
		public async Task Queue(CommandContext ctx)
		{

		}

		[Command("blowme")]
		public async Task Blow(CommandContext ctx)
		{

		}

		[Command("nick")]
		public async Task Nick(CommandContext ctx, [RemainingText] string name)
		{

		}

		[Command("makebalancedteams")]
		[Hidden]
		public async Task Teams(CommandContext ctx)
		{

		}

		[Command("makebalancedteams")]
		[Hidden]
		public async Task Teams(CommandContext ctx, bool cs)
		{

		}

		private Random random = new Random();
	}
}