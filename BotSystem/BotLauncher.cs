using EVE_AutomatiX.ClientWindow;
using EVE_AutomatiX.Models;
using EVE_AutomatiX.Monitors;
using EVE_AutomatiX.Strategics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EVE_AutomatiX
{
    class BotLauncher
    {
        string _nickName { get; set; }
        Config _config;
        BotBehavior _behavior { get; set; }
        BotState _currentState { get; set; }
        
        public BotLauncher(string nickName, Config config)
        {
            _nickName = nickName;
            _config = config;
            _config.ClientParams.ProcessName = $"EVE - {_nickName}";
            _behavior = new BotBehavior(); // протестить на разных окнах
        }

        public void Start()
        {
            // initialize instancies bot emergencyMode client and ship
            // bot start()
            Client client = new Client(ref _config, _nickName);
            bool success = client.UpdateRootAddress();
            if (!success) return;


            Ship ship = new Ship(_config, _behavior, client);


            Bot bot = new Bot(_config, _behavior, _currentState);
            bot._ship = ship;
            bot._client = client;
            bot.StartThread();

            EmergencyMode emergencyMode = new EmergencyMode(_config, _behavior, _currentState);
            emergencyMode.StartThread();


            while (_currentState != BotState.FallDown) Thread.Sleep(1000);

            // todo: how drop threads bot and emergency
            // _behavior.BotEnable = false
            // _behavior.EmergencyModeEnable = false
        }
    }
}
