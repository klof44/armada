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
using DSharpPlus.SlashCommands;

namespace armada
{
    internal class SlashCommands : ApplicationCommandModule
	{
        // Normal user commands should have "if (!Program.InactiveServers.Contains(ctx.Guild.Id))" just in case something goes wrong and an admin can kill the bot without disconnecting from discord
        // Powerful commands like !actuallyfuckingdie and !nick can be hardcoded to only accept my user id (563891145256468481)

        // IDEAS: Way to request for a meme to be added to the funny folder in a way that they have to be manually approved

        [SlashCommand("roll", "Roll as many dice as you want")]
		public async Task Roll(InteractionContext ctx, [Option("Count", "How many dice you want to roll")] long count = 1, [Option("Sides", "How many sides are on the dice")] long sides = 20, [Option("Modifier", "Adds to the total")] long mod = 0)
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
                    long roll = random.Next(1, (int) sides + 1);
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

				await ctx.CreateResponseAsync(embed);
			}
		}

		//basic help command
		[SlashCommand("help", "Base help command for non-slash commands")]
		public async Task Help(InteractionContext ctx)
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

		}

		[SlashCommand("funny", "funny command")]
		public async Task Funny(InteractionContext ctx)
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
				var message = new DiscordInteractionResponseBuilder().AddFile(meme);
				await ctx.CreateResponseAsync(message);
			}
		}

		[SlashCommand("info", "help but more info")]
		public async Task Info(InteractionContext ctx)
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

				embed.AddField("non-slash command count", ctx.Client.GetCommandsNext().RegisteredCommands.Count.ToString());
                embed.AddField("Guilds", $"In {ctx.Client.Guilds.Count} guilds");
				embed.AddField("Memes", Directory.GetFiles(Program.assetsDir + "/bot/funny").Length.ToString());
                embed.AddField("Ping", ctx.Client.Ping.ToString() + "ms");

				await ctx.CreateResponseAsync(embed);
            }
		}

		[SlashCommand("swearcount", "Shows the swearcount leaderboard")]
		public async Task Swear(InteractionContext ctx)
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

				await ctx.CreateResponseAsync(embed);
			}
		}

		[SlashCommand("swearcount", "Gets the swear count of a specific user")]
		public async Task Swears(InteractionContext ctx, [Option("User", "Who's stats you want to see")] DiscordUser user)
		{
			if (!Program.InactiveServers.Contains(ctx.Guild.Id))
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
		}
		private static Random random = new Random();
	}
}