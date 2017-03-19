﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Discord;
using Discord.WebSocket;
using System.Windows.Forms;
using Discord.Commands;
using System.Reflection;
using System.IO;
using System.Net;
using System.Drawing;
using System.Runtime.InteropServices;
using Discord.Net.Providers.WS4Net;
using Discord.Commands;
using System.Diagnostics;
using System.Timers;

namespace Discore_Selfbot
{
    class Program
    {
        public static bool Ready = false;
        public static int RunOnce = 0;
        private CommandService commands;
        public static DiscordSocketClient client;
        private DependencyMap map;
        public static string SelfbotDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Discore-Selfbot\\";
        public static bool DownloadGuilds = false;
        public static List<string> Guilds = new List<string>();
        public static List<string> Channels = new List<string>();
        public static List<ulong> ChannelsID = new List<ulong>();
        public static ulong ActiveGuildID = 0;
        public static int Uptime = 0;
        public static ulong ActiveChannelID = 0;
        public static Icon Avatar;
        public static Discord.Color FavoriteColor;
        public static GUI MyForm;
        public static System.Timers.Timer AutoNickname_Timer = new System.Timers.Timer();
        public static string CurrentUserName;
        public static ulong CurrentUserID;
        public static Random RandomGenerator = new Random((int)DateTime.Now.Ticks + DateTime.Now.Year);
        static void Main()
        {
            DisableConsoleQuickEdit.Go();
            Properties.Settings.Default.TotalRuns++;
            Console.ForegroundColor = ConsoleColor.White;
            if (Properties.Settings.Default.ANGuildsList == null)
            {
                Properties.Settings.Default.ANGuildsList = new System.Collections.Specialized.StringCollection();
            }
            Properties.Settings.Default.Save();
            Console.Title = "Discore - Selfbot - User Token Required";
            string Token = "";
            Directory.CreateDirectory(SelfbotDir);
            Directory.CreateDirectory(SelfbotDir + "Tags");
            Directory.CreateDirectory(SelfbotDir + "Nicknames");
            if (File.Exists(SelfbotDir + "Token.txt"))
            {
                Token = File.ReadAllText(SelfbotDir + "Token.txt");
            }
            else
            {
                File.CreateText(SelfbotDir + "Token.txt");
                Console.WriteLine("Insert your User Token into the file Token.txt and restart the bot");
                Console.WriteLine("And no i dont steal tokens you can view the code on github ");
                Process.Start(SelfbotDir);
            }
            if (Token == "")
            {
                Console.WriteLine("Token not found please enter your user token in this file and restart the bot");
                Process.Start(SelfbotDir);
                if (!File.Exists(SelfbotDir + "How-To-Get-User-Token.txt"))
                {
                    using (StreamWriter sw = File.CreateText(SelfbotDir + "How-To-Get-User-Token.txt"))
                    {
                        sw.WriteLine("Open this in your web browser");
                        sw.WriteLine("");
                        sw.WriteLine("https://github.com/ArchboxDev/Discore-Selfbot/blob/master/UserToken.md");
                    }
                }
            }
                while (Token == "")
            {
                
            }
            Console.Title = "Discore - Selfbot";
            Console.WriteLine("Token found Loading Bot");
                new Program().RunBot().GetAwaiter().GetResult();
        }

        

        [STAThread]
        public static void OpenGUI()
        {
            GUI.CheckForIllegalCrossThreadCalls = false;
            MyForm = new GUI();
            if (Properties.Settings.Default.AutoForm == "No")
            {
                if (Ready == false)
                {
                    return;
                }
            }
                if (Ready == false)
                {
                    Console.WriteLine("Opening GUI");
                }
                Task mytask = Task.Run(() =>
                    {
                        MyForm.ShowDialog();
                    });
        }

