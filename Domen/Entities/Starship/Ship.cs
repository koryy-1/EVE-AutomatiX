using Domen.Entities.Starship.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domen.Enums;
using EVE_AutomatiX.Starship.Modules;
using Domen.Models;

namespace Domen.Entities.Starship
{
    public class Ship
    {
        public State state { get; set; }
        public Location location { get; set; }
        public Mode mode { get; set; }
        public FlightMode flightMode { get; set; }

        public PropModule propModule { get; set; }
        public Weapon weapon { get; set; }
        public Defense defense { get; set; }
        public Amplifiers amplifiers { get; set; }

        //// monitors
        ////DangerAnalyzerThread DangerAnalyzerThread = new DangerAnalyzerThread(args...);
        //Thread DangerAnalyzerThread;
        //Thread disconnectMonitorThread;
        //Thread shipHPMonitorThread;
        //private TargetMonitor targetMonitor;

        //// controllers
        //Thread droneControlThread;
        //private TargetController targetController;
        //private MissileController missileController;
        //private NavigationController navigationController;

        //// systems
        //public CombatSystems CombatSystems;
        //public Diagnostics Diagnostics;
        //public Docking Docking;
        //public Looting Looting;
        //public Navigating Navigating;
        //public Routing Routing;

        //private Client _client;
        private List<Module> _modules;

        public Ship(Config config, BotBehavior behavior, List<Module> modules)
        {
            _modules = modules;


            propModule = new PropModule(_modules);
            weapon = new Weapon(_modules, config.WeaponsRange, config.Charges);
            // todo: defense may be list
            defense = new Defense(_modules);
            // todo: too
            amplifiers = new Amplifiers(_modules);


            //CombatSystems = new CombatSystems(client, behavior, defense, amplifiers);
            //Diagnostics = new Diagnostics(client);
            //Docking = new Docking(client);
            //Looting = new Looting(client);
            //Navigating = new Navigating(client);
            //Routing = new Routing(client);

            //if (Docking.IsDocked())
            //    location = Location.Dock;
            //else
            //    location = Location.Space;


            //DangerAnalyzerThread.StartThread();
            // todo: delete DangerAnalyzerThread, disconnectMonitor - new instance?
            //DangerAnalyzerThread.Start();

            //// monitors
            //targetMonitor = new TargetMonitor(client, behavior);
            //targetMonitor.StartThread();

            //// controllers
            //targetController = new TargetController(client, behavior, config.WeaponsRange);
            //targetController.StartThread();

            //missileController = new MissileController(client, behavior, weapon);
            //missileController.StartThread();

            //navigationController = new NavigationController(client, behavior, Navigating, propModule);

            // StartThread Controllers and Monitors
            //ThreadCreater threadCreater = new ThreadCreater(_config, _behavior, _currentState);
            //threadCreater.StartMonitors();
            //threadCreater.StartControls();
        }
    }
}
