using Application.ClientWindow;
using Domen.Models;
using EVE_AutomatiX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.StarshipSystems.Monitors
{
    public class ShipHPMonitor
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

        public bool ConditionToStartWorker()
        {
            throw new NotImplementedException();
        }

        public void Work()
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
