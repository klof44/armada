using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus;
using DSharpPlus.Entities;
using System.Linq;
using DSharpPlus.Lavalink;
using System.IO;

namespace armada
{
	internal class Commands : BaseCommandModule
	{
		// Normal user commands should have "if (!Program.InactiveServers.Contains(ctx.Guild.Id))" just in case something goes wrong and an admin can kill the bot without disconnecting from discord
		// Powerful commands like !actuallyfuckingdie and !nick can be hardcoded to only accept my user id (563891145256468481)

		[Command("roll")]
		public async Task Roll(CommandContext ctx, int count, int sides, int mod = 0)
		{
			// dice roll command

			if (!Program.InactiveServers.Contains(ctx.Guild.Id))
            {
                
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
            }
		}

		[Command("perm")]
		[Hidden]
		public async Task Perm(CommandContext ctx, ulong id)
		{
			// change users who can !kill
			// uses Program.HasPerms
		}

		[Command("actuallyfuckingdie")]
		[Hidden]
		public async Task ActuallyFuckingDie(CommandContext ctx)
		{
			// fully disconnects bot from discord
		}

		[Command("kill")]
		[Hidden]
		public async Task KillCommand(CommandContext ctx)
		{
			// Makes the bot ignore a guild
			// Uses Program.InactiveServers
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
			// plays LTG quote in voice chat (You should kiill yourself NOW!)
		}

		[Command("changefunny")]
		[Hidden]
		public async Task YouFunny(CommandContext ctx, ulong id)
		{
			// changes a users persission to use voice chat commands
			// uses	Program.NotFunny
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
		}

		private Random random = new Random();
	}
}