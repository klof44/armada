﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Linq;
using DSharpPlus.Lavalink;
using System.IO;
namespace armada
{
    internal class Commands : BaseCommandModule
	{
		//basic help command
		[Command("help")]
		public async Task Help(CommandContext ctx)
		{
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
			foreach (var cmd in ctx.CommandsNext.RegisteredCommands)
			{
				if (!cmd.Value.IsHidden)
				{
					CommandList += $" `{cmd.Key}` ";
				}
			}

			embed.AddField("Commands", CommandList);
			embed.AddField("Huh?", "For more help contact <@563891145256468481>");

			await ctx.RespondAsync(embed);

		}

		Random random = new Random();
		[Command("funny")]
		public async Task Funny(CommandContext ctx)
		{
			// posts random meme from Program.assetsDir + "/bot/funny"
			string[] files = Directory.GetFiles(Program.assetsDir + "/bot/funny");
				
			var path = files[random.Next(0, files.Length)];
			while (!Program.ValidMeme(path))
			{
				path = files[random.Next(0, files.Length)];
			}

			var meme = new FileStream(path, FileMode.Open, FileAccess.Read);
			var message = new DiscordMessageBuilder().WithFile(meme);
			await ctx.RespondAsync(message);
		}

		[Command("addmeme")]
		public async Task AddMeme(CommandContext ctx, Uri url)
		{
			// Easier way for me to add memes to the funny folder
			if (url.ToString().StartsWith("https://cdn.discordapp.com/attachments/") || url.ToString().StartsWith("https://media.discordapp.com/attachments/"))
			{
				await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":armada:"));

				var channel = ctx.Client.GetGuildAsync(913249395661750343).Result.GetMemberAsync(563891145256468481).Result;
				DiscordMessage message = await channel.SendMessageAsync(url.ToString());

				DiscordEmbedBuilder embed = new()
				{
					Color = DiscordColor.HotPink,
					Title = "New Sumbission",
				};
				embed.AddField("Submitted by", $"<@{ctx.Member.Id}>");
				embed.AddField("URL", url.ToString());

