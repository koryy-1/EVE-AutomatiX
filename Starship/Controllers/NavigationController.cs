using EVE_AutomatiX.ClientWindow;
using EVE_AutomatiX.Models;
using EVE_AutomatiX.Starship.API;
using EVE_AutomatiX.Starship.Modules;
using EVE_Bot.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Controllers
{
    public class NavigationController : ThreadWrapper
    {
        private Client _client;
        private BotBehavior _botBehavior;
        private Navigating _navigating;
        private PropModule _propModule;

        public NavigationController(Client client, BotBehavior botBehavior, Navigating navigating, PropModule propModule)
        {
            _client = client;
            _botBehavior = botBehavior;
            _navigating = navigating;
            _propModule = propModule;
        }

        public override bool ConditionToStartWorker()
        {
            return _botBehavior.CombatFlags.BattleModeEnabled;
        }

        public override void Work()
        {
            // priority
            // 1 low shield
            // 2 approuch enemy
            // 3 orbit enemy
            // 4 some actions to defined target

            //Jumping
            //Click target
            //Approaching
            //Orbiting
            //Warping
            //Aligning

            while (true)
            {
                if (_botBehavior.ShipHPFlags.IsLowShield)
                {
                    // todo: лететь в сторону от скопления НПС
                    //keep in range enemy
                    if (!_navigating.CheckFlightMode(FlightMode.KeepingAtRange))
                    {
                        var Enemy = Finder.GetEnemies().OrderBy(item => Finder.Km2m(item.Distance)).ToList().FirstOrDefault();

                        _navigating.SetFlightModeOnObject(Enemy, FlightMode.KeepingAtRange);
                    }
                }

                else if (_botBehavior.CombatFlags.CloseDistanceToEnemy)
                {
                    var Enemy = _client.Parser.OV.GetInfo()
                        .OrderBy(item => item.Distance.value).ToList()
                        .Find(item => _client.Parser.OV.GetColorInfo(item.Colors) is "red");

                    if (!_navigating.CheckFlightMode(FlightMode.KeepingAtRange, Enemy))
                    {
                        _navigating.OrbitObject(Enemy);

                        _propModule.Enable();
                    }
                }

                else if (_navigating._spaceObject != null)
                {
                    !_navigating.CheckFlightMode(FlightMode.KeepingAtRange, spaceObject)
                    if (!Checkers.CheckState(ThreadManager.ExpectedState, ThreadManager.ItemInSpace))
                    {
                        General.GotoInActiveItem(ThreadManager.ItemInSpace, ThreadManager.FlightManeuver);

                        _propModule.Enable();
                    }
                }
                else if (ThreadManager.ItemInSpace == "" && ThreadManager.FlightManeuver == "")
                {
                    if (CurrentState != "Ship Stopping")
                    {
                        Emulators.AllowControlEmulator = false;
                        General.ModuleActivityManager(ModulesInfo.MWD, false, PrivilegeControl: true);
                        Emulators.AllowControlEmulator = true;
                        General.SetSpeed(0);
                        var CurrentShipState = HI.GetShipState(HI.GetHudContainer());
                        if (CurrentShipState != null)
                        {
                        }
                    }
                }

                Thread.Sleep(5 * 1000);
            }
        }
    }
}
