using EVE_AutomatiX.Controllers;
using EVE_AutomatiX.Models;
using EVE_AutomatiX.Monitors;
using EVE_AutomatiX.Strategics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EVE_AutomatiX
{
    public class ThreadCreater
    {
        // todo: продумать тут все
        Config _config { get; set; }
        BotBehavior _behavior { get; set; }
        BotState _currentState { get; set; }

        // monitors
        Thread DangerAnalyzerThread;
        Thread disconnectMonitorThread;
        Thread shipHPMonitorThread;

        // controls
        Thread navigationControlThread;
        Thread droneControlThread;
        Thread missileControlThread;

        public ThreadCreater(Config config, BotBehavior behavior, BotState currentState)
        {
            _config = config;
            _behavior = behavior;
            _currentState = currentState;

            //DangerAnalyzerThread DangerAnalyzerThread = new DangerAnalyzerThread(args...);
            //DangerAnalyzerThread = new Thread(DangerAnalyzer.IsDangerDetected);
            //disconnectMonitorThread = new Thread(DisconnectMonitor.CheckConnLost);
            //shipHPMonitorThread = new Thread(ShipHPMonitor.CheckShipHP);

            //navigationControlThread = new Thread(NavigationController.NavigationControl);
            //droneControlThread = new Thread(DroneController.DroneControl);
            //missileControlThread = new Thread(MissileController.MissileControl);
        }

        // следить за тем чтобы каждый поток был запущен
        public void StartMonitors()
        {
            //DangerAnalyzerThread.StartThread();
            // todo: delete DangerAnalyzerThread, disconnectMonitor - new instance?
            DangerAnalyzerThread.Start();
            disconnectMonitorThread.Start();
            shipHPMonitorThread.Start();
        }
        public void StartControls()
        {
            navigationControlThread.Start();
            droneControlThread.Start();
            missileControlThread.Start();
        }
    }
}
