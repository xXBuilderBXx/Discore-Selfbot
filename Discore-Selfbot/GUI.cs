﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using System.Net;
using System.IO;
using Discord;

namespace Discore_Selfbot
{
    public partial class GUI : KryptonForm
    {
        public static Discord.Color EmbedColor;
        public bool EmbedFirstClick = false;
        public static ulong SelectedGuild = 0;
        public static ulong SelectedChannel = 0;
        public string LastEmbedTitle = "";
        public string LastEmbedText = "";
        public static EmbedPopup FormEmbedPopup;
        public GUI()
        {
            InitializeComponent();
            this.GUI_Minimize.Click += BtnTopMin_Click;
            this.GUI_OnTop.Click += BtnOnTop_Click;
            this.GUI_NavMain.SelectedPageChanged += NavInfo_SelectedPageChanged;
            this.GUI_LinkWebsite.Click += LinkWebsite_Click;
            this.GUI_LinkGithub.Click += LinkGithub_Click;
            this.GUI_LinkMyGuild.Click += LinkMyGuild_Click;
        }
        public void UpdateActive(string Guild = "MyGuild", string Channel = "MyGuild")
        {
                GUI_ActiveGuildName.Text = Guild;
                GUI_ActiveChannelName.Text = Channel;
                if (FormEmbedPopup != null)
                {
                    FormEmbedPopup.ActiveGuild.Text = Guild;
                    FormEmbedPopup.ActiveChannel.Text = Channel;
                }
        }

        private void LinkMyGuild_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://discord.gg/WJTYdNb");
        }

