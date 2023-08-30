using EVE_AutomatiX.Models;
using EVE_AutomatiX.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EVE_AutomatiX
{
    public partial class Form1 : Form
    {
        //int countTextBoxNickName = 1;
        //List<Config> botConfigs { get; set; } = new List<Config>();
        List<string> nickNameList = new List<string>();
        List<Thread> _threads = new List<Thread>();
        public Form1()
        {
            InitializeComponent();
            // load config
            // if there are no configs - create new
            // get list of json contents word config

            List<string> jsonFilePaths = ConfigReader.GetConfigPaths();

            // if no one create new
            if (jsonFilePaths.Count == 0)
            {
                ConfigReader.CreateNewConfig();
                return;
            }

            nickNameList = ConfigReader.GetNickNameList(jsonFilePaths);

            nickNameLabel1.Text = nickNameList[0];

            // fill each text label nicknames
            //foreach (var nick in nickNameList)
            //{
            //    nickNameLabel1.Text = nick;
            //}
        }

        private void Start_Click(object sender, EventArgs e)
        {
            List<string> NickNameList = getNickNameList();
            foreach (var nickname in NickNameList)
            {
                //if (bot.started)
                //{
                //    continue;
                //}
                Config config = ConfigReader.GetConfigByNickName(nickname);

                BotLauncher Bot = new BotLauncher(nickname, config);

                Thread thread = new Thread(Bot.Start);
                thread.Name = nickname;
                thread.Start();
                _threads.Add(thread);
            }
        }

        private List<string> getNickNameList()
        {
            return nickNameList;
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            // todo get/set config (launcher)
            // config file for each client?
        }

        private void CreateNewBot_Click(object sender, EventArgs e)
        {
            // todo open window to enter nickname
            //Enter_nickname Enter_nickname = new Enter_nickname();
            //Enter_nickname.ShowDialog();
            //string NickName = Enter_nickname.getNickName();

            //if (string.IsNullOrEmpty(NickName))
            //{
            //    return;
            //}

            //if (countTextBoxNickName == 5)
            //{
            //    return;
            //}

            //countTextBoxNickName++;
            //TextBox newTextBox = new TextBox();
            //newTextBox.Top = createBotButton.Top;
            //newTextBox.Left = createBotButton.Left;
            //newTextBox.Width = textBoxNickName1.Width;
            //createBotButton.Top = createBotButton.Top + 40;
            //newTextBox.Name = $"textBoxNickName{countTextBoxNickName}";
            //this.Controls.Add(newTextBox);
            /////////////////

            // if nickname not empty
            // create buttons on form + create new config file {nick}-config.json

            //string nick = TextBox1.Text;
            //string config = JsonSerializer.Serialize(new Config());
            //File.WriteAllText($"{nick}-config.json", config);
        }

        private void changeNickNameBtn_Click(object sender, EventArgs e)
        {
            Enter_nickname Enter_nickname = new Enter_nickname();
            Enter_nickname.Text = "Change nickname";
            Enter_nickname.setNickName(nickNameLabel1.Text);
            var result = Enter_nickname.ShowDialog();
            string nick = Enter_nickname.getNickName();

            if (result == DialogResult.OK && !string.IsNullOrEmpty(nick))
            {
                // change file name or field in content
                ConfigReader.RenameConfig(nickNameLabel1.Text, nick);

                nickNameLabel1.Text = nick;
            }
        }

        private void botSettingsBtn1_Click(object sender, EventArgs e)
        {
            Settings botSettings = new Settings();
            Config config = ConfigReader.GetConfigByNickName(nickNameLabel1.Text);
            if (config == null)
            {
                MessageBox.Show("config not found");
                return;
            }

            botSettings.Text = $"Settings - {nickNameLabel1.Text}";
            botSettings.setFields(config);
            var result = botSettings.ShowDialog();

            if (result == DialogResult.OK)
            {
                //LOGGER.Text = "asdasdassad";
                config = botSettings.getConfig();
                //LOGGER.Text = "1231212231";
                ConfigReader.UpdateConfig(nickNameLabel1.Text, config);
            }
        }

        private void startBot1_Click(object sender, EventArgs e)
        {
            Config config = ConfigReader.GetConfigByNickName(nickNameLabel1.Text);

            BotLauncher BotLauncher = new BotLauncher(nickNameLabel1.Text, config);

            Thread thread = new Thread(BotLauncher.Start);
            thread.Name = nickNameLabel1.Text;
            thread.Start();
            _threads.Add(thread);
            // todo add logic to stop thread for Stop btn
        }
    }
}
