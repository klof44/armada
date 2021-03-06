using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System.Linq;
using System.IO;
using DSharpPlus.SlashCommands;
using DSharpPlus.Lavalink;
using System.Collections.Generic;

namespace armada
{
    internal class SlashCommands : ApplicationCommandModule
	{
		[SlashCommand("help", "Base help command for non-slash commands")]
		public async Task Help(InteractionContext ctx)
		{
			//basic help command
			var footer = new DiscordEmbedBuilder.EmbedFooter()
			{
				Text = "Bot by klof44",
				IconUrl = ctx.Client.GetUserAsync(563891145256468481).Result.AvatarUrl
			};
				
			DiscordEmbedBuilder embed = new()
			{
				Color = DiscordColor.HotPink,
				Title = "Help",
				Footer = footer,
			};

			string CommandList = "";
			foreach (var cmd in ctx.Client.GetCommandsNext().RegisteredCommands)
			{
				if (!cmd.Value.IsHidden)
				{
					CommandList += $" `{cmd.Key}` ";
				}
			}

			embed.AddField("Commands", CommandList);
			embed.AddField("Huh?", "For more help contact <@563891145256468481>");

			await ctx.CreateResponseAsync(embed);
		}

		[SlashCommand("info", "help but more info")]
		public async Task Info(InteractionContext ctx)
		{
			// Help command but more info
            DiscordEmbedBuilder embed = new()
            {
                Color = DiscordColor.HotPink,
                Title = "Info",
            	Footer = new DiscordEmbedBuilder.EmbedFooter()
                {
                    Text = "Bot by klof44",
                    IconUrl = ctx.Client.GetUserAsync(563891145256468481).Result.AvatarUrl
                }
			};

				embed.AddField("non-slash command count", ctx.Client.GetCommandsNext().RegisteredCommands.Count.ToString());
                embed.AddField("Guilds", $"In {ctx.Client.Guilds.Count} guilds");
				embed.AddField("Memes", Directory.GetFiles(Program.assetsDir + "/bot/funny").Length.ToString());
                embed.AddField("Ping", ctx.Client.Ping.ToString() + "ms");

				await ctx.CreateResponseAsync(embed);
		}

		[SlashCommand("swearcount", "Shows swear leaderboard or swearcount of a specific user")]
		public async Task Swears(InteractionContext ctx, [Option("User", "Who's stats you want to see")] DiscordUser user = null)
		{
			// Get either the swear leaderboard or a user's swear count and ratio
			if (user != null)
			{
				ulong id = user.Id;

				DiscordEmbedBuilder embed = new()
				{
					Color = DiscordColor.HotPink
				};

				if (Program.leaderboard.ContainsKey(id))
				{
					embed.AddField("Swear count", Program.leaderboard[id].ToString());
					embed.AddField("Ratio", $"{Program.leaderboard[id]}/{Program.ratios[id]} ({Program.leaderboard[id] / Program.ratios[id] * 100}%)");
				}
				else
				{
					embed.AddField("Error", $"No data for <@{id}>\r\nSpeaking in any server with armada will automatically add them to the leaderboard");
				}

				await ctx.CreateResponseAsync(embed);	
			}
			else
			{
				DiscordEmbedBuilder embed = new()
				{
					Color = DiscordColor.HotPink
				};
				embed.AddField("Total", Program.swearCount.ToString());

				string board = "";

				var sorted = Program.leaderboard.OrderByDescending(key => key.Value);
				foreach (var users in sorted)
				{
					if (users.Value != 0)
					{
						board += $"\r\n<@{users.Key}> - {users.Value}";
					}
				}

				if (board == "")
				{
					board = "Nothing to display :(";
				}

				embed.AddField("Leaderboard", board);

				await ctx.CreateResponseAsync(embed);
			}
		}
		
		[SlashCommand("funny", "funny command")]
		public async Task Funny(InteractionContext ctx)
		{
			// posts random meme from Program.assetsDir + "/bot/funny"
			string[] files = Directory.GetFiles(Program.assetsDir + "/bot/funny");
				
			var path = files[random.Next(0, files.Length)];
			while (!Program.ValidMeme(path))
			{
				path = files[random.Next(0, files.Length)];
			}

			var meme = new FileStream(path, FileMode.Open, FileAccess.Read);
			var message = new DiscordInteractionResponseBuilder().AddFile(meme);
			await ctx.CreateResponseAsync(message);
		}
		private static Random random = new Random();

