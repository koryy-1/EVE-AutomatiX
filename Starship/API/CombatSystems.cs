using EVE_AutomatiX.ClientWindow;
using EVE_AutomatiX.Controllers;
using EVE_AutomatiX.Models;
using EVE_AutomatiX.Starship.Modules;
using EVE_AutomatiX.UIHandlers;
using EVE_Bot.Models;
using EVE_Bot.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Starship.API
{
    public class CombatSystems
    {
        private Client _client;
        private BotBehavior _botBehavior;
        private Defense _defense;
        private Amplifiers _amplifiers;

        public CombatSystems(Client client, BotBehavior botBehavior, Defense defense, Amplifiers amplifiers)
        {
            _client = client;
            _botBehavior = botBehavior;
            _defense = defense;
            _amplifiers = amplifiers;
        }

        void DESTRXY_EVERYXNE()
        {
            GetReadyForBattle();

            //logger("start clear at: {0}", DateTime.Now);

            _botBehavior.CombatFlags.BattleModeEnabled = true;

            while (Finder.AreHostileObjectsInOverview())
            {
                // lock next target
                // destroy target

                Thread.Sleep(1000 * 3);
            }

            _botBehavior.CombatFlags.BattleModeEnabled = false;

            //logger("end clear at: {0}", DateTime.Now);

            CancellationOfCombatReadiness();
        }

        // todo: DESTRXY_EVERYXNE in Bot or in Ship???
        public void GetReadyForBattle()
        {
            // activate defense modules
            _defense.Enable();
            //General.ModuleActivityManager(ModulesInfo.ThermalHardener, true);
            //General.ModuleActivityManager(ModulesInfo.KineticHardener, true);
            //General.ModuleActivityManager(ModulesInfo.MultispectrumHardener, true);

            // activate amplify modules
            _amplifiers.Enable();

            // activate atack ship systems
            //Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_NUMPAD5);
        }

        public void CancellationOfCombatReadiness()
        {
            // activate speed ship systems
            // _modificators.SpeedEnable();
            //Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_NUMPAD6);

            //Thread.Sleep(AvgDeley + r.Next(-100, 100));

            // reload charges
            // todo: inside missileController
            //Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_R);
        }
    }
}
