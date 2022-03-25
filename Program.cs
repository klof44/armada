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

namespace armada
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += Process_Exit;
            Program.LoadSettings();
            Program.swearCount = Program.GetSwearCout();
            Program.LoadSwears();
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
			};
			
			await discord.ConnectAsync();
			await Task.Delay(-1);
		}

		private static string GetToken()
		{
			return new StreamReader(Program.assetsDir + "bot/token").ReadLine();
		}

		internal static List<string> armada = new List<string>();
		internal static string assetsDir = Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().Length - 6) + "/";
		internal static List<ulong> InactiveServers = new List<ulong>();
		internal static List<ulong> HasPerms = new List<ulong>();
		internal static List<ulong> NotFunny = new List<ulong>();
		internal static List<string> Invalid = new List<string>();

		internal static void LoadSettings()
		{
			StreamReader streamReader = new StreamReader(Program.assetsDir + "bot/settings/hasperms");
			for (int i = Convert.ToInt32(streamReader.ReadLine()); i > 0; i--)
			{
				Program.HasPerms.Add(Convert.ToUInt64(streamReader.ReadLine()));
			}
			streamReader.Close();
			StreamReader streamReader2 = new StreamReader(Program.assetsDir + "bot/settings/notfunny");
			for (int j = Convert.ToInt32(streamReader2.ReadLine()); j > 0; j--)
			{
				Program.NotFunny.Add(Convert.ToUInt64(streamReader2.ReadLine()));
			}
			streamReader2.Close();
		}

		internal static void SaveSettings()
		{
			StreamWriter streamWriter = new StreamWriter(Program.assetsDir + "bot/settings/hasperms");
			streamWriter.WriteLine(Program.HasPerms.Count);
			using (List<ulong>.Enumerator enumerator = Program.HasPerms.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					long num = (long)enumerator.Current;
					streamWriter.WriteLine(num.ToString());
				}
			}
			streamWriter.Close();
			StreamWriter streamWriter2 = new StreamWriter(Program.assetsDir + "bot/settings/isfunny");
			streamWriter2.WriteLine(Program.NotFunny.Count);
			using (List<ulong>.Enumerator enumerator = Program.NotFunny.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					long num2 = (long)enumerator.Current;
					streamWriter2.WriteLine(num2.ToString());
				}
			}
			streamWriter2.Close();
		}

		public static bool ValidMeme(string path)
		{
			if (Program.Invalid.Contains(path))
			{
				Program.Invalid.Remove(path);
				return false;
			}
			Program.Invalid.Add(path);
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
			bool flag = false;
			foreach (string value in Program.swears)
			{
				if (Message.Contains(value))
				{
					flag = true;
					Dictionary<ulong, long> dictionary = Program.leaderboard;
					ulong id = user.Id;
					long num = dictionary[id];
					dictionary[id] = num + 1L;
					Program.swearCount += 1L;
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
						Program.discord.GetGuildAsync(754835352950276189UL, null).Result.GetChannel(935680546384781412UL).SendMessageAsync(discordEmbedBuilder);
					}
					Program.discord.GetGuildAsync(913249395661750343UL, null).Result.GetChannel(934941707613634661UL).SendMessageAsync(discordEmbedBuilder);
				}
			}
			if (!flag)
			{
				if (!Program.ratios.ContainsKey(user.Id))
				{
					Program.ratios.Add(user.Id, 1L);
				}
				else
				{
					Dictionary<ulong, long> dictionary2 = Program.ratios;
					ulong id = user.Id;
					long num = dictionary2[id];
					dictionary2[id] = num + 1L;
				}
			}
			Program.SaveSwear();
		}

		internal static void SaveSwear()
		{
			StreamWriter streamWriter = new StreamWriter(Program.assetsDir + "bot/SwearCounter/Count");
			streamWriter.WriteLine(Program.swearCount);
			streamWriter.Close();
			StreamWriter streamWriter2 = new StreamWriter(Program.assetsDir + "bot/SwearCounter/leaderboard.txt");
			streamWriter2.WriteLine(Program.leaderboard.Count);
			foreach (KeyValuePair<ulong, long> keyValuePair in Program.leaderboard)
			{
				streamWriter2.WriteLine(keyValuePair.Key);
				streamWriter2.WriteLine(keyValuePair.Value);
			}
			streamWriter2.Close();
			StreamWriter streamWriter3 = new StreamWriter(Program.assetsDir + "bot/SwearCounter/ratio.txt");
			streamWriter3.WriteLine(Program.ratios.Count);
			foreach (KeyValuePair<ulong, long> keyValuePair2 in Program.ratios)
			{
				streamWriter3.WriteLine(keyValuePair2.Key);
				streamWriter3.WriteLine(keyValuePair2.Value);
			}
			streamWriter3.Close();
		}

		internal static long GetSwearCout()
		{
			return Convert.ToInt64(new StreamReader(Program.assetsDir + "bot/SwearCounter/Count").ReadLine());
		}

		private static void LoadSwears()
		{
			StreamReader streamReader = new StreamReader(Program.assetsDir + "bot/SwearCounter/swears.txt");
			for (int i = Convert.ToInt32(streamReader.ReadLine()); i > 0; i--)
			{
				Program.swears.Add(streamReader.ReadLine().ToLower());
			}
			streamReader.Close();
			StreamReader streamReader2 = new StreamReader(Program.assetsDir + "bot/SwearCounter/leaderboard.txt");
			for (int j = Convert.ToInt32(streamReader2.ReadLine()); j > 0; j--)
			{
				Program.leaderboard.Add(Convert.ToUInt64(streamReader2.ReadLine()), Convert.ToInt64(streamReader2.ReadLine()));
			}
			streamReader2.Close();
			StreamReader streamReader3 = new StreamReader(Program.assetsDir + "bot/SwearCounter/ratio.txt");
			for (int k = Convert.ToInt32(streamReader3.ReadLine()); k > 0; k--)
			{
				Program.ratios.Add(Convert.ToUInt64(streamReader3.ReadLine()), Convert.ToInt64(streamReader3.ReadLine()));
			}
			streamReader3.Close();
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
			MinimumLogLevel = LogLevel.Debug
		});

		internal static CommandsNextExtension commands = Program.discord.UseCommandsNext(new CommandsNextConfiguration
		{
			StringPrefixes = new string[]
			{
				"!"
			},
			EnableDefaultHelp = false
		});
	}
}
