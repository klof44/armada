using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using Microsoft.Extensions.Logging;
using DSharpPlus.SlashCommands;

namespace armada
{
    internal class Program
    {
        // This entire class is unreadable because of decompilation but it works so I'll leave it
        // Make sure you have Lavalink.jar running while the bot is running

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += Process_Exit;
            Program.swearCount = Program.GetSwearCout();
            Program.LoadSwears();
			if (Directory.GetFiles(Program.assetsDir + "/bot/funny").Length == 0)
			{
				Console.WriteLine("WARNING!!!! NO MEMES FOUND IN " + assetsDir + "/bot/funny");
			}
            Program.MainAsync().GetAwaiter().GetResult();
        }

        private static void Process_Exit(object sender, EventArgs e)
        {
			discord.DisconnectAsync();
        }

		private static async Task MainAsync()
		{

			discord.MessageCreated += async (s, e) =>
			{
				CheckSwear(e.Message.Content, e.Author, e);

				if (e.Message.Content == "armada")
				{
					await e.Message.RespondAsync(DiscordEmoji.FromName(discord, ":armada:"));
				}
			};

			commands.RegisterCommands<Commands>();
		
			var lavalink = discord.UseLavalink();

			var slash = discord.UseSlashCommands();
			
			slash.RegisterCommands<SlashCommands>(913249395661750343); // Armada Dev
			slash.RegisterCommands<SlashCommands>(754835352950276189); // Spudland

			await discord.ConnectAsync();

			discord.Ready += async (s, e) =>
			{
				await discord.UpdateStatusAsync(new DiscordActivity("with YOUR balls", ActivityType.Playing));
			};

			await lavalink.ConnectAsync(lavalinkConfig);

			await Task.Delay(-1);
		}


		private static string GetToken()
		{
			return new StreamReader(Program.assetsDir + "bot/settings/token").ReadLine();
		}
		
		public static Dictionary<string, string> MediaTypes()
		{
			Dictionary<string, string> mediatypes = new();
			mediatypes.Add("mp4", "video");
			mediatypes.Add("webm", "video");
			mediatypes.Add("png", "image");
			mediatypes.Add("jpg", "image");
			mediatypes.Add("jpeg", "image");
			mediatypes.Add("gif", "image");
			return mediatypes;
		}
		internal static string assetsDir = Directory.GetCurrentDirectory() + "\\";
		internal static List<string> Repost = new List<string>();
		public static ulong klofId = 563891145256468481;


		public static bool ValidMeme(string path)
		{
			if (Program.Repost.Contains(path))
			{
				Program.Repost.Remove(path);
				return false;
			}
			Program.Repost.Add(path);
			return true;
		}


		public static List<string> swears = new List<string>();
		public static long swearCount;
		internal static Dictionary<ulong, long> leaderboard = new Dictionary<ulong, long>();
		internal static Dictionary<ulong, long> ratios = new Dictionary<ulong, long>();

		internal static void CheckSwear(string Message, DiscordUser user, MessageCreateEventArgs arg)
		{
			if (!Program.leaderboard.ContainsKey(user.Id))
			{
				Program.leaderboard.Add(user.Id, 0L);
			}
			bool swore = false;
			foreach (string value in Program.swears)
			{
				if (Message.Contains(value))
				{
					swore = true;
					ulong id = user.Id;
					long num = leaderboard[id];
					leaderboard[id] = num + 1;
					Program.swearCount += 1;
					string value2 = "DM channel";
					if (!(arg.Channel is DiscordDmChannel))
					{
						value2 = $"https://discord.com/channels/{arg.Guild.Id}/{arg.Channel.Id}/{arg.Message.Id}";
					}
					DiscordEmbedBuilder discordEmbedBuilder = new DiscordEmbedBuilder
					{
						Color = DiscordColor.HotPink,
						Timestamp = new DateTimeOffset?(DateTime.Now)
					};
					discordEmbedBuilder.AddField("From", string.Format("<@{0}>", arg.Author.Id), false);
					discordEmbedBuilder.AddField("Message", Message, false);
					discordEmbedBuilder.AddField("Link", value2, false);
					if (arg.Guild.Id == 754835352950276189UL)
					{
						Program.discord.GetGuildAsync(754835352950276189).Result.GetChannel(935680546384781412).SendMessageAsync(discordEmbedBuilder);
					}
					Program.discord.GetGuildAsync(913249395661750343).Result.GetChannel(934941707613634661).SendMessageAsync(discordEmbedBuilder);
				}
			}
			if (!swore)
			{
				if (!Program.ratios.ContainsKey(user.Id))
				{
					Program.ratios.Add(user.Id, 1L);
				}
				else
				{
					ulong id = user.Id;
					long num = ratios[id];
					ratios[id] = num + 1L;
				}
			}
			Program.SaveSwear();
		}

