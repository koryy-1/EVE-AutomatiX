using EVE_AutomatiX.ClientWindow;
using EVE_AutomatiX.Controllers;
using EVE_AutomatiX.Models;
using EVE_AutomatiX.Starship.API;
using EVE_AutomatiX.Starship.Controllers;
using EVE_AutomatiX.Starship.Modules;
using EVE_AutomatiX.Starship.Monitors;
using EVE_Bot.Models;
using EVE_Bot.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EVE_AutomatiX
{
    public class Ship
    {
        public State state;
        public Location location;
        public Mode mode;
        public FlightMode flightMode;

        public PropModule propModule;
        public Weapon weapon;
        public Defense defense;
        public Amplifiers amplifiers;

        // monitors
        //DangerAnalyzerThread DangerAnalyzerThread = new DangerAnalyzerThread(args...);
        Thread DangerAnalyzerThread;
        Thread disconnectMonitorThread;
        Thread shipHPMonitorThread;
        private TargetMonitor targetMonitor;

        // controllers
        Thread droneControlThread;
        private TargetController targetController;
        private MissileController missileController;
        private NavigationController navigationController;

        // systems
        public CombatSystems CombatSystems;
        public Diagnostics Diagnostics;
        public Docking Docking;
        public Looting Looting;
        public Navigating Navigating;
        public Routing Routing;

        private Client _client;

        public Ship(Config config, BotBehavior behavior, Client client)
        {
            _client = client;


            propModule = new PropModule(_client);
            weapon = new Weapon(_client, config.WeaponsRange, config.Charges);
            // todo: defense may be list
            defense = new Defense(_client);
            // todo: too
            amplifiers = new Amplifiers(_client);


            CombatSystems = new CombatSystems(client, behavior, defense, amplifiers);
            Diagnostics = new Diagnostics(client);
            Docking = new Docking(client);
            Looting = new Looting(client);
            Navigating = new Navigating(client);
            Routing = new Routing(client);

            if (Docking.IsDocked())
                location = Location.Dock;
            else
                location = Location.Space;


            //DangerAnalyzerThread.StartThread();
            // todo: delete DangerAnalyzerThread, disconnectMonitor - new instance?
            DangerAnalyzerThread.Start();

            // monitors
            targetMonitor = new TargetMonitor(client, behavior);
            targetMonitor.StartThread();

            // controllers
            targetController = new TargetController(client, behavior, config.WeaponsRange);
            targetController.StartThread();

            missileController = new MissileController(client, behavior, weapon);
            missileController.StartThread();

            navigationController = new NavigationController(client, behavior, Navigating, propModule);

            // StartThread Controllers and Monitors
            //ThreadCreater threadCreater = new ThreadCreater(_config, _behavior, _currentState);
            //threadCreater.StartMonitors();
            //threadCreater.StartControls();
        }
    }

    public enum Mode
    {
        Master,
        Emergency
    }

    public enum State
    {
        NotAvailable,
        Fighting,
        Looting,
        Searching
    }

    public enum Location
    {
        Dock,
        Space,
    }
}
