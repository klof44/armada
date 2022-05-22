using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Lavalink;
using System.Linq;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace armada
{
	public class MusicPlayer
	{
		internal MusicPlayer(ulong guild, CommandContext ctx)
		{
			node = ctx.Client.GetLavalink().ConnectedNodes.FirstOrDefault().Value;
			token = new CancellationTokenSource();
		}
		internal MusicPlayer(ulong guild, InteractionContext ctx)
		{
			node = ctx.Client.GetLavalink().ConnectedNodes.FirstOrDefault().Value;
			token = new CancellationTokenSource();
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

				try
				{
					musicPlayers[ctx.Guild.Id].musicQueue.Enqueue(track);
				}
				catch { }

				DiscordEmbedBuilder embed = new()
				{
					Color = DiscordColor.HotPink,
					Title = "Added to Queue",
				};
				embed.AddField($"{track.Title} - {track.Author}", track.Length.ToString());
				await ctx.RespondAsync(embed);

				if (!musicPlayers[ctx.Guild.Id].musicPlaying)
				{
					await musicPlayers[ctx.Guild.Id].Start(ctx);
				}
			}
			else
			{
				musicPlayers.Add(ctx.Guild.Id, new MusicPlayer(ctx.Guild.Id, ctx));

				// I can't figure out why but putting a try catch here fixes the System.Private.CoreLib.dll issue
				try
				{
					musicPlayers[ctx.Guild.Id].musicQueue.Enqueue(track);
				}
				catch { }

				DiscordEmbedBuilder embed = new()
				{
					Color = DiscordColor.HotPink,
					Title = "Added to Queue",
				};
				embed.AddField($"{track.Title} - {track.Author}", track.Length.ToString());
				await ctx.RespondAsync(embed);

				await musicPlayers[ctx.Guild.Id].Start(ctx);
			}
		}

		public static async Task PlayPlaylist(List<LavalinkTrack> playlist, CommandContext ctx)
		{
			if (musicPlayers.ContainsKey(ctx.Guild.Id))
			{

				foreach (var track in playlist)
				{
					try
					{
						musicPlayers[ctx.Guild.Id].musicQueue.Enqueue(track);
					}
					catch { }
				}

				DiscordEmbedBuilder embed = new()
				{
					Color = DiscordColor.HotPink,
					Title = "Added Playlist to Queue",
				};
				embed.AddField($"Queued {playlist.Count} tracks", "Use !queue to view queue");
				await ctx.RespondAsync(embed);

				if (!musicPlayers[ctx.Guild.Id].musicPlaying)
				{
					await musicPlayers[ctx.Guild.Id].Start(ctx);
				}
			}
			else
			{
				musicPlayers.Add(ctx.Guild.Id, new MusicPlayer(ctx.Guild.Id, ctx));

				foreach (var track in playlist)
				{
					try
					{
						musicPlayers[ctx.Guild.Id].musicQueue.Enqueue(track);
					}
					catch { }
				}

				DiscordEmbedBuilder embed = new()
				{
					Color = DiscordColor.HotPink,
					Title = "Added Playlist to Queue",
				};
				embed.AddField($"Queued {playlist.Count} tracks", "Use !queue to view queue");
				await ctx.RespondAsync(embed);

				await musicPlayers[ctx.Guild.Id].Start(ctx);
			}
		}

		private async Task Start(CommandContext ctx)
		{
			if (!musicPlaying)
			{
				GuildConnection = node.ConnectAsync(ctx.Member.VoiceState.Channel).Result;
			}
			
			while (musicQueue.Count > 0)
			{
				var CurrentTrack = musicQueue.Dequeue();
				await GuildConnection.SetVolumeAsync(50);
				await GuildConnection.PlayAsync(CurrentTrack);
				musicPlaying = true;

				DiscordEmbedBuilder embed = new()
				{
					Color = DiscordColor.HotPink,
					Title = "Now Playing",
				};
				embed.AddField($"{CurrentTrack.Title} - {CurrentTrack.Author}", CurrentTrack.Length.ToString());
				await ctx.RespondAsync(embed);

				this.token = new CancellationTokenSource();

				try 
				{
					await Task.Delay(CurrentTrack.Length + TimeSpan.FromSeconds(3), token.Token);
				} catch { }
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


		
		public static async Task Skip(CommandContext ctx)
		{
			if (musicPlayers.ContainsKey(ctx.Guild.Id))
			{
				musicPlayers[ctx.Guild.Id].token.Cancel();
				DiscordEmbedBuilder embed = new()
				{
					Color = DiscordColor.HotPink,
					Title = "Song skipped",
				};
				await ctx.RespondAsync(embed);
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
				if (musicPlayers[ctx.Guild.Id].musicQueue.Count <= 0)
				{
					embed.AddField("No tracks in queue", "Add some with `!play`");
				}
				foreach (var track in musicPlayers[ctx.Guild.Id].musicQueue)
				{
					embed.AddField($"{track.Title} - {track.Author}", track.Length.ToString());
				}
				await ctx.RespondAsync(embed);
			}
		}

		// InteractionContext versions for slash commands

		public static async Task Play(LavalinkTrack track, InteractionContext ctx)
		{
			if (musicPlayers.ContainsKey(ctx.Guild.Id))
				{
				try
				{
					musicPlayers[ctx.Guild.Id].musicQueue.Enqueue(track);
				}
				catch { }
				
				DiscordEmbedBuilder embed = new()
				{
					Color = DiscordColor.HotPink,
					Title = "Added to Queue",
				};
				embed.AddField($"{track.Title} - {track.Author}", track.Length.ToString());
				await ctx.CreateResponseAsync(embed);

				if (!musicPlayers[ctx.Guild.Id].musicPlaying)
				{
					await musicPlayers[ctx.Guild.Id].Start(ctx);
				}
			}
			else
			{
				musicPlayers.Add(ctx.Guild.Id, new MusicPlayer(ctx.Guild.Id, ctx));

				// I can't figure out why but putting a try catch here fixes the System.Private.CoreLib.dll issue
				try
				{
					musicPlayers[ctx.Guild.Id].musicQueue.Enqueue(track);
				}
				catch { }

				DiscordEmbedBuilder embed = new()
				{
					Color = DiscordColor.HotPink,
					Title = "Added to Queue",
				};
				embed.AddField($"{track.Title} - {track.Author}", track.Length.ToString());
				await ctx.CreateResponseAsync(embed);


				await musicPlayers[ctx.Guild.Id].Start(ctx);
			}
		}

		public static async Task PlayPlaylist(List<LavalinkTrack> playlist, InteractionContext ctx)
		{
			if (musicPlayers.ContainsKey(ctx.Guild.Id))
			{

				foreach (var track in playlist)
				{
					try
					{
						musicPlayers[ctx.Guild.Id].musicQueue.Enqueue(track);
					}	catch { }
				}

				DiscordEmbedBuilder embed = new()
				{
					Color = DiscordColor.HotPink,
					Title = "Added Playlist to Queue",
				};
				embed.AddField($"Queued {playlist.Count} tracks", "Use !queue to view queue");
				await ctx.CreateResponseAsync(embed);

				if (!musicPlayers[ctx.Guild.Id].musicPlaying)
				{
					await musicPlayers[ctx.Guild.Id].Start(ctx);
				}
			}
			else
			{
				musicPlayers.Add(ctx.Guild.Id, new MusicPlayer(ctx.Guild.Id, ctx));

				foreach (var track in playlist)
				{
					try
					{
						musicPlayers[ctx.Guild.Id].musicQueue.Enqueue(track);
					}
					catch { }
				}

				DiscordEmbedBuilder embed = new()
				{
					Color = DiscordColor.HotPink,
					Title = "Added Playlist to Queue",
				};
				embed.AddField($"Queued {playlist.Count} tracks", "Use !queue to view queue");
				await ctx.CreateResponseAsync(embed);

				await musicPlayers[ctx.Guild.Id].Start(ctx);
			}
		}

		private async Task Start(InteractionContext ctx)
		{
			if (!musicPlaying)
			{
				GuildConnection = node.ConnectAsync(ctx.Member.VoiceState.Channel).Result;
			}
			
			while (musicQueue.Count > 0)
			{
				var CurrentTrack = musicQueue.Dequeue();
				await GuildConnection.SetVolumeAsync(50);
				await GuildConnection.PlayAsync(CurrentTrack);
				musicPlaying = true;

				DiscordEmbedBuilder embed = new()
				{
					Color = DiscordColor.HotPink,
					Title = "Now Playing",
				};
				embed.AddField($"{CurrentTrack.Title} - {CurrentTrack.Author}", CurrentTrack.Length.ToString());
				await ctx.Channel.SendMessageAsync(embed);

				this.token = new CancellationTokenSource();

				try
				{
					await Task.Delay(CurrentTrack.Length + TimeSpan.FromSeconds(1), token.Token);
				}	catch { }

			}
			musicPlaying = false;

			await Stop(ctx);
		}

		public static async Task Stop(InteractionContext ctx)
		{
			if (musicPlayers.ContainsKey(ctx.Guild.Id))
			{
				musicPlayers[ctx.Guild.Id].token.Cancel();
				musicPlayers[ctx.Guild.Id].musicQueue.Clear();
				musicPlayers[ctx.Guild.Id].musicPlaying = false;
				await musicPlayers[ctx.Guild.Id].GuildConnection.DisconnectAsync();
				musicPlayers.Remove(ctx.Guild.Id);

				DiscordEmbedBuilder embed = new()
				{
					Color = DiscordColor.HotPink,
					Title = "Stopped",
				};
				await ctx.CreateResponseAsync(embed);
			}
		}

		public static async Task AddToQueue(LavalinkTrack track, InteractionContext ctx)
		{
			musicPlayers[ctx.Guild.Id].musicQueue.Enqueue(track);
			await Play(track, ctx);
		}

		public static async Task Skip(InteractionContext ctx)
		{
			if (musicPlayers.ContainsKey(ctx.Guild.Id))
			{
				musicPlayers[ctx.Guild.Id].token.Cancel();
				DiscordEmbedBuilder embed = new()
				{
					Color = DiscordColor.HotPink,
					Title = "Song skipped",
				};
				await ctx.CreateResponseAsync(embed);
			}
		}

		public static async Task SayQueue(InteractionContext ctx)
		{
			if (musicPlayers.ContainsKey(ctx.Guild.Id))
			{
				DiscordEmbedBuilder embed = new()
				{
					Color = DiscordColor.HotPink,
					Title = "Queue",
				};
				if (musicPlayers[ctx.Guild.Id].musicQueue.Count <= 0)
				{
					embed.AddField("No tracks in queue", "Add some with `!play`");
				}
				foreach (var track in musicPlayers[ctx.Guild.Id].musicQueue)
				{
					embed.AddField($"{track.Title} - {track.Author}", track.Length.ToString());
				}
				await ctx.CreateResponseAsync(embed);
			}
		}
	}
}