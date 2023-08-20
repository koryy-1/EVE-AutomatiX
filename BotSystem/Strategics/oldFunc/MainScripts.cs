using EVE_Bot.Configs;
using EVE_Bot.Controllers;
using EVE_Bot.Models;
using EVE_Bot.Parsers;
using EVE_Bot.Searchers;
using read_memory_64_bit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace EVE_Bot.Scripts
{
    static public class MainScripts
    {
        static public Random r = new Random();
        static public int AvgDeley = Config.AverageDelay;
        static ModulesData ModulesInfo = new ModulesData();

        static public void BotStart()
        {
            int RandomHours = r.Next(2, 4); //for ints
            DateTime TimeForBreak = DateTime.Now;
            Console.WriteLine("RandomHours {0}", RandomHours);
            Console.WriteLine("TimeForBreak {0}", TimeForBreak);
            int PauseDuration = r.Next(10, 40);

            SecScripts.CheckSituation();

            for (int i = 0; i < 300; i++)
            {
                DateTime StartTime = DateTime.Now;
                Console.WriteLine(StartTime);

                if (Config.FarmExp)
                {
                    GotoFarmExp();
                }

                FarmAnomalies(StartTime);
                if (i % 4 == 0)
                    (RandomHours, TimeForBreak) = Pause(TimeForBreak, RandomHours, PauseDuration);
            }
        }

        //static public void Pause()
        //{
        //    int randomPauseDuration = r.Next(20, 40); // Random pause duration between 20 and 40 minutes
        //    int randomInterval = r.Next(3, 5); // Random interval between pauses between 3 and 5 hours

        //    // Check if it's time for a pause
        //    if ((DateTime.Now - lastPauseTime).TotalHours > randomInterval)
        //    {
        //        // Dock the ship
        //        General.Dock();

        //        // Wait for the docking process to complete
        //        while (!Checkers.IsDocked())
        //        {
        //            Thread.Sleep(1000);
        //        }

        //        // Pause for the randomly generated duration
        //        Console.WriteLine("Pausing for {0} minutes", randomPauseDuration);
        //        Thread.Sleep(randomPauseDuration * 60 * 1000);

        //        // Undock the ship
        //        General.Undock();

        //        // Update the last pause time
        //        lastPauseTime = DateTime.Now;
        //    }
        //}

        static public (int, DateTime) Pause(DateTime TimeForBreak, int RandomHours, int PauseDuration)
        {
            if ((DateTime.Now - TimeForBreak).TotalHours > RandomHours)
            {
                RandomHours = r.Next(3, 5); //for ints
                TimeForBreak = DateTime.Now;
                PauseDuration = r.Next(20, 40);


                (int XStationCoords, int YStationCoords) = Finders1.FindObjectByWordInOverview("Station");
                if (XStationCoords == 0)
                {
                    Console.WriteLine("Station not found");
                    Console.WriteLine("pause for {0} minutes, start at {1}", PauseDuration, TimeForBreak);
                    sleep(PauseDuration * 60 * 1000);
                    return (RandomHours, TimeForBreak);
                }
                ThreadManager.AllowDocking = true;
                ThreadManager.AllowDScan = false;
                //Console.WriteLine("unload cargo");

                General.GotoInActiveItem("Station", "Dock");

                Console.WriteLine("wait 1 min for dock");
                Checkers.WatchState();

                var LobbyWnd = GetUITrees().FindEntityOfString("LobbyWnd");
                while (LobbyWnd == null)
                {
                    Thread.Sleep(1000 + r.Next(-100, 100));
                    LobbyWnd = GetUITrees().FindEntityOfString("LobbyWnd");
                }
                Thread.Sleep(5 * 1000 + r.Next(-100, 100));

                //SecondaryScripts.UnloadCargo();


                Console.WriteLine("pause for {0} minutes, start at {1}", PauseDuration, TimeForBreak);
                sleep(PauseDuration * 60 * 1000);

                General.Undock();
                ThreadManager.AllowDocking = false;
                ThreadManager.AllowDScan = true;
            }
            return (RandomHours, TimeForBreak);
        }

        static public void FarmAnomalies(DateTime StartTime)
        {
            for (int i = 0; i < 100; i++)
            {
                var (XWarpToAnomaly, YWarpToAnomaly) = Finders1.CheckCurrentSystemForCombatSites();
                if (XWarpToAnomaly == 1)
                    continue;
                if (XWarpToAnomaly == 0)
                {
                    GotoNextSystem();
                    continue;
                }

                Emulators.ClickLB(XWarpToAnomaly, YWarpToAnomaly);

                Checkers.WatchState();

                // чек на нужную аномальку вблизи
                (_, YWarpToAnomaly) = Finders1.CheckCurrentSystemForCombatSites();
                (_, int YlocProbeScannerWindow) = Finders1.FindLocWnd("ProbeScannerWindow");
                if (YWarpToAnomaly - YlocProbeScannerWindow - 104 != 0)
                {
                    Console.WriteLine("not that anomaly near");
                    continue;
                }

                DestroyTargetsInRoom();

                SecScripts.CheckMissilesAmount();

                if (i % 10 == 0)
                {
                    if ((DateTime.Now - StartTime).TotalHours > 1)
                        return;
                }
            }
        }

        static public void DestroyTargetsInRoom()
        {
            var Enemies = Checkers.GetEnemies();

            if (Enemies.Count == 0)
            {
                Console.WriteLine("room already clear");
                return;
            }
            Console.WriteLine("start clear at: {0}", DateTime.Now);

            PreparationForCombatReadiness();

            for (int i = 0; i < 15; i++)
            {
                Enemies = Checkers.GetEnemies();

                if (Enemies.Count == 0)
                {
                    Console.WriteLine("room is clear");
                    break;
                }
                
                EnsureEnemiesNear();

                // EnsureEnemiesLocked();

                if (!Enemies.Exists(item => item.TargetLocked))
                {
                    Emulators.LockTargets(Enemies); // сократить массив до 5 таргетов

                    if (!Checkers.CheckLocking())
                        continue;
                }

                General.UnlockNotEnemyTargets();
                ThreadManager.AllowToAttack = true;

                for (int j = 0; j < 20; j++)
                {
                    if (!Checkers.WatchLockingTargets())
                    {
                        ThreadManager.AllowToAttack = false;
                        break;
                    }
                    if (j % 3 == 0 && ThreadManager.AllowDroneControl)
                    {
                        General.UnlockNotEnemyTargets();
                        //Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_F);
                    }
                    Thread.Sleep(1000 * 3);
                }
            }
            Console.WriteLine("end clear at: {0}", DateTime.Now);

            CancellationOfCombatReadiness();

            // поменять вкладку
            General.ChangeTab("Mining");
            Thread.Sleep(1000);
            GotoLootCont("Dread Guristas");
            GotoLootCont("Shadow Serpentis");
            General.ChangeTab("General");

            //while (DroneController.DroneLaunchInProgress)
            //    Thread.Sleep(1000);

            //while (DroneController.DronesDroped)
            //    Thread.Sleep(1000);

            ThreadManager.MultiplierSleep = 10;

            if (General.CheckCargo())
                SecScripts.UnloadCargo();
        }

        static public void PreparationForCombatReadiness()
        {
            ThreadManager.MultiplierSleep = 2;
            ThreadManager.AllowDroneControl = true;
            ThreadManager.AllowDroneRescoop = true;
            DroneController.DroneLaunchInProgress = true;
            ThreadManager.AllowNavigationControl = true;
            General.ModuleActivityManager(ModulesInfo.ThermalHardener, true);
            General.ModuleActivityManager(ModulesInfo.KineticHardener, true);
            General.ModuleActivityManager(ModulesInfo.MultispectrumHardener, true);
            Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_NUMPAD5);
        }

        static public void CancellationOfCombatReadiness()
        {
            ThreadManager.AllowDroneControl = false;
            ThreadManager.AllowDroneRescoop = false;
            ThreadManager.AllowNavigationControl = false;
            Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_NUMPAD6);
            Thread.Sleep(AvgDeley + r.Next(-100, 100));
            Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_R);
        }

        static public void GotoFarmExp()
        {
            for (int i = 0; i < 10; i++)
            {
                if (!FindExpidition())
                    return;

                for (int j = 0; j < 60; j++)
                {
                    if (!GotoNextSystem(false))
                        break;
                }

                if (!WarpToLocExp())
                    continue;
                Checkers.WatchState();

                StartClearExp();
                if (General.CheckCargo())
                    SecScripts.UnloadCargo();

                SecScripts.CheckMissilesAmount();
            }
        }

        static public bool FindExpidition()
        {
            var AgencyWnd = GetUITrees().FindEntityOfString("AgencyWndNew");
            if (AgencyWnd == null)
            {
                Console.WriteLine("no agency window, try to open it");
                Emulators.ClickLB(15, 150);
                Thread.Sleep(1000);
            }

            AgencyWnd = GetUITrees().FindEntityOfString("AgencyWndNew");
            if (AgencyWnd == null)
            {
                Console.WriteLine("no agency window");
                return false;
            }


            var (XlocAgencyWnd, YlocAgencyWnd) = Finders1.FindLocWnd("AgencyWndNew");


            Emulators.ClickLB(XlocAgencyWnd + 200, YlocAgencyWnd + 680);
            Thread.Sleep(1000);


            var CurrentContentGroup = GetUITrees().FindEntityOfString("CurrentContentGroupEntry");
            if (CurrentContentGroup == null)
            {
                Console.WriteLine("no CurrentContentGroupEntry");
                Emulators.ClickLB(XlocAgencyWnd + 1150, YlocAgencyWnd + 10);
                return false;
            }

            var CurrentContentGroupEntry = CurrentContentGroup.handleEntity("CurrentContentGroupEntry");
            if (CurrentContentGroupEntry.children[Convert.ToInt32(CurrentContentGroupEntry.dictEntriesOfInterest["needIndex"])]
                .children[0].dictEntriesOfInterest["_setText"].ToString() != "Escalations")
            {
                Console.WriteLine("try to open escalation");
                Emulators.ClickLB(XlocAgencyWnd + 200, YlocAgencyWnd + 680);
                Thread.Sleep(1000);
            }

            CurrentContentGroupEntry = GetUITrees().FindEntityOfString("CurrentContentGroupEntry").handleEntity("CurrentContentGroupEntry");
            if (CurrentContentGroupEntry.children[Convert.ToInt32(CurrentContentGroupEntry.dictEntriesOfInterest["needIndex"])]
                .children[0].dictEntriesOfInterest["_setText"].ToString() != "Escalations")
            {
                Console.WriteLine("no escalation");
                Emulators.ClickLB(XlocAgencyWnd + 1150, YlocAgencyWnd + 10);
                return false;
            }

            var EscalationCards = GetUITrees().FindEntityOfString("EscalationsSystemContentCard");
            if (EscalationCards == null)
            {
                Console.WriteLine("no Escalations System Content Card");
                Emulators.ClickLB(XlocAgencyWnd + 1150, YlocAgencyWnd + 10);
                return false;
            }
            var EscalationCardEntry = EscalationCards.handleEntity("EscalationsSystemContentCard");

            var Routed = false;
            Emulators.ClickLB(XlocAgencyWnd + 360, YlocAgencyWnd + 220);
            Thread.Sleep(200);
            for (int i = 0; i < EscalationCardEntry.children.Length; i++)
            {
                Emulators.ClickLB(XlocAgencyWnd + 900, YlocAgencyWnd + 590);// чекать кнопку set dest/warpto
                Thread.Sleep(1000 * 2);
                Routed = true;

                var AutopilotDestinationIcon = GetUITrees().FindEntityOfString("AutopilotDestinationIcon");
                if (AutopilotDestinationIcon == null)
                {
                    Console.WriteLine("apparently we are there");
                    Emulators.ClickLB(XlocAgencyWnd + 1150, YlocAgencyWnd + 10);
                    return true;
                }
                var AutopilotDestinationIconEntity = AutopilotDestinationIcon.handleEntity("AutopilotDestinationIcon");
                var CntLowSec = 0;
                for (int j = 0; j < AutopilotDestinationIconEntity.children.Length; j++)
                {
                    if (Convert.ToInt32(AutopilotDestinationIconEntity.children[j].children[0].dictEntriesOfInterest["_color"]["rPercent"]) >= 90
                        &&
                        Convert.ToInt32(AutopilotDestinationIconEntity.children[j].children[0].dictEntriesOfInterest["_color"]["gPercent"]) < 100)
                    {
                        CntLowSec++;
                        Console.WriteLine("skip");
                        break;
                    }
                }
                if (CntLowSec == 0)
                {
                    Console.WriteLine("found highsec exp, route laid");
                    Emulators.ClickLB(XlocAgencyWnd + 1150, YlocAgencyWnd + 10);
                    return true;
                }
                Emulators.ClickLB(XlocAgencyWnd + 360, YlocAgencyWnd + 270);
                Thread.Sleep(200);
                Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_DOWN);
                Thread.Sleep(AvgDeley + r.Next(-100, 100));
            }
            Console.WriteLine("no highsec exp");
            if (Routed)
            {
                var success = SecScripts.RemoveWaypoint();
                if (!success)
                {
                    Console.WriteLine("bad try to remove waypoint");
                    General.DockToStationAndExit();
                }
            }
            Thread.Sleep(AvgDeley + r.Next(-100, 100));
            Emulators.ClickLB(XlocAgencyWnd + 1150, YlocAgencyWnd + 10); // close agency
            return false;
        }

        static public bool WarpToLocExp()
        {
            var AgencyWnd = GetUITrees().FindEntityOfString("AgencyWndNew");
            if (AgencyWnd == null)
            {
                Console.WriteLine("no agency window, try to open it");
                Emulators.ClickLB(15, 150);
                Thread.Sleep(1000 * 2);
            }

            AgencyWnd = GetUITrees().FindEntityOfString("AgencyWndNew");
            if (AgencyWnd == null)
            {
                Console.WriteLine("no agency window");
                return false;
            }

            var (XlocAgencyWnd, YlocAgencyWnd) = Finders1.FindLocWnd("AgencyWndNew");

            Thread.Sleep(AvgDeley + r.Next(-100, 100));
            Emulators.ClickLB(XlocAgencyWnd + 900, YlocAgencyWnd + 590);
            Thread.Sleep(1000 * 2);
            Emulators.ClickLB(XlocAgencyWnd + 1150, YlocAgencyWnd + 10);
            return true;
        }

        static public void StartClearExp()
        {
            Console.WriteLine(DateTime.Now);
            Finders1.SetIconFilter("up");

            for (int i = 0; i < 5; i++)
            {
                var AcGate = Finders1.FindAccelerationGate();
                if (AcGate == null)
                    break;

                Emulators.ClickLB(AcGate.Pos.x, AcGate.Pos.y);
                Thread.Sleep(200);
                var JumpBtn = General.GetCoordsButtonActiveItem("Jump");
                if (JumpBtn.Item1 == 0)
                    JumpBtn = General.GetCoordsButtonActiveItem("ActivateGate");
                Emulators.ClickLB(JumpBtn.Item1, JumpBtn.Item2);
                Checkers.WatchState();

                AcGate = Finders1.FindAccelerationGate();
                if (AcGate == null)
                    break;

                ThreadManager.ItemInSpace = AcGate.Name;
                ThreadManager.FlightManeuver = "Orbit";
                ThreadManager.ExpectedState = "Orbiting";

                General.ModuleActivityManager(ModulesInfo.MWD, true);
                MainScripts.ClearExpRoom();
            }

            Finders1.SetIconFilter("down");
            Thread.Sleep(1000 * 2);

            Console.WriteLine("try to find ExpBlock");
            var Block = Finders1.FindExpBlock();
            if (Block != null)
            {
                ThreadManager.ItemInSpace = Block.Name;
            }

            MainScripts.ClearExpRoom();

            Thread.Sleep(1000 * 8);

            Console.WriteLine("try to find ExpBlock");
            Block = Finders1.FindExpBlock();
            if (Block != null)
            {
                List<int> Coords = new List<int>();
                Coords.Add(Block.Pos.x);
                Coords.Add(Block.Pos.y);

                Emulators.LockTargets(Coords);

                ThreadManager.FlightManeuver = "Approach";
                ThreadManager.ExpectedState = "Approaching";

                ThreadManager.AllowDroneControl = true;
                ThreadManager.AllowDroneRescoop = true;
                Checkers.CheckLocking();
                ThreadManager.AllowNavigationControl = true;
                ThreadManager.AllowToAttack = true;
                Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_NUMPAD5);

                for (int i = 0; i < 120; i++)
                {
                    if (!Checkers.WatchLockingTargets())
                        break;
                    Thread.Sleep(1000 * 10);
                }
                ThreadManager.AllowToAttack = false;
                ThreadManager.AllowNavigationControl = false;
                ThreadManager.AllowDroneControl = false;
                ThreadManager.AllowDroneRescoop = false;
                Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_NUMPAD6);
            }
            General.ModuleActivityManager(ModulesInfo.MWD, false);
            ThreadManager.ItemInSpace = "";
            ThreadManager.FlightManeuver = "";
            ThreadManager.ExpectedState = "";

            Finders1.SetIconFilter("up");
            Thread.Sleep(1000 * 2);

            GotoLootCont("Cargo Container");
            Console.WriteLine(DateTime.Now);
        }

        static public void ClearExpRoom()
        {
            var CheckForEnemies = Checkers.GetCoordsEnemies();

            if (0 == CheckForEnemies.Count)
            {
                Console.WriteLine("room already clear");
                return;
            }

            ThreadManager.AllowDroneControl = true;
            ThreadManager.AllowDroneRescoop = true;
            ThreadManager.AllowShieldHPControl = true;
            ThreadManager.AllowNavigationControl = true;

            General.ModuleActivityManager(ModulesInfo.ThermalHardener, true);
            General.ModuleActivityManager(ModulesInfo.KineticHardener, true);
            General.ModuleActivityManager(ModulesInfo.MultispectrumHardener, true);
            General.ModuleActivityManager(ModulesInfo.MissileComputer, true);

            Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_NUMPAD5);

            DestroyTargets();

            ThreadManager.AllowDroneControl = false;
            ThreadManager.AllowDroneRescoop = false;
            ThreadManager.AllowShieldHPControl = false;
            ThreadManager.AllowNavigationControl = false;

            Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_NUMPAD6);
            Thread.Sleep(AvgDeley + r.Next(-100, 100));
            Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_R);

            while (DroneController.DronesDroped)
                Thread.Sleep(1000);
        }

        static public void CheckForConnectionLost()
        {
            var uiTreePreparedForFile = GetUITrees();
            var MessageBox = uiTreePreparedForFile.FindEntityOfString("MessageBox");
            if (MessageBox == null)
                return;

            var (XlocMessageBox, YlocMessageBox) = Finders1.FindLocWnd("MessageBox");

            Console.WriteLine("qiut in MessageBox on X : {0}, Y : {1}. Exit from program", XlocMessageBox + 170, YlocMessageBox + 195 + 23);
            //return (XlocMessageBox + 170, YlocMessageBox + 195);

            Emulators.ClickLB(XlocMessageBox + 170, YlocMessageBox + 195);
            Environment.Exit(0);
        }

        static public bool CurrentSystemIsDanger()
        {
            if (DangerousEnemiesInGrid() || CriminalPilotInChat() || CheckDScan())
            {
                return true;
            }
            return false;
        }

        static public bool DangerousEnemiesInGrid()
        {
            (int XCrucifier, int YCrucifier) = Finders1.FindObjectByWordInOverview("Tetrimon Crucifier");
            (int XSage, int YSage) = Finders1.FindObjectByWordInOverview("Harvest Sage");
            if (XSage != 0 || XCrucifier != 0)
            {
                Console.WriteLine("Crucifier or Sage in grid");
                return true;
            }
            return false;
        }

        static public bool CriminalPilotInChat()
        {
            List<ChatPlayer> ChatInfo = Chat.GetInfo();
            if (ChatInfo == null)
                return false;

            for (int i = 0; i < ChatInfo.Count; i++)
            {
                if (ChatInfo[i].PlayerType == "Pilot is a suspect")
                {
                    Console.WriteLine("suspect pilot in system");
                }
                if (ChatInfo[i].PlayerType == "Pilot is a criminal")
                {
                    Console.WriteLine("criminal pilot in system");
                    return true;
                }
            }
            return false;
        }

        static public bool CheckDScan()
        {
            if (!ThreadManager.AllowDScan)
                return false;

            Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_V);
            Thread.Sleep(AvgDeley + r.Next(-100, 100));

            List<DScanItem> DScanInfo = DS.GetInfo();
            if (DScanInfo == null)
            {
                return false;
            }
            for (int i = 0; i < DScanInfo.Count; i++)
            {
                if (DScanInfo[i].Type.Contains("Catalyst"))
                {
                    Console.WriteLine("catalyst called {0} is in system", DScanInfo[i].Name);
                    Console.WriteLine("pidaras detected");
                    return true;
                }
            }
            return false;
        }

        static public void DockingFromSuicides()
        {
            (int XlocNotepadWindow, int YlocNotepadWindow) = Finders1.FindLocWnd("OverView");
            if (XlocNotepadWindow == 0) //ship docked
            {
                SecScripts.DockingAndCheckingForSuicides();
                return;
            }

            //Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_H);
            (int XStationCoords, int YStationCoords) = Finders1.FindObjectByWordInOverview("Station");
            var TryTosebat = false;
            if (XStationCoords > 0)
            {
                Emulators.ClickLB(XStationCoords, YStationCoords);
                Thread.Sleep(AvgDeley + r.Next(-100, 100));
                var DockBtn = General.GetCoordsButtonActiveItem("Dock");
                Emulators.ClickLB(DockBtn.Item1, DockBtn.Item2);

                SecScripts.DockingAndCheckingForSuicides();
            }
            else
            {
                (int XGate, int YGate) = GetCoordsNextSystem();
                if (XGate > 1)
                {
                    TryTosebat = true;
                    Emulators.ClickLB(XGate, YGate);
                    Thread.Sleep(AvgDeley + r.Next(-100, 100));
                    var JumpBtn = General.GetCoordsButtonActiveItem("Jump");
                    Emulators.ClickLB(JumpBtn.Item1, JumpBtn.Item2);
                    Checkers.WatchState();
                    Thread.Sleep(1000 * 10);
                }
                if (!TryTosebat)
                {
                    (int XStargateCoords, int YStargateCoords) = Finders1.FindObjectByWordInOverview("Stargate");
                    if (XStargateCoords > 0)
                    {
                        Emulators.ClickLB(XStargateCoords, YStargateCoords);
                        Thread.Sleep(AvgDeley + r.Next(-100, 100));
                        var JumpBtn = General.GetCoordsButtonActiveItem("Jump");
                        Emulators.ClickLB(JumpBtn.Item1, JumpBtn.Item2);
                    }
                }
            }
        }

        static public bool GotoNextSystem(bool NeedToLayRoute = true)
        {
            var (XGate, YGate) = MainScripts.GetCoordsNextSystem();
            if (YGate == 1)
            {
                Thread.Sleep(1000);
                return false;
            }
            if (YGate == 0)
            {
                if (NeedToLayRoute)
                {
                    Console.WriteLine("no route, start to laying a new route");
                    SecScripts.StartLayRoute();
                }
                return false;
            }
            Emulators.ClickLB(XGate, YGate);
            Thread.Sleep(AvgDeley + r.Next(-100, 100));
            var JumpBtn = General.GetCoordsButtonActiveItem("Jump");
            if (JumpBtn.Item1 == 0)
            {
                JumpBtn = General.GetCoordsButtonActiveItem("Dock");
            }
            Emulators.ClickLB(JumpBtn.Item1, JumpBtn.Item2);

            Checkers.WatchState();
            Thread.Sleep(1000 * 10);
            return true;
        }

        static public (int, int) GetCoordsNextSystem()
        {
            List<OverviewItem> OverviewInfo = OV.GetInfo();
            if (OverviewInfo == null)
            {
                return (1, 1);
            }
            for (int i = 0; i < OverviewInfo.Count; i++)
            {
                if (OV.GetColorInfo(OverviewInfo[i].Colors) is "yellow"
                    &&
                    (OverviewInfo[i].Type.Contains("Stargate") ||
                    OverviewInfo[i].Type.Contains("Station") ||
                    OverviewInfo[i].Type.Contains("Hub")))
                {
                    return (OverviewInfo[i].Pos.x, OverviewInfo[i].Pos.y);
                }
            }
            return (0, 0);
        }

        static public void DestroyTargets(string ExcludeTarget = null)
        {
            for (int i = 0; i < 40; i++)
            {
                var EnemyCoordsArray = Checkers.GetCoordsEnemies(ExcludeTarget);

                if (0 == EnemyCoordsArray.Count)
                {
                    Console.WriteLine("room is clear");
                    return;
                }
                if (1 == EnemyCoordsArray[0])
                {
                    EnsureEnemiesNear();
                    continue;
                }
                if (2 != EnemyCoordsArray.Last())
                {
                    Emulators.LockTargets(EnemyCoordsArray);

                    if (!Checkers.CheckLocking())
                        continue;
                }

                if (!General.LockedTargetsAreAvailable())
                {
                    General.UnlockTargets();
                    continue;
                }

                EngageTarget();
            }
            Console.WriteLine("слишком долгий процесс уничтожения");
            General.DockToStationAndExit();
        }

        static public void EngageTarget()
        {
            General.UnlockNotEnemyTargets();
            ThreadManager.AllowToAttack = true;

            for (int j = 0; j < 100; j++)
            {
                if (!Checkers.WatchLockingTargets())
                {
                    ThreadManager.AllowToAttack = false;
                    break;
                }
                if (j % (9 / ThreadManager.MultipleSleepForDrones) == 0 && ThreadManager.AllowDroneControl)
                {
                    General.UnlockNotEnemyTargets();
                    //Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_F);
                }
                Thread.Sleep(1000 * ThreadManager.MultipleSleepForDrones);
            }
        }

        static public void EnsureEnemiesNear()
        {
            for (int i = 0; i < 1; i++)
            {
                Distance MaxWeaponsRange = new Distance();
                MaxWeaponsRange.measure = "km";
                MaxWeaponsRange.value = Config.WeaponsRange;

                var Enemies = Checkers.GetEnemies().OrderBy(item => Checkers.Km2m(item.Distance)).ToList();
                if (Enemies.Count == 0)
                    return;

                var Enemy = Enemies.First();
                if (Checkers.CheckDistance(Enemy, MaxWeaponsRange))
                    return;

                Console.WriteLine("enemies are far away");
                Thread.Sleep(5 * 1000);
            }
            Console.WriteLine("okay, orbit enemy");

            ThreadManager.CloseDistanceToEnemy = true;

            for (int i = 0; i < 60; i++) // 5 min
            {
                Distance MaxWeaponsRange = new Distance();
                MaxWeaponsRange.measure = "km";
                MaxWeaponsRange.value = Config.WeaponsRange;

                var Enemies = Checkers.GetEnemies().OrderBy(item => Checkers.Km2m(item.Distance)).ToList();
                if (Enemies.Count == 0)
                    break;

                var Enemy = Enemies.First();
                if (Checkers.CheckDistance(Enemy, MaxWeaponsRange))
                    break;

                Console.WriteLine("closing to enemies");
                Thread.Sleep(5 * 1000);
            }
            Console.WriteLine("enemies are available");
            ThreadManager.CloseDistanceToEnemy = false;
        }

        static public void GotoLootCont(string ContName, int NeedPrice = 0)
        {
            var Inventory = GetUITrees().FindEntityOfString("InventoryPrimary");
            if (Inventory == null)
            {
                Console.WriteLine("try to open Inventory");
                Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_NUMPAD3);
                Thread.Sleep(1000 * 2);
            }
            Inventory = GetUITrees().FindEntityOfString("InventoryPrimary");
            if (Inventory == null)
            {
                Console.WriteLine("no Inventory window");
            }


            int XlocInventory = 0;
            int YlocInventory = 0;
            int HeightInventory = 0;
            int WidthLeftSidebar = 0;

            var ContNameInCargo = "";
            if (ContName == "Cargo Container")
                ContNameInCargo = "ItemFloatingCargo";

            if (ContName == "Dread Guristas" || ContName == "Shadow Serpentis" || ContName == "Wreck")
                ContNameInCargo = "ItemWreck";

            for (int i = 0; i < 10; i++)
            {
                (int XContCoords, int YContCoords) = Finders1.FindObjectByWordInOverview(ContName);
                if (XContCoords == 0)
                {
                    Console.WriteLine("no {0}", ContName);
                    return;
                }


                General.GotoInActiveItem(ContName, "OpenCargo");
                if (!Checkers.CheckDistance(ContName, 2500) && !Checkers.CheckState("Approaching"))
                    continue;

                var (XOpenCargo, YOpenCargo) = General.GetCoordsButtonActiveItem("OpenCargo");
                if (XOpenCargo == 0)
                    continue;
                Thread.Sleep(AvgDeley + r.Next(-100, 100));
                UITreeNode OpenedCont = null;
                for (int j = 0; j < 300; j++)
                {
                    if (Checkers.CheckDistance(ContName, 3500))
                    {
                        Emulators.ClickLB(XOpenCargo, YOpenCargo + 10); //3 button loot
                    }
                    try
                    {
                        OpenedCont = GetUITrees().FindEntityOfString(ContNameInCargo);
                    }
                    catch
                    {
                        continue;
                    }
                    if (OpenedCont != null)
                    {
                        Console.WriteLine("{0} is here", ContNameInCargo);
                        break;
                    }
                    Thread.Sleep(1000);
                }
                Thread.Sleep(1000);
                if (General.CheckCargoPrice(NeedPrice))
                {
                    (XlocInventory, YlocInventory) = Finders1.FindLocWnd("InventoryPrimary");

                    HeightInventory = Window.GetHeightWindow("InventoryPrimary");
                    WidthLeftSidebar = Window.GetWidthWindow("TreeViewEntryInventoryCargo");

                    Console.WriteLine("button loot all: {0}, {1}", XlocInventory + WidthLeftSidebar + 30, YlocInventory + HeightInventory - 20 + 23);

                    Emulators.ClickLB(XlocInventory + WidthLeftSidebar + 30, YlocInventory + HeightInventory - 20); // Loot all

                    Thread.Sleep(2 * 1000);
                    //проверка что конт залутан
                    OpenedCont = GetUITrees().FindEntityOfString(ContNameInCargo);
                    if (OpenedCont == null)
                    {
                        Console.WriteLine("cont looted");
                        //break;
                    }
                    Thread.Sleep(1000 * 5);
                }
                else
                {
                    Console.WriteLine("garbage in loot");
                    Emulators.ClickLB(XlocInventory + 60, YlocInventory + 55);
                    return;
                }
                
            }
        }

        static void sleep(int milliseconds) => Thread.Sleep(milliseconds);

        static public UITreeNode GetUITrees()
        {
            return ReadMemory.GetUITrees(Window.RootAddress, Window.processId);
        }
    }
}