        public async Task RunBot()
        {
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                WebSocketProvider = WS4NetProvider.Instance
            });
            commands = new CommandService();
            map = new DependencyMap();
            await InstallCommands();
            WebClient WBC = new WebClient();
            WebClient myWebClient = new WebClient();
            FavoriteColor = new Discord.Color(Properties.Settings.Default.FavoriteColor.R, Properties.Settings.Default.FavoriteColor.G, Properties.Settings.Default.FavoriteColor.B);
            int GuildsCount = 0;
            RunOnce = 0;
            client.GuildAvailable += (g) =>
            {
                if (!Guilds.Contains(g.Name))
                {
                    GuildsCount++;
                    Guilds.Add(g.Name);
                    if (g.IconUrl == null)
                    {
                        WBC.DownloadFile("http://dev.blaze.ml/Letters/" + g.Name.ToUpper().ToCharArray()[0] + ".png", $"{g.Id}.png");
                    }
                    else
                    {
                        WBC.DownloadFile(g.IconUrl, $"{g.Id}.png");
                    }
                    var Item = MyForm.GuildList.Items.Add(g.Name, System.Drawing.Image.FromFile($"{g.Id}.png"));
                    Item.AccessibleDescription = g.Id.ToString();
                    Item.DisplayStyle = ToolStripItemDisplayStyle.Image;
                }
                if (GuildsCount == client.Guilds.Count())
                {
                    if (Ready == false)
                    {
                        Ready = true;
                        AutoNickname_Timer.Interval = 60000;
                        if (Properties.Settings.Default.ANTimer == "5")
                        {
                            AutoNickname_Timer.Interval = 300000;
                        }
                        if (Properties.Settings.Default.ANTimer == "10")
                        {
                            AutoNickname_Timer.Interval = 600000;
                        }
                        AutoNickname_Timer.Elapsed += Timer;
                        AutoNickname_Timer.Start();
                        if (client.CurrentUser.Id == 190590364871032834)
                        {
                            Console.WriteLine("Hi master");
                        }
                        if (client.CurrentUser.Id == 213621714909659136)
                        {
                            Console.WriteLine("Bubbie's butt is bubbly");
                        }
                        if (client.CurrentUser.Id == 155490847494897664 || client.CurrentUser.Id == 107827535479353344)
                        {
                            Console.WriteLine("Julia + Novus <3");
                        }
                        if (client.CurrentUser.Id == 213627387206828032)
                        {
                            Console.WriteLine("Towergay");
                        }
                    }
                }
                return Task.CompletedTask;
            };
            client.JoinedGuild += (g) =>
            {
                if (!File.Exists($"{g.Id}.png"))
                {
                    if (g.IconUrl == null)
                    {
                        var GuildNameFormat = new String(g.Name.Where(Char.IsLetter).ToArray());
                        WBC.DownloadFile("http://dev.blaze.ml/Letters/" + GuildNameFormat.ToCharArray()[0] + ".png", $"{g.Id}.png");
                    }
                    else
                    {
                        WBC.DownloadFile(g.IconUrl, $"{g.Id}.png");
                    }
                    WBC.Dispose();
                }
                var Item = MyForm.GuildList.Items.Add(g.Name, System.Drawing.Image.FromFile($"{g.Id}.png"));
                Item.AccessibleDescription = g.Id.ToString();
                Item.DisplayStyle = ToolStripItemDisplayStyle.Image;
                
                Guilds.Add(g.Name);
                Console.WriteLine($"Joined Guild > {g.Name} ({g.Id}) - Owner {g.Owner.Username}");

                return Task.CompletedTask;
            };
            client.LeftGuild += (g) =>
            {
                int Index = Guilds.IndexOf(g.Name);
                MyForm.GuildList.Items.RemoveAt(Index);
                Guilds.Remove(g.Name);
                Console.WriteLine($"Left Guild > {g.Name} ({g.Id})");

                return Task.CompletedTask;
            };
            client.Connected += () =>
            {
                CurrentUserName = client.CurrentUser.Username;
                CurrentUserID = client.CurrentUser.Id;
                Console.Title = "Discore - Selfbot - Online!!";
                Console.WriteLine("CONNECTED!");
                
                if (File.Exists("avatar.png"))
                {
                    File.Delete("avatar.png");
                }
                myWebClient.DownloadFile(client.CurrentUser.GetAvatarUrl(), "avatar.png");
                Bitmap b = (Bitmap)System.Drawing.Image.FromFile("avatar.png");
                IntPtr pIcon = b.GetHicon();
                Icon i = Icon.FromHandle(pIcon);
                Avatar = i;
                i.Dispose();
                if (Properties.Settings.Default.AutoForm == "Yes")
                {
                    MyForm.Text = CurrentUserName;
                    MyForm.Icon = i;
                }
                return Task.CompletedTask;
            };
            client.Disconnected += (e) =>
            {
                Console.Title = "Discore - Selfbot - Offline!";
                Console.WriteLine("DISCONNECTED!");
                return Task.CompletedTask;
            };
            client.Ready += () =>
            {
                Console.WriteLine($"Selfbot is ready | Found {client.Guilds.Count()} guilds");
                RunOnce = 1;
                return Task.CompletedTask;
            };
            await client.LoginAsync(TokenType.User, File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Discore-Selfbot\\Token.txt"));
            await client.StartAsync();
            OpenGUI();
            await Task.Delay(-1);
        }
        public async static void SendMessage(IUserMessage CommandMessage, [Remainder] string Message)
        {
            if (CommandMessage.Channel is IPrivateChannel)
            {
                var Channel = CommandMessage.Channel as IDMChannel;
                if (Properties.Settings.Default.SendAction == "Edit")
                {
                    await CommandMessage.ModifyAsync(x =>
                    {
                        x.Content = $"`Selfbot | {Message}`";
                    });
                }
                else
                {
                    await CommandMessage.DeleteAsync();
                    await Channel.SendMessageAsync($"`Selfbot | {Message}`");
                }
            }
            else
            {
                var Channel = CommandMessage.Channel as ITextChannel;
                if (Properties.Settings.Default.SendAction == "Edit")
                {
                    await CommandMessage.ModifyAsync(x =>
                    {
                        x.Content = $"`Selfbot | {Message}`";
                    });
                }
                else
                {
                    await CommandMessage.DeleteAsync();
                    await Channel.SendMessageAsync($"`Selfbot | {Message}`");
                }
            }
        }
        public async static void SendEmbed(IUserMessage CommandMessage, Embed Embed)
        {
            if (CommandMessage.Channel is IPrivateChannel)
            {
                var Channel = CommandMessage.Channel as IDMChannel;
                    if (Properties.Settings.Default.SendAction == "Edit")
                    {
                        await CommandMessage.ModifyAsync(x =>
                        {
                            x.Content = " ";
                            x.Embed = Embed;
                        });
                    }
                    else
                    {
                        await CommandMessage.DeleteAsync();
                        await Channel.SendMessageAsync("", false, Embed);
                    }
            }
            else
            {
                var Channel = CommandMessage.Channel as ITextChannel;
                IGuildUser GuildUser = CommandMessage.Author as IGuildUser;
                if (GuildUser.GetPermissions(Channel as ITextChannel).EmbedLinks || GuildUser.GuildPermissions.EmbedLinks)
                {
                    if (Properties.Settings.Default.SendAction == "Edit")
                    {
                        await CommandMessage.ModifyAsync(x =>
                        {
                            x.Content = " ";
                            x.Embed = Embed;
                        });
                    }
                    else
                    {
                        await CommandMessage.DeleteAsync();
                        await Channel.SendMessageAsync("", false, Embed);
                    }
                }
                else
                {
                    Program.SendMessage(CommandMessage, "No embed perms");
                }
            }
        }
        private async void Timer(object sender, ElapsedEventArgs e)
        {
            Uptime++;
            Properties.Settings.Default.TotalUptime++;
            Properties.Settings.Default.Save();
            if (Properties.Settings.Default.ANGuildsList.Count != 0)
            {
                    List<string> NickList = new List<string>();
                    var NicknamePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Discore-Selfbot\\Nicknames\\";
                    foreach (var Guild in Properties.Settings.Default.ANGuildsList)
                    {
                        NickList.Clear();
                        try
                        {
                            foreach (var Item in Directory.GetFiles(NicknamePath))
                            {
                                if (Item.StartsWith($"{NicknamePath + Guild}-"))
                                {
                                    NickList.Add(Item.Replace($"{NicknamePath + Guild}-", ""));
                                }
                            }
                            if (NickList.Count != 0)
                            {
                                int randomValue = Program.RandomGenerator.Next(0, NickList.Count);
                                var DGuild = client.GetGuild(Convert.ToUInt64(Guild));
                                var GuildUser = DGuild.GetUser(CurrentUserID);
                                string Nickname = NickList[randomValue];
                                await GuildUser.ModifyAsync(x => x.Nickname = Nickname);
                            }
                        }
                        catch
                        {

                        }
                    }
            }
        }
            public async Task InstallCommands()
        {
            client.MessageReceived += HandleCommand;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }
        public async Task HandleCommand(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            if (message.Author.Id == client.CurrentUser.Id)
            {
                MyForm.BtnSendActive.Enabled = true;
                if (message.Channel is IPrivateChannel)
                {
                    MyForm.ChannelList.Visible = false;
                    MyForm.AGName.Text = "DM";
                    MyForm.AGID.Text = $"(1)";
                    MyForm.ACName.Text = message.Channel.Name;
                    MyForm.ACID.Text = $"({message.Channel.Id})";
                    MyForm.BtnSendActive.Text = "Active DM";
                    ActiveGuildID = 0;
                    ActiveChannelID = message.Channel.Id;
                }
                else
                {
                    var GU = message.Author as IGuildUser;
                    MyForm.AGName.Text = GU.Guild.Name;
                    MyForm.AGID.Text = $"({GU.Guild.Id})";
                    MyForm.ACName.Text = message.Channel.Name;
                    MyForm.ACID.Text = $"({message.Channel.Id})";
                    MyForm.BtnSendActive.Text = "Active";
                    ActiveGuildID = GU.Guild.Id;
                    ActiveChannelID = message.Channel.Id;
                    if (GU.GuildPermissions.EmbedLinks == true)
                    {
                        MyForm.BtnSendActive.Text = "Active";
                    }
                    else
                    {
                        if (GU.GetPermissions(message.Channel as ITextChannel).EmbedLinks == true)
                        {
                            MyForm.BtnSendActive.Text = "Active";
                        }
                        else
                        {
                            MyForm.BtnSendActive.Text = "Active" + Environment.NewLine + "No perms";
                        }
                    }
                }
                int argPos = 0;
                if (!(message.HasStringPrefix("self ", ref argPos))) return;
                var context = new CommandContext(client, message);
                var result = await commands.ExecuteAsync(context, argPos, map);
                if (result.IsSuccess)
                {
                    Console.WriteLine($"Command > {message.Content}");
                }
            }
        }
    }
    public class InfoModule : ModuleBase
    {
        [Command("test")]
        public async Task test()
        {
            Program.SendMessage(Context.Message as IUserMessage, $"Hi {Program.CurrentUserName}");
        }
        [Command("neko")]
        public async Task neko()
        {
            var RandomValue = Program.RandomGenerator.Next(1, 11);
            var embed = new EmbedBuilder();
            if (GUI.EmbedColor.RawValue == 0)
            {
                embed.Color = Program.FavoriteColor;
            }
            else
            {
                embed.Color = GUI.EmbedColor;
            }
            switch (RandomValue)
            {
                case 1:
                    embed.ImageUrl = "https://em.wattpad.com/cfe2f4102b9bb5e0e32ad2ef4e6ad0edf906130c/687474703a2f2f666330382e64657669616e746172742e6e65742f667337302f662f323031312f3230322f332f332f615f6769726c5f6e656b6f5f62795f6d6f6b617468656865786769726c2d643431377432772e6a7067?s=fit&h=360&w=360&q=80";
                    break;
                case 2:
                    embed.ImageUrl = "https://68.media.tumblr.com/8dc8675a8d5a5a58fa224d0f13b3edd6/tumblr_oj1dtq9rMk1vwt3qvo1_500.jpg";
                    break;
                case 3:
                    embed.ImageUrl = "https://68.media.tumblr.com/5c472ee7d83552b5f65e9223810223de/tumblr_obgu5eQEEq1qjkxb4o1_500.png";
                    break;
                case 4:
                    embed.ImageUrl = "https://68.media.tumblr.com/4c43df58c426321ca6d5f3c80d76f141/tumblr_olh7x7zeJe1vwt3qvo1_500.jpg";
                    break;
                case 5:
                    embed.ImageUrl = "https://68.media.tumblr.com/d5bc9bb09cd2fac7f39f14c3a9254ab8/tumblr_ogkc06Fv531vbwt78o1_500.png";
                    break;
                case 6:
                    embed.ImageUrl = "https://68.media.tumblr.com/0211be68a458ef95a958918b0972973a/tumblr_o7m6s8GekH1vsna11o1_500.gif";
                    break;
                case 7:
                    embed.ImageUrl = "https://68.media.tumblr.com/2392325783e722994f418fdbfce2051d/tumblr_okym54bfdK1vwt3qvo1_500.png";
                    break;
                case 8:
                    embed.ImageUrl = "https://68.media.tumblr.com/37c749b7fcf43c33d7ed3e4f69c3e56a/tumblr_o80parZMmX1v61aw6o1_500.jpg";
                    break;
                case 9:
                    embed.ImageUrl = "https://68.media.tumblr.com/89854b3b3d572aa6380e0e811f8453a8/tumblr_okkhgbxPQ31vwt3qvo1_500.jpg";
                    break;
                case 10:
                    embed.ImageUrl = "https://68.media.tumblr.com/bdb7ad6e1b981ef67310402b0a107f8f/tumblr_o2ugsiwXDB1uwflhdo1_500.jpg";
                    break;

            }
            Program.SendEmbed(Context.Message as IUserMessage, embed);
        }

        [Command("clean")]
        public async Task clean(int Ammount)
        {
            int Count = Ammount;
            var Messages = await Context.Channel.GetMessagesAsync().Flatten();
            foreach(var Message in Messages)
            {
                if (Message.Author.Id == Context.Client.CurrentUser.Id)
                {
                    if (Count != 0)
                    {
                        await Message.DeleteAsync();
                        Count--;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        [Command("guild")]
        public async Task guild()
        {
            if (Context.Channel is IPrivateChannel)
            {
                await Context.Message.Channel.SendMessageAsync("`Selfbot | Cannot use command in private channel`");
                return;
            }
            int Members = 0;
            int Bots = 0;
            int MembersOnline = 0;
            int BotsOnline = 0;
            IGuildUser Owner = await Context.Guild.GetOwnerAsync();
            foreach (var User in await Context.Guild.GetUsersAsync())
            {
                if (User.IsBot)
                {
                    if (User.Status == UserStatus.Online || User.Status == UserStatus.Idle || User.Status == UserStatus.AFK || User.Status == UserStatus.DoNotDisturb)
                    {
                        BotsOnline++;
                    }
                    else
                    {
                        Bots++;
                    }
                }
                else
                {
                    if (User.Status == UserStatus.Online || User.Status == UserStatus.Idle || User.Status == UserStatus.AFK || User.Status == UserStatus.DoNotDisturb)
                    {
                        MembersOnline++;
                    }
                    else
                    {
                        Members++;
                    }
                }
            }
            int TextChan = 0;
            int VoiceChan = 0;
            foreach (var Channel in await Context.Guild.GetChannelsAsync())
            {
                if (Channel is ITextChannel)
                {
                    TextChan++;
                }
                else
                {
                    VoiceChan++;
                }
            }
            var embed = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder()
                {
                    Name = $"{Context.Guild.Name}"
                },
                ThumbnailUrl = Context.Guild.IconUrl,
                Description = $"Owner: {Owner.Mention}```md" + Environment.NewLine + $"[Online](Offline)" + Environment.NewLine + $"<Users> [{MembersOnline}]({Members}) <Bots> [{BotsOnline}]({Bots})" + Environment.NewLine + $"Channels <Text {TextChan}> <Voice {VoiceChan}>" + Environment.NewLine + $"<Roles {Context.Guild.Roles.Count}> <CustomEmojis {Context.Guild.Emojis.Count}> <Region {Context.Guild.VoiceRegionId}>```",
                Footer = new EmbedFooterBuilder()
                {
                    Text = $"Created {Context.Guild.CreatedAt.Date.Day} {Context.Guild.CreatedAt.Date.DayOfWeek} {Context.Guild.CreatedAt.Year}"
                }
            };
            if (GUI.EmbedColor.RawValue == 0)
            {
                embed.Color = Program.FavoriteColor;
            }
            else
            {
                embed.Color = GUI.EmbedColor;
            }
            Program.SendEmbed(Context.Message as IUserMessage, embed);
        }

        [Command("ping")]
        public async Task ping(string IP = "")
        {
            if (IP == "")
            {
                System.Net.NetworkInformation.PingReply PingDiscord = new System.Net.NetworkInformation.Ping().Send("discordapp.com");
                System.Net.NetworkInformation.PingReply PingGoogle = new System.Net.NetworkInformation.Ping().Send("google.com");
                Program.SendMessage(Context.Message as IUserMessage, $"PONG > Discord: {PingDiscord.RoundtripTime} MS Google: {PingGoogle.RoundtripTime} MS Gateway: " + Program.client.Latency + " MS");
            }
            else
            {
                System.Net.NetworkInformation.PingReply Ping = new System.Net.NetworkInformation.Ping().Send("discordapp.com");
                Program.SendMessage(Context.Message as IUserMessage, $"PONG > {IP}: {Ping.RoundtripTime} MS");
            }
        }

        [Command("uptime")]
        public async Task uptime()
        {
            Program.SendMessage(Context.Message, $"Uptime {Program.Uptime} minutes | TotalUptime {Properties.Settings.Default.TotalUptime} minutes | TotalRuns {Properties.Settings.Default.TotalRuns}");
        }

        [Command("calc")]
        public async Task calc(int Num1, string Func, int Num2)
        {
            string Message = "";
            int Result = 0;
            switch (Func)
            {
                case "+":
                    Result = Num1 + Num2;
                    Message = $"{Num1} {Func} {Num2} = {Result}";
                    break;
                case "-":
                    Result = Num1 - Num2;
                    Message = $"{Num1} {Func} {Num2} = {Result}";
                    break;
                case "*":
                    Result = Num1 * Num2;
                    Message = $"{Num1} {Func} {Num2} = {Result}";
                    break;
                case "x":
                    Result = Num1 * Num2;
                    Message = $"{Num1} {Func} {Num2} = {Result}";
                    break;
                case "/":
                    Result = Num1 / Num2;
                    Message = $"{Num1} {Func} {Num2} = {Result}";
                    break;
                default:
                    Message = "Unknown Function Use | + - * /";
                    break;
            }
            Program.SendMessage(Context.Message as IUserMessage, Message);
        }
        [Command("info")]
        public async Task info()
        {
            var Guilds = await Context.Client.GetGuildsAsync();
            var embed = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder()
                {
                    Name = $"Selfbot | {Context.Client.CurrentUser.Username}",
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Url = Context.Client.CurrentUser.GetAvatarUrl()
                },
                Description = $"```md" + Environment.NewLine + $"<Guilds {Guilds.Count()}> <Created {Context.Client.CurrentUser.CreatedAt.Date.ToShortDateString()}> <ID {Context.Client.CurrentUser.Id}>" + Environment.NewLine + $"<SelfbotUptime {Program.Uptime} minutes>```",
            };
            if (GUI.EmbedColor.RawValue == 0)
            {
                embed.Color = Program.FavoriteColor;
            }
            else
            {
                embed.Color = GUI.EmbedColor;
            }
            Program.SendEmbed(Context.Message as IUserMessage, embed);
        }

        [Command("cleanembed")]
        [Alias("cleanembeds")]
        public async Task cleanembed()
        {
            await Context.Message.DeleteAsync();
            var Messages = await Context.Channel.GetMessagesAsync().Flatten();
            foreach (var Message in Messages)
            {
                if (Message.Author.Id == Context.Client.CurrentUser.Id)
                {
                    if (Message.Embeds.Count == 1)
                    {
                        await Message.DeleteAsync();
                    }
                }
            }
        }

        [Command("form")]
        [Alias("gui")]
        public async Task form()
        {
            await Context.Message.DeleteAsync();
            if (!Program.MyForm.Visible)
            {
                Console.WriteLine("Opening gui");
                GUI.SelectedGuild = 0;
                GUI.SelectChannel = 0;
                Program.OpenGUI();
                Program.MyForm.Activate();
            }
            else
            {
                Program.MyForm.Activate();
                Console.WriteLine("Gui already open");
            }
        }

        [Command("embed")]
        public async Task embed([Remainder] string Text)
        {
            var embed = new EmbedBuilder()
            {
                Description = Text,
            };
            if (GUI.EmbedColor.RawValue == 0)
            {
                embed.Color = Program.FavoriteColor;
            }
            else
            {
                embed.Color = GUI.EmbedColor;
            }
            Program.SendEmbed(Context.Message, embed);
        }

        [Command("bot")]
        [Alias("botinfo")]
        public async Task botinfo()
        {
            var embed = new EmbedBuilder()
            {
                Title = "Discore Selfbot Info",
                Description = $"Selfbot made by xXBuilderBXx#9113 [Github](https://github.com/ArchboxDev/Discore-Selfbot)",
            };
            if (GUI.EmbedColor.RawValue == 0)
            {
                embed.Color = Program.FavoriteColor;
            }
            else
            {
                embed.Color = GUI.EmbedColor;
            }
            Program.SendEmbed(Context.Message, embed);
        }

        [Command("lenny")]
        public async Task lenny()
        {
            var CommandMessage = Context.Message as IUserMessage;
            if (Properties.Settings.Default.SendAction == "Edit")
            {
                await CommandMessage.ModifyAsync(x =>
                {
                    x.Content = "( ͡° ͜ʖ ͡°)";
                });
            }
            else
            {
                await CommandMessage.DeleteAsync();
                await Context.Channel.SendMessageAsync("( ͡° ͜ʖ ͡°)");
            }
        }

        [Command("lewd")]
        public async Task lewd([Remainder] string Text)
        {
            var embed = new EmbedBuilder()
            {
                Description = "LEWD",
                Color = new Discord.Color(255, 20, 147),
                ThumbnailUrl = "https://encrypted-tbn1.gstatic.com/images?q=tbn:ANd9GcRM7wR508Do1SR7I-kJACZtjyb4vCXX_N5ftE4PbSC5ptNheXi1"
            };
            if (Text.Contains("is") || Text.Contains("are") || Text.Contains("is lewd") || Text.Contains("are lewd"))
            {
                embed.Description = Text;
                if (Text.EndsWith("is") || Text.EndsWith("are"))
                {
                    embed.Description = Text + " LEWD";
                }
            }
            else
            if (Text.Any())
            {
                embed.Description = "LEWD " + Text;
            }
            Program.SendEmbed(Context.Message, embed);
        }

        [Command("user")]
        public async Task user(string ID)
        {
            if (Context.IsPrivate)
            {
                await Context.Message.Channel.SendMessageAsync("`Selfbot | Cannot use command in private channel`");
                return;
            }
            string User = ID;
            if (User.StartsWith("<@"))
            {
                User = User.Replace("<@", "").Replace(">", "");
                if (User.StartsWith("!"))
                {
                    User = User.Replace("!", "");
                }
            }
            try
            {
                var GuildUser = await Context.Guild.GetUserAsync(Convert.ToUInt64(User));
                int Count = 0;
                foreach(var Guild in await Context.Client.GetGuildsAsync())
                {
                    foreach (var Member in await Guild.GetUsersAsync())
                    {
                        if (Member.Id == GuildUser.Id)
                        {
                            Count++;
                        }
                    }
                }
                var embed = new EmbedBuilder()
                {
                    Author = new EmbedAuthorBuilder()
                    {
                        Name = $"{GuildUser.Username}#{GuildUser.Discriminator}",
                        IconUrl = GuildUser.GetAvatarUrl()
                    },
                    Description = $"{GuildUser.Mention} - {GuildUser.Id}" + Environment.NewLine + $"Created {GuildUser.CreatedAt.Date.ToShortDateString()} | Joined Guild {GuildUser.JoinedAt.Value.Date.ToShortDateString()}" + Environment.NewLine + $"I am in {Count} Guilds with {Context.Message.Author.Username}",
                    Url = GuildUser.GetAvatarUrl()
                };
                if (GUI.EmbedColor.RawValue == 0)
                {
                    embed.Color = Program.FavoriteColor;
                }
                else
                {
                    embed.Color = GUI.EmbedColor;
                }
                Program.SendEmbed(Context.Message, embed);
            }
            catch
            {
                Program.SendMessage(Context.Message as IUserMessage, "Could not find user");
            }
        }

        [Command("find")]
        public async Task find(string ID)
        {
            int GuildCount = 0;
            Console.WriteLine("----- Guilds Found -----");
            foreach(var Guild in await Context.Client.GetGuildsAsync())
            {
                foreach(var User in await Guild.GetUsersAsync())
                {
                    if (User.Id.ToString() == ID)
                    {
                        Console.WriteLine(Guild.Name);
                        GuildCount++;
                    }
                }
            }
            Console.WriteLine("----- ----- -----");
            Program.SendMessage(Context.Message as IUserMessage, $"Found {ID} in {GuildCount} guilds check console for names");
        }

        [Command("tag")]
        public async Task tag([Remainder] string Tag) 
        {
            IGuildUser GuildUser = null;
            bool AllowedEmbeds = false;
            if (Context.Channel is IPrivateChannel)
            {
                AllowedEmbeds = true;
            }
            else
            {
                GuildUser = Context.Message.Author as IGuildUser;
                AllowedEmbeds = GuildUser.GetPermissions(Context.Channel as ITextChannel).EmbedLinks;
            }
            var TagPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Discore-Selfbot\\Tags\\";
            if (File.Exists(TagPath + Tag + ".txt"))
            {
                string TagMention = "";
                string TagAuthor = "";
                string TagContent = "";
                using (Stream stream = File.Open(TagPath + Tag + ".txt", FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        TagMention = reader.ReadLine();
                        TagAuthor = reader.ReadLine();
                        TagContent = reader.ReadLine();
                    }
                }
                if (AllowedEmbeds == true)
                {
                    string TagThumbnail = "";
                    if (File.Exists(TagPath + Tag + "-thumbnail.txt"))
                    {
                        TagThumbnail = File.ReadAllText(TagPath + Tag + "-thumbnail.txt");
                    }
                    var embed = new EmbedBuilder()
                    {
                        Title = $"Selfbot Tag | {Tag}",
                        Description = $"{TagMention} {TagAuthor}" + Environment.NewLine + TagContent,
                        ThumbnailUrl = TagThumbnail,
                        Url = TagThumbnail
                    };
                    if (GUI.EmbedColor.RawValue == 0)
                    {
                        embed.Color = Program.FavoriteColor;
                    }
                    else
                    {
                        embed.Color = GUI.EmbedColor;
                    }
                    Program.SendEmbed(Context.Message, embed);
                }
                else
                {
                    Program.SendMessage(Context.Message, TagAuthor + Environment.NewLine + TagContent);
                }
            }
            else
            {
                Program.SendMessage(Context.Message as IUserMessage, $"Tag {Tag} not found");
            }
        }

        [Command("addtag")]
        public async Task addtag(string Tag = "", string MessageID = "")
        {
            await Context.Message.ModifyAsync(x =>
            {
                x.Content = "`Please Wait`";
            });
            string TagMention = "";
            string TagAuthor = "";
            string TagContent = "";
            string TagImage = "";
            var TagPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Discore-Selfbot\\Tags\\";
            bool Numeric = MessageID.All(char.IsDigit);
            if (Numeric == true)
            {
                foreach (var Message in await Context.Channel.GetMessagesAsync().Flatten())
                {
                    if (Message.Id.ToString() == MessageID)
                    {
                        TagMention = Message.Author.Mention;
                        TagAuthor = $"{Message.Author.Username}#{Message.Author.Discriminator} said";
                        TagContent = Message.Content;
                        if (Message.Attachments.Count == 1)
                        {
                            foreach(var Attachment in Message.Attachments)
                            {
                                TagImage = Attachment.Url;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                TagMention = Context.Message.Author.Mention;
                TagAuthor = $"{Context.Message.Author.Username}#{Context.Message.Author.Discriminator} said";
                TagContent = MessageID;
            }
            if (TagContent == "")
            {
                Program.SendMessage(Context.Message as IUserMessage, "Tag content not set or found");
                return;
            }
            if (File.Exists(TagPath + Tag + ".txt"))
            {
                Program.SendMessage(Context.Message as IUserMessage, $"Tag {Tag} already exists");
            }
            else
            {
                using (StreamWriter sw = File.CreateText(TagPath + Tag + ".txt"))
                {
                    sw.WriteLine(TagMention);
                    sw.WriteLine(TagAuthor);
                    sw.WriteLine(TagContent);
                }
                if (TagImage == "")
                {
                    Program.SendMessage(Context.Message as IUserMessage, $"Tag {Tag} created");
                }
                else
                {
                    using (StreamWriter sw = File.CreateText(TagPath + Tag + "-thumbnail.txt"))
                    {
                        sw.WriteLine(TagImage);
                    }
                    Program.SendMessage(Context.Message as IUserMessage, $"Tag {Tag} created with an image");
                }
            }
        }

        [Command("deltag")]
        public async Task deltag([Remainder] string Tag)
        {
            if (Tag.Contains("-thumbnail"))
            {
                return;
            }
            var TagPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Discore-Selfbot\\Tags\\";
            if (File.Exists(TagPath + Tag + ".txt"))
            {
                if (File.Exists(TagPath + Tag + "-thumbnail.txt"))
                {
                    File.Delete(TagPath + Tag + "-thumbnail.txt");
                }
                File.Delete(TagPath + Tag + ".txt");
                Program.SendMessage(Context.Message as IUserMessage, $"Tag {Tag} deleted");
            }
            else
            {
                Program.SendMessage(Context.Message as IUserMessage, $"Tag {Tag} not found");
            }
        }

        [Command("an bind")]
        public async Task anbind()
        {
            if (Context.Channel is IPrivateChannel)
            {
                await Context.Message.Channel.SendMessageAsync("`Selfbot | Cannot use command in private channel`");
                return;
            }
            var GuildUser = Context.User as IGuildUser;
            string Message = "";
            if (GuildUser.GuildPermissions.ChangeNickname == false)
            {
                Message = "You do not have perms for change nickname";
            }
            else
            {
                if (Properties.Settings.Default.ANGuildsList.Count == 5)
                {
                    Message = "5 Guilds is the max limit for auto nicknames";
                }
                else
                {
                    if (Properties.Settings.Default.ANGuildsList.Contains(Context.Guild.Id.ToString()))
                    {
                        Message = "Guild removed from auto nickname list";
                        Properties.Settings.Default.ANGuildsList.Remove(Context.Guild.Id.ToString());
                    }
                    else
                    {
                        Message = "Guild added to auto nickname list";
                        Properties.Settings.Default.ANGuildsList.Add(Context.Guild.Id.ToString());
                    }
                    Properties.Settings.Default.Save();
                }
            }
            Program.SendMessage(Context.Message as IUserMessage, Message);
        }

        [Command("an add")]
        public async Task anadd([Remainder] string Nickname)
        {
            if (Context.Channel is IPrivateChannel)
            {
                await Context.Message.Channel.SendMessageAsync("`Selfbot | Cannot use command in private channel`");
                return;
            }
            if (!Properties.Settings.Default.ANGuildsList.Contains(Context.Guild.Id.ToString()))
            {
                Program.SendMessage(Context.Message as IUserMessage, $"This guild is not in the auto nickname list use | self an bind");
                return;
            }
            var NicknamePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Discore-Selfbot\\Nicknames\\";
            if (File.Exists(NicknamePath + Context.Guild.Id + "-" + Nickname))
            {
                Program.SendMessage(Context.Message as IUserMessage, $"{Nickname} already exists");
            }
            else
            {
                File.Create(NicknamePath + Context.Guild.Id + "-" + Nickname);
                Program.SendMessage(Context.Message as IUserMessage, $"{Nickname} added to auto nickname list");
            }
        }

        [Command("an del")]
        public async Task andel(string Nickname)
        {
            if (Context.Channel is IPrivateChannel)
            {
                await Context.Message.Channel.SendMessageAsync("`Selfbot | Cannot use command in private channel`");
                return;
            }
            if (!Properties.Settings.Default.ANGuildsList.Contains(Context.Guild.Id.ToString()))
            {
                Program.SendMessage(Context.Message as IUserMessage, $"This guild is not in the auto nickname list use | self an bind");
                return;
            }
            var NicknamePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Discore-Selfbot\\Nicknames\\";
            if (File.Exists(NicknamePath + Context.Guild.Id + "-" + Nickname))
            {
                File.Delete(NicknamePath + Context.Guild.Id + "-" + Nickname);
                
                Program.SendMessage(Context.Message as IUserMessage, $"{Nickname} deleted");
            }
            else
            {
                Program.SendMessage(Context.Message as IUserMessage, $"{Nickname} does not exists");
            }
        }

        [Command("an list")]
        public async Task anlist()
        {
            if (Context.Channel is IPrivateChannel)
            {
                await Context.Message.Channel.SendMessageAsync("`Selfbot | Cannot use command in private channel`");
                return;
            }
            List<string> ANList = new List<string>();
            var NicknamePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Discore-Selfbot\\Nicknames\\";
            foreach (var Item in Directory.GetFiles(NicknamePath))
            {
                if (Item.StartsWith($"{NicknamePath + Context.Guild.Id}-"))
                {
                    ANList.Add(Item.Replace($"{NicknamePath + Context.Guild.Id}-", ""));
                }
            }
            if (ANList.Count == 0)
            {
                Program.SendMessage(Context.Message as IUserMessage, "This guild does not have any nicknames set");
                return;
            }
            string NicknameList = string.Join(" | ", ANList.ToArray());
            if (Properties.Settings.Default.SendAction == "Edit")
            {
                var M = Context.Message as IUserMessage;
                await M.ModifyAsync(x =>
                {
                    x.Content = "```List of auto nicknames" + Environment.NewLine + NicknameList + "```";
                });
            }
            else
            {
                await Context.Message.DeleteAsync();
                await Context.Message.Channel.SendMessageAsync("```List of auto nicknames" + Environment.NewLine + NicknameList + "```");
            }
        }

        [Command("tags")]
        public async Task tags()
        {
            List<string> TagList = new List<string>();
            var TagPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Discore-Selfbot\\Tags\\";
            foreach (var File in Directory.GetFiles(TagPath))
            {
                if (!File.Contains("-thumbnail"))
                {
                    TagList.Add(File.Replace(TagPath, "").Replace(".txt", ""));
                }
            }
            string Tags = string.Join(" | ", TagList.ToArray());
            if (Properties.Settings.Default.SendAction == "Edit")
            {
                var M = Context.Message as IUserMessage;
                await M.ModifyAsync(x =>
                {
                    x.Content = $"Selfbot Tags" + Environment.NewLine + Tags;
                });
            }
            else
            {
                await Context.Message.DeleteAsync();
                await Context.Message.Channel.SendMessageAsync($"```--- Selfbot Tags ---" + Environment.NewLine + Tags + "```");
            }
        }
    }
}