				await channel.CreateDmChannelAsync().Result.SendMessageAsync(embed);
			}
			else
			{
				await ctx.RespondAsync("Please use a url starting with `https://cdn.discordapp.com/attachments/` or `https://media.discordapp.com/attachments/`");
			}
		}

		[Command("addmeme")]
		public async Task MemeNoLink(CommandContext ctx)
		{
			var file = ctx.Message.Attachments.FirstOrDefault();
			var url = file.Url;
			if (url.ToString().StartsWith("https://cdn.discordapp.com/attachments/"))
			{
				await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":armada:"));

				var channel = ctx.Client.GetGuildAsync(913249395661750343).Result.GetMemberAsync(563891145256468481).Result;
				DiscordMessage message = await channel.SendMessageAsync(url.ToString());

				DiscordEmbedBuilder embed = new()
				{
					Color = DiscordColor.HotPink,
					Title = "New Sumbission",
				};
				embed.AddField("Submitted by", $"<@{ctx.Member.Id}>");
				embed.AddField("URL", url.ToString());

				await channel.CreateDmChannelAsync().Result.SendMessageAsync(embed);
			}
			else
			{
				await ctx.RespondAsync("Please use a `.mp4`, `.gif`, `.webm`, `.png`, or `.jpg` file");
			}
		}

		[Command("info")]
		public async Task Info(CommandContext ctx)
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
                },
            };

			embed.AddField("Commands count", ctx.CommandsNext.RegisteredCommands.Count.ToString());
            embed.AddField("Guilds", $"In {ctx.Client.Guilds.Count} guilds");
			embed.AddField("Memes", Directory.GetFiles(Program.assetsDir + "/bot/funny").Length.ToString());
            embed.AddField("Ping", ctx.Client.Ping.ToString() + "ms");

			await ctx.RespondAsync(embed);
		}

		[Command("actuallyfuckingdie")]
		[Hidden]
		public async Task ActuallyFuckingDie(CommandContext ctx)
		{
			// fully disconnects bot from discord
			if (ctx.Member.Id == 563891145256468481)
			{
				await ctx.Client.DisconnectAsync();
			}
		}

		[Command("delmsg")]
		[Hidden]
		public async Task DelMsg(CommandContext ctx, ulong id)
		{
			// deletes the specified message
			if (ctx.Member.Id == 563891145256468481)
			{
				var msg = await ctx.Channel.GetMessageAsync(id);
				await msg.DeleteAsync();
			}
		}

		[Command("youropinionisshit")]
		[Hidden]
		public async Task Opinion(CommandContext ctx, ulong id)
		{
			// server mute / unmutes a user
			if (ctx.Member.Id == 563891145256468481)
			{
				var member = await ctx.Guild.GetMemberAsync(id);
				await member.SetDeafAsync(!member.IsDeafened);
				await member.SetMuteAsync(!member.IsMuted);
			}

		}

		[Command("swearcount")]
		public async Task Swear(CommandContext ctx)
		{
			// Displays swear counter leaderboard
			DiscordEmbedBuilder embed = new()
			{
				Color = DiscordColor.HotPink
			};
			embed.AddField("Total", Program.swearCount.ToString());

			string board = "";

			var sorted = Program.leaderboard.OrderByDescending(key => key.Value);
			foreach (var user in sorted)
			{
				if (user.Value != 0)
				{
					board += $"\r\n<@{user.Key}> - {user.Value}";
				}
			}

			if (board == "")
			{
				board = "Nothing to display :(";
			}

			embed.AddField("Leaderboard", board);

			await ctx.RespondAsync(embed);
			
		}

		[Command("swearcount")]
		public async Task Swears(CommandContext ctx, ulong id)
		{
			// gives swear count of a specific user along with the ratio of messages with swears to messages without
			// uses Program.leaderboard and Program.ratios
			DiscordEmbedBuilder embed = new()
			{
				Color = DiscordColor.HotPink
			};

			if (Program.leaderboard.ContainsKey(id))
			{
				embed.AddField("Swear count", Program.leaderboard[id].ToString());
				embed.AddField("Ratio", $"{Program.leaderboard[id]}/{Program.ratios[id]} ({ Math.Round(Convert.ToDouble(Program.leaderboard[id]) / Convert.ToDouble(Program.ratios[id]) * 100, 6)}%)");
			}
			else
			{
				embed.AddField("Error", $"No data for <@{id}>\r\nSpeaking in any server with armada will automatically add them to the leaderboard");
			}
			await ctx.RespondAsync(embed);
		}

		// Allows users with permission to change the swearcount of a user
		[Command("ChangeSwears")]
		[Hidden]
		public async Task ChangeSwear(CommandContext ctx, DiscordUser user, int amount)
		{
			if (ctx.User.Id == 563891145256468481)
			{
                Program.leaderboard[user.Id] += amount;
                Program.swearCount += amount;
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":skull:"));
            }
		}


		[Command("talk")]
		public async Task Play(CommandContext ctx, [RemainingText] string search)
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

		[Command("shutup")]
		public async Task Leave(CommandContext ctx)
		{
			// Disconnect from voice chat and clear queue
			await ctx.Client.GetLavalink().GetGuildConnection(ctx.Guild).DisconnectAsync();
			await MusicPlayer.Stop(ctx);
		}

		[Command("skip")]
		public async Task Skip(CommandContext ctx)
		{
			// Skip current song
			await MusicPlayer.Skip(ctx);
		}

		[Command("queue")]
		public async Task Queue(CommandContext ctx)
		{
			// Displays the queue
			await MusicPlayer.SayQueue(ctx);
		}

		[Command("shuffle")]
		public async Task Shuffle(CommandContext ctx)
		{
			// Shuffles the queue
			await MusicPlayer.Shuffle(ctx);
		}

		[Command("vol")]
		public async Task Volume(CommandContext ctx, int volume)
		{
			// Changes volume of music

			if (ctx.Member.VoiceState == null)
			{
				DiscordEmbedBuilder embedDeny = new()
				{
					Color = DiscordColor.HotPink,
					Title = "You can't do that"
				};

				await ctx.RespondAsync(embedDeny);
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
					await ctx.Client.GetLavalink().GetGuildConnection(ctx.Guild).SetVolumeAsync(volume);
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
				await ctx.Client.GetLavalink().GetGuildConnection(ctx.Guild).SetVolumeAsync(volume);
				embed.Title = $"Volume set to {volume}%";
			}

			await ctx.RespondAsync(embed);
		}

		[Command("makebalancedteams")]
		[Hidden]
		public async Task Teams(CommandContext ctx)
		{
			// groups users that react to a message into 2 random teams
			DiscordEmbedBuilder embed = new()
			{
				Color = DiscordColor.HotPink
			};

			var msg = ctx.Channel.SendMessageAsync($"React to this message with {DiscordEmoji.FromName(ctx.Client, ":skull:")} within the next 20 seconds to added to a team");
			var react = msg.Result.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":armada:"));

			await Task.Delay(TimeSpan.FromSeconds(20));

			var users = msg.Result.GetReactionsAsync(DiscordEmoji.FromName(ctx.Client, ":armada:"), 50).Result.ToList();

			if (users.Count < 2)
			{
				embed.AddField("Error", "Less than 2 people reacted");
				return;
			}

			users.OrderBy(x => random.Next(0, users.Count - 1));

			string Team1 = "";
			string Team2 = "";

			bool Randomness = true;
			foreach (var member in users)
			{
				if (Randomness)
				{
					Team1 += $"\r\n{member.Mention}";
					Randomness = false;
				}
				else
				{
					Team2 += $"\r\n{member.Mention}";
					Randomness = true;
				}
			}

			embed.AddField("Team 1", Team1);
			embed.AddField("Team 2", Team2);

			await ctx.RespondAsync(embed);
		}
	}
}