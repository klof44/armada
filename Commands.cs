using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Linq;
using DSharpPlus.Lavalink;
using System.IO;
using System.Net;
namespace armada
{
    internal class Commands : BaseCommandModule
	{
        // Normal user commands should have "if (!Program.InactiveServers.Contains(ctx.Guild.Id))" just in case something goes wrong and an admin can kill the bot without disconnecting from discord
        // Powerful commands like !actuallyfuckingdie and !nick can be hardcoded to only accept my user id (563891145256468481)

        // IDEAS: Way to request for a meme to be added to the funny folder in a way that they have to be manually approved

        [Command("roll")]
		public async Task Roll(CommandContext ctx, int count, int sides, int mod = 0)
		{
			// dice roll command

			if (!Program.InactiveServers.Contains(ctx.Guild.Id))
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                {
					Color = DiscordColor.HotPink,
                    Title = $"{count}d{sides} + {mod}",
                };

                long total = 0;
                string rolls = "";
                for (int i = 0; i < count; i++)
                {
                    int roll = random.Next(1, sides + 1);
                    rolls += $"{roll}{DiscordEmoji.FromName(ctx.Client, ":black_small_square:")}";
                    total += roll;
                }

                if (mod != 0)
                {
					embed.AddField($"Total: {total}", $"{rolls}");
                }
				else
                {
					embed.AddField($"Total: {total + mod}", $"{rolls}");
				}

				await ctx.RespondAsync(embed);
			}
		}

		//basic help command
		[Command("help")]
		public async Task Help(CommandContext ctx)
		{
			if (!Program.InactiveServers.Contains(ctx.Guild.Id))
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

		}

		[Command("funny")]
		public async Task Funny(CommandContext ctx)
		{
			// posts random meme from Program.assetsDir + "/bot/funny"

			if (!Program.InactiveServers.Contains(ctx.Guild.Id))
			{
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
		}

		Dictionary<ulong, CommandContext> submissions = new Dictionary<ulong, CommandContext>();
		[Command("addmeme")]
		public async Task AddMeme(CommandContext ctx, Uri url)
		{
			// Easier way for me to add memes to the funny folder

			if (!Program.InactiveServers.Contains(ctx.Guild.Id) && ctx.User.Id == 563891145256468481)
			{
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

					submissions.Add(message.Id, ctx);
				}
				else
				{
					await ctx.RespondAsync("Please use a url starting with `https://cdn.discordapp.com/attachments/`");
				}
			}
		}

		[Command("judge")]
		[Hidden]
		public async Task JudgeMeme(CommandContext ctx, ulong id, string verdict)
		{
			if (ctx.Channel.IsPrivate && ctx.Channel.Id == 689841330062491657)
			{
				if (verdict.ToLower() == "good")
				{
					WebClient client = new WebClient();
					byte[] data = client.DownloadData(new Uri(submissions[id].Message.Content));
					File.WriteAllBytes(Program.assetsDir + "/bot/funny/" + submissions[id].Message.Id + "." + ctx.Message.Content.Split(".").Last(), data);

					await submissions[id].RespondAsync("Downloaded");
				}
				if (verdict.ToLower() == "bad")
				{
					await submissions[id].Message.DeleteAsync();
				}
				else
				{
					await ctx.RespondAsync("?");
				}

				submissions.Remove(id);
			}
		}

		[Command("info")]
		public async Task Info(CommandContext ctx)
		{
			// Help command but more info

			if (!Program.InactiveServers.Contains(ctx.Guild.Id))
			{
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
		}

		[Command("perm")]
		[Hidden]
		public async Task Perm(CommandContext ctx, ulong id)
		{
			// change users who can !kill
			// uses Program.HasPerms
			if (ctx.Member.Id == 563891145256468481)
			{
				if (Program.HasPerms.Contains(id))
				{
					Program.HasPerms.Remove(id);
					await ctx.RespondAsync($"Removed <@{id}>");
				}
				else
				{
					Program.HasPerms.Add(id);
					await ctx.RespondAsync($"Added <@{id}>");
				}
			}
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

		[Command("kill")]
		[Hidden]
		public async Task KillCommand(CommandContext ctx)
		{
			// Makes the bot ignore a guild
			// Uses Program.InactiveServers
			if (Program.HasPerms.Contains(ctx.Member.Id))
			{
				if (!Program.InactiveServers.Contains(ctx.Guild.Id))
				{
					Program.InactiveServers.Add(ctx.Guild.Id);
					await ctx.RespondAsync($"{ctx.Member.Mention} has disabled the bot in this guild.");
				}
				else
				{
					Program.InactiveServers.Remove(ctx.Guild.Id);
					await ctx.RespondAsync($"{ctx.Member.Mention} has enabled the bot in this guild.");
				}
			}
		}

		[Command("changefunny")]
		[Hidden]
		public async Task YouFunny(CommandContext ctx, ulong id)
		{
			// changes a users persission to use voice chat commands
			// uses	Program.NotFunny

			if (!Program.InactiveServers.Contains(ctx.Member.Id))
			{
				if (Program.NotFunny.Contains(id))
				{
					Program.NotFunny.Remove(id);
					await ctx.RespondAsync($"Good ending {DiscordEmoji.FromName(ctx.Client, ":smiley:")}");
				}
				else
				{
					Program.NotFunny.Add(id);
					await ctx.RespondAsync($"{ctx.Client.GetUserAsync(id).Result.Mention} fuck you {DiscordEmoji.FromName(ctx.Client, ":skull:")}");
				}
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

			if (!Program.InactiveServers.Contains(ctx.Guild.Id))
			{
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
		}

		[Command("swearcount")]
		public async Task Swears(CommandContext ctx, ulong id)
		{
			// gives swear count of a specific user along with the ratio of messages with swears to messages without
			// uses Program.leaderboard and Program.ratios

			if (!Program.InactiveServers.Contains(ctx.Guild.Id))
			{
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

				await ctx.RespondAsync(embed);
			}
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
			LavalinkTrack track;
			if (Uri.TryCreate(search, UriKind.Absolute, out Uri uri))
			{
				track = ctx.Client.GetLavalink().ConnectedNodes.Values.First().Rest.GetTracksAsync(uri).Result.Tracks.First();
			}
			else
			{
				track = ctx.Client.GetLavalink().ConnectedNodes.Values.First().Rest.GetTracksAsync(search).Result.Tracks.First();
			}

			await MusicPlayer.Play(track, ctx);
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

		[Command("makebalancedteams")]
		[Hidden]
		public async Task Teams(CommandContext ctx)
		{
			// groups users that react to a message into 2 random teams
			if (!Program.InactiveServers.Contains(ctx.Guild.Id))
			{
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
    
		private static Random random = new Random();
	}
}