		[SlashCommand("talk", "Music command")]
		public async Task Play(InteractionContext ctx, [Option("search", "The name or url of the song")] string search)
		{
			// Uses Lavalink to play a song from youtube
			LavalinkLoadResult track;
			if (Uri.TryCreate(search, UriKind.Absolute, out Uri uri))
			{
				track = ctx.Client.GetLavalink().ConnectedNodes.Values.First().Rest.GetTracksAsync(uri).Result;
				if (track.LoadResultType == LavalinkLoadResultType.PlaylistLoaded)
				{
					List<LavalinkTrack> playlist = new();
					foreach (var tracks in track.Tracks)
					{
						playlist.Add(tracks);
					}

					await MusicPlayer.PlayPlaylist(playlist, ctx);
					return;
				}
			}
			else
			{
				track = ctx.Client.GetLavalink().ConnectedNodes.Values.First().Rest.GetTracksAsync(search).Result;
			}


			await MusicPlayer.Play(track.Tracks.First(), ctx);
		}

		[SlashCommand("shutup", "disconects from voice channel")]
		public async Task Leave(InteractionContext ctx)
		{
			// Disconnect from voice chat and clear queue
			await ctx.Client.GetLavalink().GetGuildConnection(ctx.Guild).DisconnectAsync();
			await MusicPlayer.Stop(ctx);
		}

		[SlashCommand("skip", "skip current track")]
		public async Task Skip(InteractionContext ctx)
		{
			// Skip current song
			await MusicPlayer.Skip(ctx);
		}

		[SlashCommand("queue", "show queue")]
		public async Task Queue(InteractionContext ctx)
		{
			// Displays the queue
			await MusicPlayer.SayQueue(ctx);
		}

		[SlashCommand("Shuffle", "Shuffles the queue")]
		public async Task Shuffle(InteractionContext ctx)
		{
			// Shuffles the queue
			await MusicPlayer.Shuffle(ctx);
		}

		[SlashCommand("vol", "Adjust music volume")]
		public async Task Volume(InteractionContext ctx, [Option("volume", "The desired volume level")] long volume)
		{
			// Changes volume of music

			if (ctx.Member.VoiceState == null)
			{
				DiscordEmbedBuilder embedDeny = new()
				{
					Color = DiscordColor.HotPink,
					Title = "You can't do that"
				};
				
				await ctx.CreateResponseAsync(embedDeny);
				return;
			}

			DiscordEmbedBuilder embed = new()
			{
				Color = DiscordColor.HotPink
			};

			if (volume > 100)
			{
				if (ctx.Member.Id == 563891145256468481)
				{
					await ctx.Client.GetLavalink().GetGuildConnection(ctx.Guild).SetVolumeAsync(Convert.ToInt32(volume));
					embed.Title = $"Volume set to {volume}%";
				}
				else
				{
					await ctx.Client.GetLavalink().GetGuildConnection(ctx.Guild).SetVolumeAsync(100);
					embed.Title = $"Volume set to 100% becasue {volume} is too high";
				}
			}
			else
			{
				await ctx.Client.GetLavalink().GetGuildConnection(ctx.Guild).SetVolumeAsync(Convert.ToInt32(volume));
				embed.Title = $"Volume set to {volume}%";
			}

			await ctx.CreateResponseAsync(embed);
		}
	}

	public class RequireUserIdAttribute : SlashCheckBaseAttribute
	{
		public ulong UserId;

		public RequireUserIdAttribute(ulong userId)
		{
			this.UserId = userId;
		}
		public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
		{
			if (ctx.User.Id == UserId)
			{
				await ctx.CreateResponseAsync(":)");
				return true;
			}
			else
			{
				await ctx.CreateResponseAsync($"{DiscordEmoji.FromName(ctx.Client, ":skull:")}");
				return false;
			}
		}
	}
}