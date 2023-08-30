using EVE_AutomatiX.ClientWindow;
using EVE_AutomatiX.Models;
using EVE_Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Monitors
{
    public class ShipHPMonitor : ThreadWrapper
    {
        private Client _client;
        private BotBehavior _botBehavior;

        public ShipHPMonitor(Client client, BotBehavior botBehavior)
        {
            _client = client;
            _botBehavior = botBehavior;
        }

        public void CheckShipHP()
        {
            throw new NotImplementedException();
        }

        public override bool ConditionToStartWorker()
        {
            throw new NotImplementedException();
        }

        public override void Work()
        {
            // calc delta
            // equal threshold shield value
            // set flag IsLowShield
            throw new NotImplementedException();
        }

        private HealthPoints GetHealthPoints()
        {
            return _client.Parser.HI.GetShipHP();
        }
    }
}
