using EVE_AutomatiX.ClientWindow;
using EVE_AutomatiX.Models;
using EVE_AutomatiX.Starship.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Strategics
{
    class FarmNPC
    {
        Config _config { get; set; }
        BotBehavior _behavior { get; set; }
        BotState _currentState { get; set; }
        public Ship _ship { get; set; }
        public Client _client { get; set; }
        public FarmNPC(Config config, BotBehavior behavior, BotState currentState)
        {
            _config = config;
            _behavior = behavior;
            _currentState = currentState;
        }
        public void Start()
        {
            DateTime farmStartTime = DateTime.Now;

            var allShipSystemsAreOK = _ship.Diagnostics.Run();
            if (!allShipSystemsAreOK) return;

            while (_currentState != BotState.FallDown)
            {
                if (_config.FarmExp) GotoFarmExp();

                _ship.Routing.EnsureRouteBuilt();

                GotoFarmAnomaly();

                Pause();

                _ship.Routing.GotoNextSystem();
            }
            //logger("bot fall down");
        }

        private void GotoFarmAnomaly()
        {
            while (IsAnomalyInCurrentSystem())
            {
                WarpToAnomaly();
                FarmAnomaly();
                _ship.Looting.GotoLoot();
            }
        }

        private bool IsAnomalyInCurrentSystem()
        {
            // todo: List<Anomaly> from config
            if (_client.Parser.PS.FindAnomalies(new List<string> { "Guristas", "Serpentis" }).Count() > 0)
            {
                return true;
            }
            return false;
        }

        private void GotoFarmExp()
        {
            throw new NotImplementedException();
        }

        private void FarmAnomaly()
        {
            throw new NotImplementedException();
        }

        private void WarpToAnomaly()
        {
            // List<Anomaly> from config
            var Anomalies = _client.Parser.PS.FindAnomalies(new List<string> { "Guristas", "Serpentis" });
            // Mouse.Click(Anomalies[0].CoordsOfWarpBtn)
            // Check ship is warp
            throw new NotImplementedException();
        }

        private void Pause()
        {
            throw new NotImplementedException();
        }

    }
}
