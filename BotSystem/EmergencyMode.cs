using EVE_AutomatiX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Strategics
{
    public class EmergencyMode : ThreadWrapper
    {
        Config _config { get; set; }
        BotBehavior _behavior { get; set; }
        BotState _currentState { get; set; }

        public EmergencyMode(Config config, BotBehavior behavior, BotState currentState)
        {
            _config = config;
            _behavior = behavior;
            _currentState = currentState;
        }

        public override void Work()
        {
            throw new NotImplementedException();
        }

        public override bool ConditionToStartWorker()
        {
            //return _behavior.EmergencyModeEnable;
            throw new NotImplementedException();
        }
    }
}
