using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Lavalink;

namespace armada
{
	public class Misc
	{
		internal static bool Playing { get; private set; } = false;

		public static async Task PlaySong(LavalinkGuildConnection conn, CommandContext ctx)
		{

		}

		internal static Dictionary<ulong, Queue<LavalinkTrack>> Queues = new Dictionary<ulong, Queue<LavalinkTrack>>();
	}
}