		internal static void SaveSwear()
		{
			StreamWriter countStream = new StreamWriter(Program.assetsDir + "bot/SwearCounter/Count");
			countStream.WriteLine(Program.swearCount);
			countStream.Close();

			StreamWriter leaderboardStream = new StreamWriter(Program.assetsDir + "bot/SwearCounter/leaderboard.txt");
			leaderboardStream.WriteLine(Program.leaderboard.Count);
			foreach (KeyValuePair<ulong, long> keyValuePair in Program.leaderboard)
			{
				leaderboardStream.WriteLine(keyValuePair.Key);
				leaderboardStream.WriteLine(keyValuePair.Value);
			}
			leaderboardStream.Close();

			StreamWriter ratioStream = new StreamWriter(Program.assetsDir + "bot/SwearCounter/ratio.txt");
			ratioStream.WriteLine(Program.ratios.Count);
			foreach (KeyValuePair<ulong, long> keyValuePair2 in Program.ratios)
			{
				ratioStream.WriteLine(keyValuePair2.Key);
				ratioStream.WriteLine(keyValuePair2.Value);
			}
			ratioStream.Close();
		}

		internal static long GetSwearCout()
		{
			return Convert.ToInt64(new StreamReader(Program.assetsDir + "bot/SwearCounter/Count").ReadLine());
		}

		private static void LoadSwears()
		{
			StreamReader swearsStream = new StreamReader(Program.assetsDir + "bot/SwearCounter/swears.txt");
			for (int i = Convert.ToInt32(swearsStream.ReadLine()); i > 0; i--)
			{
				Program.swears.Add(swearsStream.ReadLine().ToLower());
			}
			swearsStream.Close();

			StreamReader leaderboardStream = new StreamReader(Program.assetsDir + "bot/SwearCounter/leaderboard.txt");
			for (int j = Convert.ToInt32(leaderboardStream.ReadLine()); j > 0; j--)
			{
				Program.leaderboard.Add(Convert.ToUInt64(leaderboardStream.ReadLine()), Convert.ToInt64(leaderboardStream.ReadLine()));
			}
			leaderboardStream.Close();

			StreamReader ratioStream = new StreamReader(Program.assetsDir + "bot/SwearCounter/ratio.txt");
			for (int k = Convert.ToInt32(ratioStream.ReadLine()); k > 0; k--)
			{
				Program.ratios.Add(Convert.ToUInt64(ratioStream.ReadLine()), Convert.ToInt64(ratioStream.ReadLine()));
			}
			ratioStream.Close();
		}


		private static ConnectionEndpoint endpoint = new ConnectionEndpoint
		{
			Hostname = "127.0.0.1",
			Port = 8000
		};

		private static LavalinkConfiguration lavalinkConfig = new LavalinkConfiguration
		{
			Password = "greatpassword",
			RestEndpoint = Program.endpoint,
			SocketEndpoint = Program.endpoint
		};

		private static string token = Program.GetToken();

		internal static DiscordClient discord = new DiscordClient(new DiscordConfiguration
		{
			Token = Program.token,
			TokenType = TokenType.Bot,
			Intents = DiscordIntents.All,
			MinimumLogLevel = LogLevel.Debug,
			ReconnectIndefinitely = true,
		});

		internal static CommandsNextExtension commands = Program.discord.UseCommandsNext(new CommandsNextConfiguration
		{
			StringPrefixes = new string[]
			{
				"!"
			},
			EnableDefaultHelp = false,
		});
	}
}
