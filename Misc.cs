using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Lavalink;

namespace BotTest.Misc
{
	public class Misc
	{
		internal static bool Playing { get; private set; } = false;

		public static async Task PlaySong(LavalinkGuildConnection conn, CommandContext ctx)
		{
			while (Queue.Count > 0)
            {

            }
		}

		internal static Queue<LavalinkTrack> Queue = new Queue<LavalinkTrack>();
		internal static Dictionary<ulong, Queue<LavalinkTrack>> Queues = new Dictionary<ulong, Queue<LavalinkTrack>>();
		internal static CancellationTokenSource source = new CancellationTokenSource();
	}
}