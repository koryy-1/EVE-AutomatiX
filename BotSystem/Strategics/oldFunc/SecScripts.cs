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
    static public class SecScripts
    {
        static public Random r = new Random();
        static public int AvgDeley = Config.AverageDelay;
        static ModulesData ModulesInfo = new ModulesData();

        static public void CheckSituation()
        {
            //после перезахода
            //поменять вкладку чата
            (int XlocChatWindowStack, int YlocChatWindowStack) = Finders1.FindLocWnd("ChatWindowStack");
            if (XlocChatWindowStack != 0)
            {
                Emulators.ClickLB(XlocChatWindowStack + 20, YlocChatWindowStack + 15);
                Thread.Sleep(AvgDeley + r.Next(-100, 100));
                Emulators.ClickLB(500, 100);
                Thread.Sleep(AvgDeley + r.Next(-100, 100));
            }

            //проверить расположение модулей на F кнопках
            //if CheckModulesLocOnF_Buttons
            // General.DockToStationAndExit();

            //определить где находится корабль
            General.EnsureUndocked();
            ThreadManager.AllowDocking = false;

            //поменять вкладку в инвентаре
            (int XlocInventory, int YlocInventory) = Finders1.FindLocWnd("InventoryPrimary");
            Emulators.ClickLB(XlocInventory + 60, YlocInventory + 55);
            Thread.Sleep(AvgDeley + r.Next(-100, 100));

            //узнать количество ракет
            CheckMissilesAmount();


            //узнать количество дронов, реконнект
            //var (XlocNotepadWindow, _) = Finders.FindLocWnd("DroneView");
            //if (XlocNotepadWindow == 0)
            //{
            //    General.DockToStationAndExit();
            //}
            //var DronesQuantity = Checkers.CheckQuantityDrones();
            //if (DronesQuantity <= 3)
            //{
            //    //reconnect drones
            //    var HudContainer = GetUITrees().FindEntityOfString("HudContainer").handleEntity("HudContainer");
            //    var (XHudContainer, YHudContainer) = General.GetCoordsEntityOnScreen(HudContainer
            //            .children[Convert.ToInt32(HudContainer.dictEntriesOfInterest["needIndex"])]
            //            );

            //    var CenterHudContainer = HudContainer.FindEntityOfString("CenterHudContainer").handleEntity("CenterHudContainer");
            //    var (XCenterHudContainer, _) = General.GetCoordsEntityOnScreen(CenterHudContainer
            //            .children[Convert.ToInt32(CenterHudContainer.dictEntriesOfInterest["needIndex"])]
            //            );

            //    Emulators.ClickRB(XHudContainer + XCenterHudContainer + 95, YHudContainer + 100);
            //    Thread.Sleep(AvgDeley + r.Next(-100, 100));
            //    General.ClickInContextMenu("Reconnect Drones");
            //    Thread.Sleep(3 * 1000);

            //    DroneController.ScoopDrones();

            //    //поменять вкладку в инвентаре
            //    (int XlocInventory, int YlocInventory) = Finders.FindLocWnd("InventoryPrimary");
            //    Emulators.ClickLB(XlocInventory + 60, YlocInventory + 55);
            //    Thread.Sleep(AvgDeley + r.Next(-100, 100));
            //}



            ThreadManager.AllowDScan = true;

            //узнать количество дронов, докнуться если меньше 3-х
            //DronesQuantity = Checkers.CheckQuantityDrones();
            //if (DronesQuantity < 3)
            //{
            //    General.DockToStationAndExit();
            //}

            //поменять вкладку в гриде
            General.ChangeTab("General");

            //продолжить фармить экспу или аномальку
            var Block = Finders1.FindExpBlock();
            var AcGate = Finders1.FindAccelerationGate();
            if (Block != null || AcGate != null)
            {
                ThreadManager.FlightManeuver = "Orbit";
                ThreadManager.ExpectedState = "Orbiting";
                if (Block != null)
                {
                    ThreadManager.ItemInSpace = Block.Name;
                }
                else if (AcGate != null)
                {
                    ThreadManager.ItemInSpace = AcGate.Name;
                }
                General.ModuleActivityManager(ModulesInfo.MWD, true);

                MainScripts.ClearExpRoom();
                MainScripts.StartClearExp();
            }
            else
            {
                MainScripts.DestroyTargetsInRoom();
            }
            ThreadManager.MultiplierSleep = 10;
        }


        static public void DockingAndCheckingForSuicides()
        {
            int IsCriminal = 1;
            while (IsCriminal == 1)
            {
                IsCriminal = 0;
                Thread.Sleep(1000 * 60 * 2);

                List<ChatPlayer> ChatInfo = Chat.GetInfo();

                for (int i = 0; i < ChatInfo.Count; i++)
                {
                    if (ChatInfo[i].PlayerType == "Pilot is a suspect")
                    {
                        Console.WriteLine("suspect pilot in system");
                    }
                    if (ChatInfo[i].PlayerType == "Pilot is a criminal")
                    {
                        IsCriminal = 1;
                        Console.WriteLine("criminal pilot still in system");
                        break;
                    }
                }
            }
            General.Undock();
        }

        static public void UnloadCargo()
        {
            (int XStationCoords, int YStationCoords) = Finders1.FindObjectByWordInOverview("Station");
            if (XStationCoords == 0)
            {
                Console.WriteLine("Station not found");
                return;
            }
            ThreadManager.AllowDocking = true;
            ThreadManager.AllowDScan = false;
            Console.WriteLine("unload cargo");

            General.GotoInActiveItem("Station", "Dock");

            Console.WriteLine("wait 1 min for dock");
            Checkers.WatchState();
            //Thread.Sleep(8 * 1000 + r.Next(-100, 100));

            var LobbyWnd = GetUITrees().FindEntityOfString("LobbyWnd");
            while (LobbyWnd == null)
            {
                Thread.Sleep(1000 + r.Next(-100, 100));
                LobbyWnd = GetUITrees().FindEntityOfString("LobbyWnd");
            }
            Thread.Sleep(5 * 1000 + r.Next(-100, 100));

            General.RepairShip();

            (int XlocChatWindowStack, int YlocChatWindowStack) = Finders1.FindLocWnd("InventoryPrimary");
            if (XlocChatWindowStack == 0)
            {
                General.Undock();
                ThreadManager.AllowDocking = false;
                ThreadManager.AllowDScan = true;
                return;
            }

            // unload
            var Inventory = Invent.GetInfo();
            var missiles = Inventory.Find(item => item.Name.Contains(Config.Missiles));
            Emulators.ClickRB(missiles.Pos.x, missiles.Pos.y);
            Thread.Sleep(AvgDeley + r.Next(-100, 100));
            General.ClickInContextMenu("Invert Selection");
            Thread.Sleep(AvgDeley + r.Next(-100, 100));

            var anyItem = Inventory.Find(item => !item.Name.Contains(Config.Missiles));

            Emulators.Drag(anyItem.Pos.x, anyItem.Pos.y, XlocChatWindowStack + 60, YlocChatWindowStack + 125);
            Thread.Sleep(AvgDeley + r.Next(-100, 100));

            General.Undock();
            ThreadManager.AllowDocking = false;
            ThreadManager.AllowDScan = true;
        }

        static public void StartLayRoute()
        {
            List<NotepadItem> NotepadInfo = NP.GetInfo();
            if (NotepadInfo == null)
                return;

            ThreadManager.AllowDScan = false;
            Thread.Sleep(3 * 1000);

            for (int i = 0; i < NotepadInfo.Count; i++)
            {
                Emulators.ClickRB(NotepadInfo[i].Pos.x, NotepadInfo[i].Pos.y);
                Thread.Sleep(AvgDeley + r.Next(-100, 100));
                var success = General.ClickInContextMenu("Add Waypoint");
                if (!success)
                {
                    Emulators.ClickLB(500, 100);
                }
                Thread.Sleep(AvgDeley + r.Next(-100, 100));
            }

            Emulators.ClickLB(500, 100);
            Thread.Sleep(AvgDeley + r.Next(-100, 100));
            ThreadManager.AllowDScan = true;
        }

        static public bool RemoveWaypoint()
        {
            for (int i = 0; i < 5; i++)
            {
                RoutePanel RouteInfo = NavPanel.GetRoutePanelInfo();
                if (RouteInfo.Systems == null)
                    return true;

                Console.WriteLine("remove waypoint");

                Emulators.ClickLB(RouteInfo.ButtonLoc.x, RouteInfo.ButtonLoc.y);
                Thread.Sleep(AvgDeley + r.Next(-100, 100));
                General.ClickInExpandedUtilMenu("Clear All Waypoints");
                Thread.Sleep(1000 + r.Next(-100, 100));
            }
            return false;
        }

        static public void CheckMissilesAmount()
        {
            var Inventory = Invent.GetInfo();
            if (Inventory == null)
            {
                Console.WriteLine("try to open inventary");
                Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_NUMPAD3);
                Thread.Sleep(2000 + r.Next(-100, 100));
                Inventory = Invent.GetInfo();
                if (Inventory == null)
                {
                    Console.WriteLine("inventary not found");
                    General.DockToStationAndExit();
                }
            }

            var MissilesAmount = Inventory.FindAll(item => item.Name.Contains(Config.Missiles))
                .Select(item => item.Amount).Sum();
            if (MissilesAmount < 1000)
            {
                Console.WriteLine("Not enough amount missiles = {0}", MissilesAmount);
                General.DockToStationAndExit();
            }
            else
            {
                Console.WriteLine("Amount missiles = {0}", MissilesAmount);
            }

            //if (Inventory.Find(item => item.Name.Contains(ModulesInfo.Missiles)).Amount < 100)
            //    General.DockToStationAndExit();

            //if (Checkers.CheckQuantityDrones() < 3)
            //    General.DockToStationAndExit();
        }

        static public UITreeNode GetUITrees()
        {
            return ReadMemory.GetUITrees(Window.RootAddress, Window.processId);
        }
    }
}
