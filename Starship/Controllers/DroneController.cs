using EVE_AutomatiX.ClientWindow;
using EVE_AutomatiX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Controllers
{
    public class DroneController : ThreadWrapper
    {
        private Client _client;
        private BotBehavior _botBehavior;

        public DroneController(Client client, BotBehavior botBehavior)
        {
            _client = client;
            _botBehavior = botBehavior;
        }

        public override bool ConditionToStartWorker()
        {
            return _botBehavior.CombatFlags.BattleModeEnabled;
        }

        public void DroneControl()
        {
            throw new NotImplementedException();
        }

        public override void Work()
        {
            throw new NotImplementedException();
        }
    }
}
