using EVE_AutomatiX.ClientWindow;
using EVE_AutomatiX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Strategics
{
    public class Bot : ThreadWrapper
    {
        Config _config { get; set; }
        BotBehavior _behavior { get; set; }
        BotState _currentState { get; set; }
        public Ship _ship { get; set; }
        public Client _client { get; set; }

        public Bot(Config config, BotBehavior behavior, BotState currentState)
        {
            _config = config;
            _behavior = behavior;
            _currentState = currentState;
        }

        public override void Work()
        {
            switch (_config._protocol)
            {
                case 0:
                    Autopilot autopilot = new Autopilot();
                    autopilot._ship = _ship;
                    autopilot._client = _client;
                    autopilot.Start();
                    break;
                case 1:
                    FarmNPC farmNPC = new FarmNPC(_config, _behavior, _currentState);
                    farmNPC._ship = _ship;
                    farmNPC._client = _client;
                    farmNPC.Start();
                    break;
                default:
                    // logger("protocol {protocol} not exist")
                    break;
            }
        }

        public override bool ConditionToStartWorker()
        {
            // todo: conditions for autopilot
            return _behavior.CombatFlags.BotEnable;
        }
    }
}
