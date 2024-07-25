using Application.ClientWindow;
using Domen.Enums;
using Domen.Models;
using EVE_AutomatiX;
using EVE_AutomatiX.Starship.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.StarshipSystems.Controllers
{
    public class MissileController
    {
        private Client _client;
        private BotBehavior _botBehavior;
        private Weapon _weapon;

        int CountTimeFor0Amount = 0;

        public MissileController(Client client, BotBehavior botBehavior, Weapon weapon)
        {
            _client = client;
            _botBehavior = botBehavior;
            _weapon = weapon;
        }

        public bool ConditionToStartWorker()
        {
            return _botBehavior.CombatFlags.BattleModeEnabled;
        }

        public void Work()
        {
            while (true)
            {
                if (_weapon.Name != ModuleNames.MissileLauncher)
                {
                    //logger($"{ModuleName.MissileLauncher} not found");
                    return;
                    // todo: currentState = BotState.Falldown
                }

                if (_weapon.Mode == "idle" && _weapon.AmountСharges != 0)
                {
                    //logger("missiles are idle");
                    _weapon.Enable();
                    CountTimeFor0Amount = 0;
                }
                else if (_weapon.Mode == "reloading")
                {
                    CountTimeFor0Amount = 0;
                }
                else if (_weapon.Mode == "idle" && _weapon.AmountСharges == 0)
                {
                    CountTimeFor0Amount++;
                }
                if (CountTimeFor0Amount > 10)
                {
                    //logger("missiles are over, amount = {0}", _weapon.AmountСharges);
                    return;
                    // todo: currentState = BotState.Falldown
                }

                Thread.Sleep(300);
            }
        }
    }
}
