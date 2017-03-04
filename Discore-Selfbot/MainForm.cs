﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Discord;
using System.Net;

namespace Discore_Selfbot
{
    public partial class MainForm : Form
    {
        public static Discord.Color EmbedColor;
        public static ulong SelectedGuild = 0;
        public static ulong SelectChannel = 0;
        public static ulong ActiveGuildID = 0;
        public static ulong ActiveChannelID = 0;
        public string LastEmbedTitle = "";
        public string LastEmbedText = "";
        
        public MainForm()
        {
            InitializeComponent();
            
        }
        public async void MainForm_Load(object sender, EventArgs e)
        {
            if (Program.ConnectedOnce == false)
            {
                if (Properties.Settings.Default.AutoForm == "No")
                {
                    return;
                }
            }
            WebClient WBC = new WebClient();
                Program.Guilds.Clear();
                Program.GuildsID.Clear();
            this.Text = Program.DiscordUser;
                if (System.IO.File.Exists($"avatar.png"))
                {
                    Bitmap b = (Bitmap)System.Drawing.Image.FromFile($"avatar.png");
                    IntPtr pIcon = b.GetHicon();
                    Icon i = Icon.FromHandle(pIcon);
                    i.Dispose();
                    this.Icon = i;
                }
                foreach (var Guild in Program.client.Guilds)
                {
                    if (!System.IO.File.Exists($"{Guild.Id}.png"))
                    {
                        if (Guild.IconUrl == null)
                        {
                            WBC.DownloadFile("http://dev.blaze.ml/Letters/" + Guild.Name.ToUpper().ToCharArray()[0] + ".png", $"{Guild.Id}.png");
                        }
                        else
                        {
                            WBC.DownloadFile(Guild.IconUrl, $"{Guild.Id}.png");
                        }
                        WBC.Dispose();
                    }
                    this.ListGuild.Items.Add(Guild.Name, System.Drawing.Image.FromFile($"{Guild.Id}.png")).DisplayStyle = ToolStripItemDisplayStyle.Image;
                    Program.Guilds.Add(Guild.Name);
                    Program.GuildsID.Add(Guild.Id);
                }
        }
        private void SelectedChannelClick(object sender, EventArgs e)
        {
            if (SelectedGuild == 0)
            {
                MessageBox.Show("No guild selected");
                return;
            }
            if (SelectChannel == 0)
            {
                MessageBox.Show("No channel selected");
                return;
            }
            if (EmbedTitle.Text == LastEmbedTitle)
            {
                if (EmbedText.Text == LastEmbedText)
                {
                    MessageBox.Show("You already send this message");
                    return;
                }
            }
            LastEmbedTitle = EmbedTitle.Text;
            LastEmbedText = EmbedText.Text;
            var Guild = Program.client.GetGuild(SelectedGuild);
            var Chan = Guild.GetChannel(SelectChannel) as ITextChannel;
            var embed = new EmbedBuilder()
            {
                Title = EmbedTitle.Text,
                Description = EmbedText.Text,
                Color = EmbedColor
            };
            Chan.SendMessageAsync("", false, embed);
        }

        private void EmbedActive_Click(object sender, EventArgs e)
        {
            if (ActiveGuildID == 0)
            {
                MessageBox.Show("No guild selected");
                return;
            }
            if (ActiveChannelID == 0)
            {
                MessageBox.Show("No channel selected");
                return;
            }
            if (EmbedTitle.Text == LastEmbedTitle)
            {
                if (EmbedText.Text == LastEmbedText)
                {
                    MessageBox.Show("You already send this message");
                    return;
                }
            }
            LastEmbedTitle = EmbedTitle.Text;
            LastEmbedText = EmbedText.Text;
            var Guild = Program.client.GetGuild(ActiveGuildID);
            var Chan = Guild.GetChannel(ActiveChannelID) as ITextChannel;
            var embed = new EmbedBuilder()
            {
                Title = EmbedTitle.Text,
                Description = EmbedText.Text,
                Color = EmbedColor
            };
            Chan.SendMessageAsync("", false, embed);
        }

        private void GuildList_Clicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Console.WriteLine($"Selected Guild {e.ClickedItem.ToolTipText}");
            Text = Program.client.CurrentUser.Username + " - " + e.ClickedItem.Text;
            var GuildIndex = ListGuild.Items.IndexOf(e.ClickedItem);
            var Guild = Program.client.GetGuild(Program.GuildsID[GuildIndex]);
            TextGuildInfo.Text = $"ID: {Guild.Id}" + Environment.NewLine + $"Owner: {Guild.Owner.Username} - {Guild.Owner.Id}" + Environment.NewLine + $"Users: {Guild.Users.Where(x => !x.IsBot).Count()} Bots: {Guild.Users.Where(x => x.IsBot).Count()}" + Environment.NewLine + $"Roles: {Guild.Roles.Count - 1} Emojis: {Guild.Emojis.Count}" + Environment.NewLine + $"Created: {Guild.CreatedAt.Date.ToShortDateString()}";
            SelectedGuild = Program.GuildsID[GuildIndex];
            ListChannel.Items.Clear();
            ListChannel.Visible = true;
            Program.Channels.Clear();
            Program.ChannelsID.Clear();
            foreach (var Chan in Guild.TextChannels)
            {
                Program.Channels.Add(Chan.Name);
                Program.ChannelsID.Add(Chan.Id);
                ListChannel.Items.Add($"{Chan.Name}");
            }
            TextGuildRoles.Clear();
            List<string> RoleList = new List<string>();
            foreach (var Role in Guild.Roles)
            {
                if (Role != Guild.EveryoneRole)
                {
                    AppendText(TextGuildRoles, Role.Name + Environment.NewLine, System.Drawing.Color.FromArgb(Role.Color.R,Role.Color.G,Role.Color.B));
                    AppendText(TextGuildRoles, Role.Id + Environment.NewLine + Environment.NewLine, System.Drawing.Color.FromArgb(0, 0, 0));
                }
            }
            List<string> EmojiList = new List<string>();
            foreach (var Emoji in Guild.Emojis)
            {
                EmojiList.Add(Emoji.Name);
            }
            TextGuildEmoji.Text = string.Join(Environment.NewLine, EmojiList.ToArray());
        }