        private void LinkGithub_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/ArchboxDev/Discore-Selfbot");
        }

        private void LinkWebsite_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://blaze.ml/");
        }

        private void NavInfo_SelectedPageChanged(object sender, EventArgs e)
        {
           if (GUI_NavMain.SelectedPage.Name == "NavCustomPage")
            {
                GUI_CCList.Items.Clear();
                foreach (var File in Directory.GetFiles(Program.SelfbotDir + "Custom\\"))
                {
                    if (!File.Contains(".message"))
                    {
                        GUI_CCList.Items.Add(File.Replace(Program.SelfbotDir + "Custom\\", "").Replace(".txt", ""));
                    }
                }
            }
        }

        private void BtnTopMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void BtnOnTop_Click(object sender, EventArgs e)
        {
            if (GUI_OnTop.Checked == ButtonCheckState.Unchecked)
            {
                this.TopMost = false;
            }
            else
            {
                this.TopMost = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Microsoft.Win32.RegistryKey rkApp = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (rkApp.GetValue("Discore-Selfbot") != null)
            {
                GUI_AutoStartText.Text = "Yes";
            }
            GUI_FavColor.StateNormal.Back.Color1 = Properties.Settings.Default.FavoriteColor;
            GUI_FavColor.OverrideDefault.Back.Color1 = Properties.Settings.Default.FavoriteColor;
            if (Properties.Settings.Default.Theme == "Dark")
            {
                GUI_ThemeManager.GlobalPaletteMode = PaletteModeManager.Office2010Black;
            }
            if (Properties.Settings.Default.Theme == "Dark Sparkle")
            {
                GUI_ThemeManager.GlobalPaletteMode = PaletteModeManager.SparkleBlue;
            }
            
            if (Program.Ready == false)
            {
                return;
            }
            GUI_ActiveGuildName.Text = Program.ActiveGuild;
            GUI_ActiveChannelName.Text = Program.ActiveChannel;
            foreach(var Item in Program.MentionLog)
            {
                GUI_Mentions.Items.Add(Item);
            }
            
            WebClient WBC = new WebClient();
            Program.GuildIDs.Clear();
            foreach (var Guild in Program.client.Guilds)
            {
                Program.GuildIDs.Add(Guild.Id);
                if (Guild.IconUrl == null)
                {
                    var GuildNameFormat = new String(Guild.Name.Where(Char.IsLetter).ToArray());
                    using (Stream ImageStream = WBC.OpenRead("http://dev.blaze.ml/Letters/" + GuildNameFormat.ToCharArray()[0] + ".png"))
                    {
                        Bitmap Image = new Bitmap(ImageStream);
                        ToolStripButton GuildButton = new ToolStripButton(Guild.Name, Image);
                        GuildButton.AccessibleDescription = Guild.Id.ToString();
                        GuildButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
                        if (Guild.OwnerId == Program.client.CurrentUser.Id)
                        {
                            using (Graphics Grap = Graphics.FromImage(Image))
                            {
                                Grap.DrawRectangle(new Pen(Brushes.Gold, 10), new Rectangle(0, 0, Image.Width, Image.Height));
                            }
                            GuildButton.Image = Image;
                            GUI_Guilds.Items.Insert(0, (GuildButton));
                        }
                        else
                        {
                            GUI_Guilds.Items.Add(GuildButton);
                        }
                    }
                }
                else
                {
                    using (Stream ImageStream = WBC.OpenRead(Guild.IconUrl))
                    {
                        Bitmap Image = new Bitmap(ImageStream);
                        ToolStripButton GuildButton = new ToolStripButton(Guild.Name, Image);
                        GuildButton.AccessibleDescription = Guild.Id.ToString();
                        GuildButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
                        if (Guild.OwnerId == Program.client.CurrentUser.Id)
                        {
                            using (Graphics Grap = Graphics.FromImage(Image))
                            {
                                Grap.DrawRectangle(new Pen(Brushes.Gold, 10), new Rectangle(0, 0, Image.Width, Image.Height));
                            }
                            GuildButton.Image = Image;
                            GUI_Guilds.Items.Insert(0, (GuildButton));
                        }
                        else
                        {
                            GUI_Guilds.Items.Add(GuildButton);
                        }
                    }
                }
            }
            this.Text = Program.CurrentUserName;
            using (Stream ImageStream = WBC.OpenRead(Program.client.CurrentUser.GetAvatarUrl()))
            {
                Bitmap b = (Bitmap)System.Drawing.Image.FromStream(ImageStream);
                IntPtr pIcon = b.GetHicon();
                Icon i = Icon.FromHandle(pIcon);
                this.Icon = i;
            }
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

        private void GuildList_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            foreach(ToolStripButton ToolButton in GUI_Guilds.Items)
            {
                ToolButton.Checked = false;
            }
            ToolStripButton TSB = e.ClickedItem as ToolStripButton;
            TSB.Checked = true;
            GUI_EmbedSendSelected.Enabled = false;
            GUI_EmbedSendSelected.Text = "Selected";
            Console.WriteLine($"Selected Guild {e.ClickedItem.ToolTipText}");
            Text = Program.client.CurrentUser.Username + " - " + e.ClickedItem.Text;
            var Guild = Program.client.GetGuild(Convert.ToUInt64(e.ClickedItem.AccessibleDescription));
            if (Guild == null)
            {
                Console.WriteLine("Unable to get guild");
                return;
            }
            GUI_GuildInfo.Text = $"ID: {Guild.Id}" + Environment.NewLine + $"Owner: {Guild.Owner.Username} - {Guild.Owner.Id}" + Environment.NewLine + $"Users: Online {Guild.Users.Where(x => !x.IsBot & x.Status != UserStatus.Offline).Count()}/{Guild.Users.Where(x => !x.IsBot & x.Status == UserStatus.Offline).Count()} Offline" + Environment.NewLine + $"Bots: Online {Guild.Users.Where(x => x.IsBot & x.Status != UserStatus.Offline).Count()}/{Guild.Users.Where(x => x.IsBot & x.Status == UserStatus.Offline).Count()} Offline" + Environment.NewLine + $"Roles: {Guild.Roles.Count - 1} Emojis: {Guild.Emojis.Count}" + Environment.NewLine + $"Created: {Guild.CreatedAt.Date.ToShortDateString()}";
            SelectedGuild = Convert.ToUInt64(e.ClickedItem.AccessibleDescription);
            GUI_Channels.Items.Clear();
            GUI_Channels.Visible = true;
            Program.ChannelsID.Clear();
            foreach (var Chan in Guild.TextChannels)
            {
                GUI_Channels.Items.Add($"{Chan.Name}");
                Program.ChannelsID.Add(Chan.Id);
            }
            GUI_Channels.Enabled = true;
            GUI_GuildRoles.Clear();
            foreach (var Role in Guild.Roles)
            {
                if (Role != Guild.EveryoneRole)
                {
                    if (Role.Color.R == 0)
                    {
                        AppendText(GUI_GuildRoles, Role.Name + Environment.NewLine, System.Drawing.Color.FromArgb(255, 255, 255));
                    }
                    else
                    {
                        AppendText(GUI_GuildRoles, Role.Name + Environment.NewLine, System.Drawing.Color.FromArgb(Role.Color.R, Role.Color.G, Role.Color.B));
                    }
                    AppendText(GUI_GuildRoles, Role.Id + Environment.NewLine + Environment.NewLine, System.Drawing.Color.FromArgb(255, 255, 255));
                }
            }
            List<string> EmojiList = new List<string>();
            foreach (var Emoji in Guild.Emojis)
            {
                EmojiList.Add(Emoji.Name);
            }
            GUI_GuildEmojis.Text = string.Join(Environment.NewLine, EmojiList.ToArray());
        }

        private void ChannelList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine($"Selected Channel {GUI_Channels.SelectedItem}");
            int Index = GUI_Channels.SelectedIndex;
            SelectedChannel = Program.ChannelsID[Index];
            var Guild = Program.client.GetGuild(SelectedGuild);
            var Chan = Guild.GetChannel(SelectedChannel) as ITextChannel;
            var User = Guild.GetUser(Program.CurrentUserID);
            if (User.GuildPermissions.EmbedLinks || User.GetPermissions(Chan).EmbedLinks)
            {
                GUI_EmbedSendSelected.Enabled = true;
                GUI_EmbedSendSelected.Values.Text = "Selected" + Environment.NewLine + GUI_Channels.SelectedItem;
            }
            else
            {
                GUI_EmbedSendSelected.Enabled = false;
                GUI_EmbedSendSelected.Values.Text = "Selected" + Environment.NewLine + "No Perms";
            }
        }

        private async void BtnSendSelected_Click(object sender, EventArgs e)
        {
            if (SelectedGuild == 0)
            {
                MessageBox.Show("No guild selected");
                return;
            }
            if (SelectedChannel == 0)
            {
                MessageBox.Show("No channel selected");
                return;
            }
            if (GUI_EmbedTitle.Text == LastEmbedTitle)
            {
                if (GUI_EmbedText.Text == LastEmbedText)
                {
                    MessageBox.Show("You already send this message");
                    return;
                }
            }
            LastEmbedTitle = GUI_EmbedTitle.Text;
            LastEmbedText = GUI_EmbedText.Text;
            var Guild = Program.client.GetGuild(SelectedGuild);
            var Chan = Guild.GetChannel(SelectedChannel) as ITextChannel;
            var embed = new EmbedBuilder()
            {
                Title = GUI_EmbedTitle.Text,
                Description = GUI_EmbedText.Text,
                Color = EmbedColor,
                Footer = new EmbedFooterBuilder()
                {
                    Text = GUI_EmbedFooter.Text
                }
            };
            await Chan.SendMessageAsync("", false, embed);
        }

        private async void BtnSendActive_Click(object sender, EventArgs e)
        {
            if (GUI_EmbedSendActive.Text == "Active")
            {
                if (Program.ActiveGuildID == 0)
                {
                    MessageBox.Show("No active guild");
                    return;
                }
            }
            if (Program.ActiveChannelID == 0)
            {
                MessageBox.Show("No active channel");
                return;
            }
            if (GUI_EmbedTitle.Text == LastEmbedTitle)
            {
                if (GUI_EmbedText.Text == LastEmbedText)
                {
                    MessageBox.Show("You already send this message");
                    return;
                }
            }
            LastEmbedTitle = GUI_EmbedTitle.Text;
            LastEmbedText = GUI_EmbedText.Text;
            var embed = new EmbedBuilder()
            {
                Title = GUI_EmbedTitle.Text,
                Description = GUI_EmbedText.Text,
                Color = EmbedColor,
                Footer = new EmbedFooterBuilder()
                {
                    Text = GUI_EmbedFooter.Text
                }
            };
            if (GUI_ActiveGuildName.Text != "DM" & GUI_ActiveChannelName.Text.Contains("@"))
            {
                var Guild = Program.client.GetGuild(Program.ActiveGuildID);
                var GuildChan = Guild.GetChannel(Program.ActiveChannelID) as ITextChannel;
                await GuildChan.SendMessageAsync("", false, embed);
            }
            else
            {
                IDMChannel DMChan = Program.client.GetChannel(Program.ActiveChannelID) as IDMChannel;
                await DMChan.SendMessageAsync("", false, embed);
            }
        }

        private void HyperlinkGithub_LinkClicked(object sender, EventArgs e)
        {
            
        }

        private void HyperlinkWebsite_LinkClicked(object sender, EventArgs e)
        {
            
        }

        private void HyperlinkGuild_LinkClicked(object sender, EventArgs e)
        {
            
        }

        private void LinkDownload_LinkClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://discore.blaze.ml/");
        }

        private void BtnOpenSelfbotFolder_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Program.SelfbotDir);
        }

        private void ViewCommandsList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            GUI_CommandsInfo.Text = e.Node.ToolTipText;
        }

        private void BtnThemeDefault_Click(object sender, EventArgs e)
        {
            GUI_ThemeManager.GlobalPaletteMode = PaletteModeManager.Office2010Blue;
            Properties.Settings.Default.Theme = "Default";
            Properties.Settings.Default.Save();
        }

        private void BtnThemeDark_Click(object sender, EventArgs e)
        {
            GUI_ThemeManager.GlobalPaletteMode = PaletteModeManager.Office2010Black;
            Properties.Settings.Default.Theme = "Default";
            Properties.Settings.Default.Save();
        }

        private void BtnThemeSparkle_Click(object sender, EventArgs e)
        {
            GUI_ThemeManager.GlobalPaletteMode = PaletteModeManager.SparkleBlue;
            Properties.Settings.Default.Theme = "Default";
            Properties.Settings.Default.Save();
        }

        private void BtnCMEdit_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.SendAction = "Edit";
            Properties.Settings.Default.Save();
        }

        private void BtnCMDelete_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.SendAction = "Delete";
            Properties.Settings.Default.Save();
        }

        private void BtnAFYes_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoForm = "Yes";
            Properties.Settings.Default.Save();
        }

        private void BtnAFNo_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoForm = "No";
            Properties.Settings.Default.Save();
        }

        private void BtnFavColor_Click(object sender, EventArgs e)
        {

        }

        private void BtnAN1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Auto nickname timer changed to 1 minute");
            Properties.Settings.Default.ANTimer = "1";
            Program.AutoNickname_Timer.Interval = 60000;
        }

        private void BtnAN5_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Auto nickname timer changed to 5 minute");
            Properties.Settings.Default.ANTimer = "5";
            Program.AutoNickname_Timer.Interval = 300000;
        }

        private void BtnAN10_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Auto nickname timer changed to 10 minute");
            Properties.Settings.Default.ANTimer = "10";
            Program.AutoNickname_Timer.Interval = 600000;
        }

        private void ViewBotsList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Name == "PixelBot")
            {
                GUI_BotsInfo.Text = "A gamer featured bot with commands for steam/osu/minecraft and twitch streamer alerts";
                GUI_BotsWebLink.AccessibleDescription = "https://blaze.ml";
                GUI_BotsInviteLink.AccessibleDescription = "https://discordapp.com/oauth2/authorize?&client_id=277933222015401985&scope=bot&permissions=0";
                GUI_BotsWebLink.Visible = true;
                GUI_BotsInviteLink.Visible = true;
            }
            if (e.Node.Name == "Minotaur")
            {
                GUI_BotsInfo.Text = "A guild moderation bot with ban/kick/mute commands and advanced logging/userlogs/modlogs";
                GUI_BotsWebLink.AccessibleDescription = "https://blaze.ml";
                GUI_BotsInviteLink.AccessibleDescription = "https://discordapp.com/oauth2/authorize?&client_id=281849383404830733&scope=bot&permissions=0";
                GUI_BotsWebLink.Visible = true;
                GUI_BotsInviteLink.Visible = true;
            }
            if (e.Node.Name == "Discord Cards")
            {
                GUI_BotsInfo.Text = "Buy/Trade/Collect all of the rare cards featured around discord";
                GUI_BotsWebLink.AccessibleDescription = "";
                GUI_BotsInviteLink.AccessibleDescription = "https://discordapp.com/oauth2/authorize?client_id=275388037817696287&scope=bot";
                GUI_BotsWebLink.Visible = false;
                GUI_BotsInviteLink.Visible = true;
            }
            if (e.Node.Name == "Casino Bot")
            {
                GUI_BotsInfo.Text = "Spin the wheel and get the JACKPOT!";
                GUI_BotsWebLink.AccessibleDescription = "";
                GUI_BotsInviteLink.AccessibleDescription = "https://discordapp.com/oauth2/authorize?client_id=263330369409908736&scope=bot&permissions=19456";
                GUI_BotsWebLink.Visible = false;
                GUI_BotsInviteLink.Visible = true;
            }
            if (e.Node.Name == "Discord RPG")
            {
                GUI_BotsInfo.Text = "Who dosent love a good RPG bot?";
                GUI_BotsWebLink.AccessibleDescription = "https://wiki.discorddungeons.me/Home";
                GUI_BotsInviteLink.AccessibleDescription = "https://discordapp.com/oauth2/authorize?&client_id=170915256833540097&scope=bot&permissions=0";
                GUI_BotsWebLink.Visible = true;
                GUI_BotsInviteLink.Visible = true;
            }
            if (e.Node.Name == "Sekoboto")
            {
                GUI_BotsInfo.Text = "A cool bot with random and usefull commands it also her per guild/channel configs";
                GUI_BotsWebLink.AccessibleDescription = "https://sekusuikuto.archbox.pro/";
                GUI_BotsInviteLink.AccessibleDescription = "https://discordapp.com/oauth2/authorize?client_id=217215738753056768&scope=bot&permissions=1518657";
                GUI_BotsWebLink.Visible = true;
                GUI_BotsInviteLink.Visible = true;
            }
        }

        private void HyperlinkBotWebsite_LinkClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(GUI_BotsWebLink.AccessibleDescription);
        }

        private void HyperlinkBotInvite_LinkClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(GUI_BotsInviteLink.AccessibleDescription);
        }
        
        private void CustomAddCustomAdd_LinkClicked(object sender, EventArgs e)
        {
            MessageBox.Show("Custom commands disabled > Under construction");
            return;
            var Custom = new CustomCommand();
                Custom.Show();
        }

        private void CustomDelete_LinkClicked(object sender, EventArgs e)
        {
            if (GUI_CCList.SelectedIndex == -1)
            {
                MessageBox.Show("Custom commands disabled > Under construction");
                return;
                MessageBox.Show("No item selected");
                return;
            }
            string Selected = GUI_CCList.SelectedItem.ToString();
            File.Delete(Program.SelfbotDir + "Custom\\" + Selected + ".txt");
            File.Delete(Program.SelfbotDir + "Custom\\" + Selected + ".message.txt");
            GUI_CCList.Items.RemoveAt(GUI_CCList.SelectedIndex);
        }

        private void KryptonColorButton1_SelectedColorChanged(object sender, ColorEventArgs e)
        {
            if (e.Color.IsEmpty)
            {
                GUI_EmbedColorStrip.BackColor = new System.Drawing.Color();
                EmbedColor = new Discord.Color();
            }
            else
            {
                GUI_EmbedColorStrip.BackColor = e.Color;
                EmbedColor = new Discord.Color(e.Color.R, e.Color.G, e.Color.B);
            }
        }

        private void BtnLogsAll_LinkClicked(object sender, EventArgs e)
        {

        }

        private void BtnLogsGuild_LinkClicked(object sender, EventArgs e)
        {

        }

        private void BtnLogsChannel_LinkClicked(object sender, EventArgs e)
        {

        }

        private void EmbedTitle_TextChanged(object sender, EventArgs e)
        {
            if (EmbedFirstClick == false)
            {
                EmbedFirstClick = true;
                GUI_EmbedTitle.Text = "";
                GUI_EmbedText.Text = "";
                GUI_EmbedFooter.Text = "";
            }
        }

        private void EmbedText_TextChanged(object sender, EventArgs e)
        {
            if (EmbedFirstClick == false)
            {
                EmbedFirstClick = true;
                GUI_EmbedTitle.Text = "";
                GUI_EmbedText.Text = "";
                GUI_EmbedFooter.Text = "";
            }
        }

        private void EmbedFooter_TextChanged(object sender, EventArgs e)
        {
            if (EmbedFirstClick == false)
            {
                EmbedFirstClick = true;
                GUI_EmbedTitle.Text = "";
                GUI_EmbedText.Text = "";
                GUI_EmbedFooter.Text = "";
            }
        }

        private void BtnHideConsoleYes_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.HideConsole = "Yes";
            Properties.Settings.Default.Save();
        }

        private void BtnHideConsoleNo_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.HideConsole = "No";
            Properties.Settings.Default.Save();
        }

        private void BtnFavColor_SelectedColorChanged(object sender, ColorEventArgs e)
        {
            if (e.Color.IsEmpty)
            {
                GUI_FavColor.StateNormal.Back.Color1 = e.Color;
                GUI_FavColor.OverrideDefault.Back.Color1 = e.Color;
                Properties.Settings.Default.FavoriteColor = System.Drawing.Color.Empty;
                Program.FavoriteColor = new Discord.Color();
            }
            else
            {
                GUI_FavColor.StateNormal.Back.Color1 = e.Color;
                GUI_FavColor.OverrideDefault.Back.Color1 = e.Color;
                Properties.Settings.Default.FavoriteColor = e.Color;
                Program.FavoriteColor = new Discord.Color(e.Color.R, e.Color.G, e.Color.B);
            }
            Properties.Settings.Default.Save();
        }

        private void BtnRoleColorYes_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.RoleColor = "Yes";
            Properties.Settings.Default.Save();
        }

        private void BtnRoleColorNo_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.RoleColor = "No";
            Properties.Settings.Default.Save();
        }

        private void EmbedPaint_Click(object sender, EventArgs e)
        {
            GUI_EmbedColorMenu.PerformDropDown();
        }

        private void EmbedClear_Click(object sender, EventArgs e)
        {
            EmbedFirstClick = true;
            GUI_EmbedTitle.Text = "";
            GUI_EmbedText.Text = "";
            GUI_EmbedFooter.Text = "";
        }

        private void EmbedPopup_Click(object sender, EventArgs e)
        {
            FormEmbedPopup = new EmbedPopup();
            FormEmbedPopup.ShowDialog();
            FormEmbedPopup.ActiveGuild.Text = Program.ActiveGuild;
            FormEmbedPopup.ActiveChannel.Text = Program.ActiveChannel;
        }

        private void ActiveGuildName_Click(object sender, EventArgs e)
        {
            
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void GUI_AutoStartYes_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.RegistryKey RegKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            string DiscorePath = Environment.GetFolderPath(Environment.SpecialFolder.Programs) + @"\Blaze\Discore.appref-ms";
            RegKey.SetValue("Discore-Selfbot", DiscorePath);
            GUI_AutoStartText.Text = "Yes";
        }

        private void GUI_AutoStartNo_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.RegistryKey RegKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            RegKey.DeleteValue("Discore-Selfbot");
            GUI_AutoStartText.Text = "No";
        }
    }
}
