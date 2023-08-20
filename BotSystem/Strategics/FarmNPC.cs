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
        public FarmNPC(Config config, BotBehavior behavior, BotState currentState)
        {
            _config = config;
            _behavior = behavior;
            _currentState = currentState;
        }
        public void Start()
        {
            DateTime farmStartTime = DateTime.Now;
            var allShipSystemsAreOK = CommonFuncs.RunDiagnostics();
            if (!allShipSystemsAreOK)
            {
                return;
            }

            while (_currentState != BotState.FallDown)
            {
                if (_config.FarmExp) GotoFarmExp();

                Routing.EnsureRouteBuilt();

                GotoFarmAnomaly();

                CommonFuncs.Pause();

                Routing.GotoNextSystem();
            }
            //logger("bot fall down");
        }

        private void GotoFarmAnomaly()
        {
            while (IsAnomalyInCurrentSystem())
            {
                GotoAnomaly();
                FarmAnomaly();
                Looting.GotoLoot();
            }
        }

        private bool IsAnomalyInCurrentSystem()
        {
            // todo: названия брать из конфига + точные названия
            if (Finder.FindAnomalies(new List<int> { 0, 2 }).Count() > 0) // List<Anomaly> from config
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

        private void GotoAnomaly()
        {
            var Anomalies = Finder.FindAnomalies(new List<int> { 0, 2 }); // List<Anomaly> from config
            // Mouse.Click(Anomalies[0].CoordsOfWarpBtn)
            // Check ship is warp
            throw new NotImplementedException();
        }
    }
}