        private void ChannelSelected(object sender, EventArgs e)
        {
            Console.WriteLine($"Selected Channel {ListChannel.SelectedText}");
            var Index = ListChannel.SelectedIndex;
            SelectChannel = Program.ChannelsID[Index];
        }

        private void OpenColorsButton_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog1 = new ColorDialog();

            if (colorDialog1.ShowDialog() == DialogResult.OK)

            {
                EmbedColor = new Discord.Color(colorDialog1.Color.R, colorDialog1.Color.G, colorDialog1.Color.B);
                OpenColorsButton.ForeColor = colorDialog1.Color;

            }
        }

        private void ButtonBotWebsite_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(ButtonBotWebsite.AccessibleDescription);
        }

        private void ButtonBotInvite_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(ButtonBotInvite.AccessibleDescription);
        }

        private void ViewBots_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Name == "PixelBot")
            {
                TextBotInfo.Text = "A gamer featured bot with commands for steam/osu/minecraft and twitch streamer alerts";
                ButtonBotWebsite.AccessibleDescription = "http://dev.blaze.ml";
                ButtonBotInvite.AccessibleDescription = "https://discordapp.com/oauth2/authorize?&client_id=277933222015401985&scope=bot&permissions=0";
                ButtonBotWebsite.Visible = true;
                ButtonBotInvite.Visible = true;
            }
            if (e.Node.Name == "Minotaur")
            {
                TextBotInfo.Text = "A guild moderation bot with ban/kick/mute commands and advanced logging/userlogs/modlogs";
                ButtonBotWebsite.AccessibleDescription = "http://dev.blaze.ml";
                ButtonBotInvite.AccessibleDescription = "https://discordapp.com/oauth2/authorize?&client_id=281849383404830733&scope=bot&permissions=0";
                ButtonBotWebsite.Visible = true;
                ButtonBotInvite.Visible = true;
            }
            if (e.Node.Name == "Discord Cards")
            {
                TextBotInfo.Text = "Buy/Trade/Collect all of the rare cards featured around discord";
                ButtonBotWebsite.AccessibleDescription = "";
                ButtonBotInvite.AccessibleDescription = "https://discordapp.com/oauth2/authorize?client_id=275388037817696287&scope=bot";
                ButtonBotWebsite.Visible = false;
                ButtonBotInvite.Visible = true;
            }
            if (e.Node.Name == "Casino Bot")
            {
                TextBotInfo.Text = "Spin the wheel and get the JACKPOT!";
                ButtonBotWebsite.AccessibleDescription = "http://dev.blaze.ml";
                ButtonBotInvite.AccessibleDescription = "https://discordapp.com/oauth2/authorize?client_id=263330369409908736&scope=bot&permissions=19456";
                ButtonBotWebsite.Visible = false;
                ButtonBotInvite.Visible = true;
            }
            if (e.Node.Name == "Discord RPG")
            {
                TextBotInfo.Text = "Who dosent love a good RPG bot?";
                ButtonBotWebsite.AccessibleDescription = "";
                ButtonBotInvite.AccessibleDescription = "https://discordapp.com/oauth2/authorize?&client_id=170915256833540097&scope=bot&permissions=0";
                ButtonBotWebsite.Visible = false;
                ButtonBotInvite.Visible = true;
            }
            if (e.Node.Name == "Sekusuikuto")
            {
                TextBotInfo.Text = "Currently down and also the website so i cannot add :P";
                ButtonBotWebsite.AccessibleDescription = "";
                ButtonBotInvite.AccessibleDescription = "";
                ButtonBotWebsite.Visible = false;
                ButtonBotInvite.Visible = false;
            }
        }

        private void BtnSetEdit_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.SendAction = "Edit";
            Properties.Settings.Default.Save();
        }

        private void BtnSetDelete_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.SendAction = "Delete";
            Properties.Settings.Default.Save();
        }

        private void BtnSetFormYes_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoForm = "Yes";
            Properties.Settings.Default.Save();
        }

        private void BtnSetFormNo_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoForm = "No";
            Properties.Settings.Default.Save();
        }
        
        private void ViewRoles_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            MessageBox.Show(e.Node.Tag.ToString());
        }

        private void ThemeSkype_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/SnazzyPine25/BeautifulDiscordThemes");
        }

        private void ThemeAlexflipnote_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/AlexFlipnote/Discord_Theme");
        }

        private void ThemeOther1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/beautiful-discord-community/resources/");
        }

        private void ThemeOther2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Jiiks/BetterDiscordApp/wiki/Themes");
        }
        public void AppendText(RichTextBox box, string text, System.Drawing.Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            //box.SelectionFont = font;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }
}
