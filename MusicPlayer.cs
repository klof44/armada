using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Lavalink;
using System.Linq;
using DSharpPlus.Entities;

namespace armada
{
	public class MusicPlayer
	{
		internal MusicPlayer(ulong guild, CommandContext ctx)
		{
			node = ctx.Client.GetLavalink().ConnectedNodes.First().Value;
			token = new CancellationTokenSource();
			musicPlayers.Add(guild, this);
		}

		internal static Dictionary<ulong, MusicPlayer> musicPlayers = new Dictionary<ulong, MusicPlayer>();

		bool musicPlaying = false;
		Queue<LavalinkTrack> musicQueue = new Queue<LavalinkTrack>();
		LavalinkNodeConnection node;
		LavalinkGuildConnection GuildConnection;
		internal CancellationTokenSource token;
		
		public static async Task Play(LavalinkTrack track, CommandContext ctx)
		{
			if (musicPlayers.ContainsKey(ctx.Guild.Id))
			{
				musicPlayers[ctx.Guild.Id].musicQueue.Enqueue(track);
				
				DiscordEmbedBuilder embed = new()
				{
					Color = DiscordColor.HotPink,
					Title = "Added to Queue",
				};
				embed.AddField($"{track.Title} - {track.Author}", track.Length.ToString());
				await ctx.RespondAsync(embed);

				if (!musicPlayers[ctx.Guild.Id].musicPlaying)
				{
					await musicPlayers[ctx.Guild.Id].Start(track, ctx);
				}
			}
			else
			{
				musicPlayers.Add(ctx.Guild.Id, new MusicPlayer(ctx.Guild.Id, ctx));

				musicPlayers[ctx.Guild.Id].musicQueue.Enqueue(track);

				DiscordEmbedBuilder embed = new()
				{
					Color = DiscordColor.HotPink,
					Title = "Added to Queue",
				};
				embed.AddField($"{track.Title} - {track.Author}", track.Length.ToString());
				await ctx.RespondAsync(embed);


				await musicPlayers[ctx.Guild.Id].Start(track, ctx);
			}
		}

		private async Task Start(LavalinkTrack track, CommandContext ctx)
		{
			if (!musicPlaying)
			{
				GuildConnection = node.ConnectAsync(ctx.Member.VoiceState.Channel).Result;
			}
			
			while (musicQueue.Count > 0)
			{
				var CurrentTrack = musicQueue.Dequeue();
				await GuildConnection.PlayAsync(CurrentTrack);
				musicPlaying = true;

				DiscordEmbedBuilder embed = new()
				{
					Color = DiscordColor.HotPink,
					Title = "Now Playing",
				};
				embed.AddField($"{CurrentTrack.Title} - {CurrentTrack.Author}", CurrentTrack.Length.ToString());
				await ctx.RespondAsync(embed);

				await Task.Delay(CurrentTrack.Length + TimeSpan.FromSeconds(3), token.Token);
			}
			musicPlaying = false;

			await Stop(ctx);
		}

		public static async Task Stop(CommandContext ctx)
		{
			if (musicPlayers.ContainsKey(ctx.Guild.Id))
			{
				musicPlayers[ctx.Guild.Id].token.Cancel();
				musicPlayers[ctx.Guild.Id].musicQueue.Clear();
				musicPlayers[ctx.Guild.Id].musicPlaying = false;
				await musicPlayers[ctx.Guild.Id].GuildConnection.DisconnectAsync();
				musicPlayers.Remove(ctx.Guild.Id);
			}
		}

		public static async Task AddToQueue(LavalinkTrack track, CommandContext ctx)
		{
			musicPlayers[ctx.Guild.Id].musicQueue.Enqueue(track);
			await Play(track, ctx);
		}

		public static async Task Skip(CommandContext ctx)
		{
			if (musicPlayers.ContainsKey(ctx.Guild.Id))
			{
				musicPlayers[ctx.Guild.Id].token.Cancel();
			}
		}

		public static async Task SayQueue(CommandContext ctx)
		{
			if (musicPlayers.ContainsKey(ctx.Guild.Id))
			{
				DiscordEmbedBuilder embed = new()
				{
					Color = DiscordColor.HotPink,
					Title = "Queue",
				};
				foreach (var track in musicPlayers[ctx.Guild.Id].musicQueue)
				{
					embed.AddField($"{track.Title} - {track.Author}", track.Length.ToString());
				}
				await ctx.RespondAsync(embed);
			}
		}
	}
}