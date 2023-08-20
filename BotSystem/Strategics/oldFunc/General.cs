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
    internal class General
    {
        static public Random r = new Random();
        static public int AvgDeley = Config.AverageDelay;
        static ModulesData ModulesInfo = new ModulesData();

        static public void ModuleActivityManager(string ModuleName, bool ModuleEnable, bool PrivilegeControl = false)
        {
            if (ModuleEnable)
                ActivateModule(ModuleName, PrivilegeControl);
            else
                DeactivateModule(ModuleName, PrivilegeControl);
        }

        static public void ActivateModule(string ModuleName, bool PrivilegeControl)
        {
            var AllModules = HI.GetAllModulesInfo(HI.GetHudContainer());

            if (!AllModules.Exists(item => item.Name == ModuleName))
            {
                Console.WriteLine($"Module {ModuleName} not found");
                return;
            }

            var DesiredModules = AllModules.FindAll(item => item.Name == ModuleName);

            foreach (var Module in DesiredModules)
            {
                if (Module.Mode != "idle")
                {
                    Console.WriteLine($"Module {Module.Name} on {Module.SlotNum} slot already active");
                    continue;
                }

                Emulators.PressButton(Module.VirtualKey, PrivilegeControl);
                Thread.Sleep(1000);
                var DesiredModule = HI.GetAllModulesInfo(HI.GetHudContainer())
                    .Find(item => item.Name == Module.Name && item.SlotNum == Module.SlotNum);

                if (DesiredModule.Mode == "glow")
                {
                    //Console.WriteLine($"{DesiredModule.Name} activated successfully");
                    continue;
                }
                else if (DesiredModule.Mode == "busy")
                {
                    Console.WriteLine($"{DesiredModule.Name} activated unsuccessfully");
                    continue;
                }
                else if (DesiredModule.Mode == "reloading")
                {
                    Console.WriteLine($"{DesiredModule.Name} is reloading");
                    continue;
                }
            }
        }

        static public void DeactivateModule(string ModuleName, bool PrivilegeControl)
        {
            var AllModules = HI.GetAllModulesInfo(HI.GetHudContainer());

            if (!AllModules.Exists(item => item.Name == ModuleName))
            {
                Console.WriteLine($"Module {ModuleName} not found");
                return;
            }

            var DesiredModule = AllModules.Find(item => item.Name == ModuleName);

            if (DesiredModule.Mode == "idle")
            {
                Console.WriteLine($"Module {ModuleName} is inactive");
                return;
            }

            Emulators.PressButton(DesiredModule.VirtualKey, PrivilegeControl);
            Thread.Sleep(1000);
            DesiredModule = HI.GetAllModulesInfo(HI.GetHudContainer()).Find(item => item.Name == ModuleName);

            if (DesiredModule.Mode == "busy")
            {
                Console.WriteLine($"{DesiredModule.Name} deactivated successfully");
                return;
            }
            else if (DesiredModule.Mode == "glow")
            {
                Console.WriteLine($"{DesiredModule.Name} deactivated unsuccessfully");
                return;
            }
            else if (DesiredModule.Mode == "reloading")
            {
                Console.WriteLine($"{DesiredModule.Name} is reloading");
                return;
            }
        }

        static public void EnsureUndocked()
        {
            //определить где находится корабль
            var PresenceGrid = GetUITrees().FindEntityOfString("OverView");
            if (PresenceGrid == null) // ship docked
            {
                Undock();
            }
        }

        static public void RepairShip()
        {
            Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_NUMPAD1);
            Thread.Sleep(1000 + r.Next(-100, 100));

            var RepairShop = GetUITrees().FindEntityOfString("RepairShopWindow");
            if (RepairShop == null)
                return;

            var (XRepairShop, YRepairShop) = Finders1.FindLocWnd("RepairShopWindow");
            var Height = Window.GetHeightWindow("RepairShopWindow");
            var Width = Window.GetWidthWindow("RepairShopWindow");

            var ItemEntries = RepairShop.handleEntity("RepairShopWindow").FindEntityOfString("Item").handleEntity("Item");
            for (int i = 0; i < ItemEntries.children.Length; i++)
            {
                var Item = ItemEntries.children[i];
                Item = Item.FindEntityOfStringByDictEntriesOfInterest("_setText", Config.ShipName);
                if (Item == null)
                    continue;

                int YEntity;
                if (Item.dictEntriesOfInterest["_displayY"] is Newtonsoft.Json.Linq.JObject)
                    YEntity = Convert.ToInt32(Item.dictEntriesOfInterest["_displayY"]["int_low32"]);
                else
                    YEntity = Convert.ToInt32(Item.dictEntriesOfInterest["_displayY"]);

                Emulators.ClickLB(XRepairShop + 40, YRepairShop + 107 + YEntity);
                Thread.Sleep(AvgDeley + r.Next(-100, 100));
                Emulators.ClickLB(XRepairShop + Width / 2, YRepairShop + Height - 10);
                Thread.Sleep(1000 + r.Next(-100, 100));

                var BTNRepairAll = GetUITrees().FindEntityOfString("RepairShopWindow").handleEntity("RepairShopWindow")
                    .FindEntityOfStringByDictEntriesOfInterest("_setText", "Repair All");
                if (BTNRepairAll == null)
                {
                    Console.WriteLine("ship is not damaged");
                    Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_NUMPAD1);
                    return;
                }

                Emulators.ClickLB(XRepairShop + Width / 2 + 40, YRepairShop + Height - 10);

                Thread.Sleep(2 * 1000 + r.Next(-100, 100));
                var HybridWindow = GetUITrees().FindEntityOfString("HybridWindow");
                if (HybridWindow == null)
                    return;

                var (XHybridWindow, YHybridWindow) = Finders1.FindLocWnd("HybridWindow");
                var HybridWindowHeight = Window.GetHeightWindow("HybridWindow");
                var HybridWindowWidth = Window.GetWidthWindow("HybridWindow");

                Emulators.ClickLB(XHybridWindow + HybridWindowWidth / 2 - 30, YHybridWindow + HybridWindowHeight - 10);
                Console.WriteLine("ship repaired");
                Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_NUMPAD1);
                return;
            }
            Console.WriteLine("Fail to repair ship");
            Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_NUMPAD1);
        }

        static public void Undock()
        {
            //var (XLobbyWndEntry, YLobbyWndEntry) = Finders.FindLocWnd("LobbyWnd");
            var LobbyWndEntry = GetUITrees().FindEntityOfString("LobbyWnd");
            if (LobbyWndEntry == null) // ship undocked
            {
                Console.WriteLine("not found LobbyWnd");
                return;
            }

            //LobbyWndEntry = GetUITrees().FindEntityOfString("LobbyWnd").handleEntity("LobbyWnd")
            //    .FindEntityOfStringByDictEntriesOfInterest("_name", "undockparent");
            //if (LobbyWndEntry == null)
            //{
            //    Console.WriteLine("not found LobbyWnd");
            //    return;
            //}

            //LobbyWndEntry = LobbyWndEntry.handleEntityByDictEntriesOfInterest("_name", "undockparent");

            //int XItem, YItem;

            //if (LobbyWndEntry.children[Convert.ToInt32(LobbyWndEntry.dictEntriesOfInterest["needIndex"])]
            //    .dictEntriesOfInterest["_displayY"] is Newtonsoft.Json.Linq.JObject)
            //{
            //    YItem = Convert.ToInt32(LobbyWndEntry.children[Convert.ToInt32(LobbyWndEntry.dictEntriesOfInterest["needIndex"])]
            //        .dictEntriesOfInterest["_displayY"]["int_low32"]);
            //}
            //else
            //    YItem = Convert.ToInt32(LobbyWndEntry.children[Convert.ToInt32(LobbyWndEntry.dictEntriesOfInterest["needIndex"])]
            //        .dictEntriesOfInterest["_displayY"]);

            //if (LobbyWndEntry.children[Convert.ToInt32(LobbyWndEntry.dictEntriesOfInterest["needIndex"])].children[0]
            //    .dictEntriesOfInterest["_displayX"] is Newtonsoft.Json.Linq.JObject)
            //{
            //    XItem = Convert.ToInt32(LobbyWndEntry.children[Convert.ToInt32(LobbyWndEntry.dictEntriesOfInterest["needIndex"])].children[0]
            //        .dictEntriesOfInterest["_displayX"]["int_low32"]);
            //}
            //else
            //    XItem = Convert.ToInt32(LobbyWndEntry.children[Convert.ToInt32(LobbyWndEntry.dictEntriesOfInterest["needIndex"])].children[0]
            //        .dictEntriesOfInterest["_displayX"]);

            for (int i = 0; i < 5; i++)
            {
                //Emulators.ClickLB(XLobbyWndEntry + XItem + 80, YLobbyWndEntry + YItem + 20);
                Emulators.PressButton((int)WinApi.VirtualKeyShort.VK_NUMPAD2);
                Thread.Sleep(1000);

                var CheckUndock = GetUITrees().FindEntityOfString("LobbyWnd").handleEntity("LobbyWnd")
                    .FindEntityOfStringByDictEntriesOfInterest("_setText", "Undocking");

                if (CheckUndock != null)
                {
                    Thread.Sleep(1000 * 15);//wait for 15 sec after undock
                    Emulators.ClickLB(500, 100);
                    Thread.Sleep(1000 * 3);
                    Console.WriteLine("undocked");
                    return;
                }
            }
            Console.WriteLine("fail to try undock");
        }

        static public void DockToStationAndExit()
        {
            (int XStationCoords, int YStationCoords) = Finders1.FindObjectByWordInOverview("Station");
            if (XStationCoords != 0)
            {
                GotoInActiveItem("Station", "Dock");
            }
            Environment.Exit(10);
        }

        static public bool Orbiting(string ObjectNameInGrid)
        {
            if (Checkers.CheckState("Orbiting", ObjectNameInGrid))
                return true;

            for (int i = 0; i < 5; i++)
            {
                var (X, Y) = Finders1.FindObjectByWordInOverview(ObjectNameInGrid);
                if (X == 0)
                {
                    Console.WriteLine($"No {ObjectNameInGrid} in grid");
                    return false;
                }
                GotoInActiveItem(ObjectNameInGrid, "Orbit");

                if (Checkers.CheckState("Orbiting", ObjectNameInGrid))
                    return true;
            }
            Console.WriteLine($"fail to orbiting {ObjectNameInGrid}");
            return false;
        }

        static public bool OrbitViaContextMenu(string ObjectNameInGrid, string OrbitRadius)
        {
            if (Checkers.CheckState("Orbiting", ObjectNameInGrid, OrbitRadius))
                return true;

            int X = 0, Y = 0;
            for (int i = 0; i < 5; i++)
            {
                (X, Y) = Finders1.FindObjectByWordInOverview(ObjectNameInGrid);
                if (X == 0)
                {
                    Console.WriteLine($"No {ObjectNameInGrid} in grid");
                    return false;
                }
                Emulators.ClickRB(X, Y);
                Thread.Sleep(1000);

                //list of main context menu
                if (!ClickInContextMenu("Orbit"))
                    continue;
                Thread.Sleep(200);
                //list of orbit distance menu
                if (!ClickInContextMenu(OrbitRadius))
                    continue;

                Thread.Sleep(AvgDeley + r.Next(-100, 100));

                if (Checkers.CheckState("Orbiting", ObjectNameInGrid, OrbitRadius))
                    return true;
            }
            Console.WriteLine($"fail to orbit {ObjectNameInGrid} via context menu");

            Orbiting(ObjectNameInGrid);

            return false;
        }

        static public bool ClickInContextMenu(string ContextMenuField)
        {
            var ContextMenuEntry = GetUITrees().FindEntityOfString("ContextMenu");
            if (ContextMenuEntry == null)
            {
                Console.WriteLine("not found ContextMenu");
                return false;
            }

            ContextMenuEntry = ContextMenuEntry.handleEntity("ContextMenu")
                .FindEntityOfStringByDictEntriesOfInterest("_setText", ContextMenuField);
            if (ContextMenuEntry == null)
            {
                Console.WriteLine($"not found {ContextMenuField} in ContextMenu");
                return false;
            }

            //find coords of context menu
            var (XContextMenuField, YContextMenuField) = GetCoordsEntityOnScreen(ContextMenuEntry
            .children[Convert.ToInt32(ContextMenuEntry.dictEntriesOfInterest["needIndex"])]
            );

            ContextMenuEntry = ContextMenuEntry.handleEntityByDictEntriesOfInterest("_setText", ContextMenuField);

            var (_, YOffset) = GetCoordsEntityOnScreen(ContextMenuEntry);

            Emulators.ClickLB(XContextMenuField + 40, YContextMenuField + YOffset + 14);
            return true;
        }

        static public bool ClickInExpandedUtilMenu(string ContextMenuField)
        {
            var ExpandedUtilMenuEntry = GetUITrees().FindEntityOfString("ExpandedUtilMenu");
            if (ExpandedUtilMenuEntry == null)
            {
                Console.WriteLine("not found ExpandedUtilMenu");
                return false;
            }

            ExpandedUtilMenuEntry = ExpandedUtilMenuEntry.handleEntity("ExpandedUtilMenu")
                .FindEntityOfStringByDictEntriesOfInterest("_setText", ContextMenuField);
            if (ExpandedUtilMenuEntry == null)
            {
                Console.WriteLine($"not found {ContextMenuField} in ContextMenu");
                return false;
            }

            //find coords of context menu
            var (XContextMenuField, YContextMenuField) = GetCoordsEntityOnScreen(ExpandedUtilMenuEntry
            .children[Convert.ToInt32(ExpandedUtilMenuEntry.dictEntriesOfInterest["needIndex"])]
            );

            ExpandedUtilMenuEntry = ExpandedUtilMenuEntry.handleEntityByDictEntriesOfInterest("_setText", ContextMenuField);

            var (_, YOffset) = GetCoordsEntityOnScreen(ExpandedUtilMenuEntry);

            Emulators.ClickLB(XContextMenuField + 40, YContextMenuField + YOffset + 10);
            return true;
        }

        static public (int, int) GetCoordsEntityOnScreen(UITreeNode Entity)
        {
            int XEntity = 0;
            int YEntity = 0;

            if (Entity.dictEntriesOfInterest["_displayX"] is Newtonsoft.Json.Linq.JObject)
                XEntity = Convert.ToInt32(Entity.dictEntriesOfInterest["_displayX"]["int_low32"]);
            else
                XEntity = Convert.ToInt32(Entity.dictEntriesOfInterest["_displayX"]);


            if (Entity.dictEntriesOfInterest["_displayY"] is Newtonsoft.Json.Linq.JObject)
                YEntity = Convert.ToInt32(Entity.dictEntriesOfInterest["_displayY"]["int_low32"]);
            else
                YEntity = Convert.ToInt32(Entity.dictEntriesOfInterest["_displayY"]);

            return (XEntity, YEntity);
        }

        static public (int, int) GetCoordsButtonActiveItem(string Maneuver)
        {
            var (XSelectedItem, YSelectedItem) = Finders1.FindLocWnd("ActiveItem");
            if (XSelectedItem == 0)
            {
                Console.WriteLine("not found ActiveItem");
                return (0, 0);
            }

            var SelectedItemEntry = GetUITrees().FindEntityOfString("ActiveItem").handleEntity("ActiveItem")
                .FindEntityOfStringByDictEntriesOfInterest("_name", $"selectedItem{Maneuver}");
            if (SelectedItemEntry == null)
            {
                Console.WriteLine($"not found selectedItem{Maneuver}");
                return (0, 0);
            }

            SelectedItemEntry = SelectedItemEntry.handleEntityByDictEntriesOfInterest("_name", $"selectedItem{Maneuver}");

            var (XManeuver, _) = GetCoordsEntityOnScreen(SelectedItemEntry
                .children[Convert.ToInt32(SelectedItemEntry.dictEntriesOfInterest["needIndex"])]
                );

            return (XSelectedItem + XManeuver + 16, YSelectedItem + 80);
        }

        static public void GotoInActiveItem(string ItemInSpace, string Maneuver)
        {
            //Approach
            //AlignTo
            //WarpTo

            //ActivateGate
            //OpenCargo
            //Dock
            //Orbit
            //Jump

            //KeepAtRange
            //LockTarget
            //UnLockTarget
            for (int i = 0; i < 5; i++)
            {
                var (X, Y) = Finders1.FindObjectByWordInOverview(ItemInSpace);
                if (X == 0)
                {
                    Console.WriteLine($"No {ItemInSpace} in grid");
                    return;
                }
                Emulators.AllowControlEmulator = false;
                Emulators.ClickLB(X, Y, PrivilegeControl: true);
                Thread.Sleep(200 + r.Next(-100, 100));

                var (XSelectedItem, YSelectedItem) = Finders1.FindLocWnd("ActiveItem");
                if (XSelectedItem == 0)
                {
                    Console.WriteLine("not found ActiveItem");
                    continue;
                }

                var SelectedItemEntry = GetUITrees().FindEntityOfString("ActiveItem").handleEntity("ActiveItem")
                    .FindEntityOfStringByDictEntriesOfInterest("_name", $"selectedItem{Maneuver}");
                if (SelectedItemEntry == null)
                {
                    Console.WriteLine($"not found selectedItem{Maneuver}");
                    continue;
                }

                SelectedItemEntry = SelectedItemEntry.handleEntityByDictEntriesOfInterest("_name", $"selectedItem{Maneuver}");

                var (XManeuver, _) = GetCoordsEntityOnScreen(SelectedItemEntry
                    .children[Convert.ToInt32(SelectedItemEntry.dictEntriesOfInterest["needIndex"])]
                    );

                Emulators.ClickLB(XSelectedItem + XManeuver + 16, YSelectedItem + 85, PrivilegeControl: true);
                Emulators.AllowControlEmulator = true;
                Thread.Sleep(120 + r.Next(-100, 100));

                if (Maneuver == "Approach" && Checkers.CheckState("Approaching"))
                    return;
                else if (Maneuver == "AlignTo" && Checkers.CheckState("Aligning"))
                    return;
                else if (Maneuver == "Orbit" && Checkers.CheckState("Orbiting", ItemInSpace))
                    return;
                else if (Maneuver == "WarpTo" && (Checkers.CheckState("Approaching") || Checkers.CheckState("Jumping")))
                    return;
                else if (Maneuver == "OpenCargo" && (Checkers.CheckDistance(ItemInSpace, 2500) || Checkers.CheckState("Approaching")))
                    return;
                else if (Maneuver == "Dock" &&
                    (Checkers.CheckState("Warp") || //Warp Drive Active
                    Checkers.CheckState("Destination") ||
                    Checkers.CheckState("Approaching") ||
                    //Checkers.CheckState("Establish Warp Vector") ||
                    Checkers.CheckState("Docking")))
                    return;
                else if (Maneuver == "ActivateGate")
                    return;
                else if (Maneuver == "KeepAtRange" && Checkers.CheckState("Keeping at Range"))
                    return;
            }
        }

        static public void EnsureFormFleet()
        {
            var FleetTabInChat = GetUITrees().FindEntityOfString("ChatWindowStack").FindEntityOfStringByDictEntriesOfInterest("_setText", "Fleet");
            if (FleetTabInChat != null)
            {
                Console.WriteLine("Fleet already formed");
                return;
            }

            //var (XFleet, YFleet) = Finders.FindLocWnd("FleetWindow");
            //if (XFleet != 0)
            //{
            //    Console.WriteLine("Fleet already formed");
            //    return;
            //}
            for (int i = 0; i < 5; i++)
            {
                var HudContainer = GetUITrees().FindEntityOfString("HudContainer").handleEntity("HudContainer");
                var (XHudContainer, YHudContainer) = GetCoordsEntityOnScreen(HudContainer
                        .children[Convert.ToInt32(HudContainer.dictEntriesOfInterest["needIndex"])]
                        );

                var CenterHudContainer = HudContainer.FindEntityOfString("CenterHudContainer").handleEntity("CenterHudContainer");
                var (XCenterHudContainer, _) = GetCoordsEntityOnScreen(CenterHudContainer
                        .children[Convert.ToInt32(CenterHudContainer.dictEntriesOfInterest["needIndex"])]
                        );

                Emulators.ClickRB(XHudContainer + XCenterHudContainer + 95, YHudContainer + 100);
                Thread.Sleep(AvgDeley + r.Next(-100, 100));
                ClickInContextMenu("Pilot");
                Thread.Sleep(AvgDeley + r.Next(-100, 100));
                ClickInContextMenu("Form Fleet With...");
                Thread.Sleep(2 * 1000);

                FleetTabInChat = GetUITrees().FindEntityOfString("ChatWindowStack").FindEntityOfStringByDictEntriesOfInterest("_setText", "Fleet");
                if (FleetTabInChat != null)
                {
                    Console.WriteLine("Fleet formed");
                    return;
                }
            }
            Console.WriteLine("fail to form fleet");
        }

        static public void SetSpeed(int Speed)
        {
            var HudContainer = GetUITrees().FindEntityOfString("HudContainer").handleEntity("HudContainer");
            var (XHudContainer, YHudContainer) = GetCoordsEntityOnScreen(HudContainer
                    .children[Convert.ToInt32(HudContainer.dictEntriesOfInterest["needIndex"])]
                    );

            var CenterHudContainer = HudContainer.FindEntityOfString("CenterHudContainer").handleEntity("CenterHudContainer");
            var (XCenterHudContainer, _) = GetCoordsEntityOnScreen(CenterHudContainer
                    .children[Convert.ToInt32(CenterHudContainer.dictEntriesOfInterest["needIndex"])]
                    );

            Emulators.AllowControlEmulator = false;
            if (Speed == 0)
                Emulators.ClickLB(XHudContainer + XCenterHudContainer + 45, YHudContainer + 133, PrivilegeControl: true); // stop
            if (Speed == 1)
                Emulators.ClickLB(XHudContainer + XCenterHudContainer + 97, YHudContainer + 155, PrivilegeControl: true); // speed 1400 m/s
            if (Speed == 2)
                Emulators.ClickLB(XHudContainer + XCenterHudContainer + 138, YHudContainer + 133, PrivilegeControl: true); // max speed
            Emulators.AllowControlEmulator = true;
        }

        static public void ChangeTab(string TabName)
        {
            var OverView = GetUITrees().FindEntityOfString("OverView");
            if (OverView == null)
            {
                Console.WriteLine("fail to change tab: not found overview");
                return;
            }
            var CurrentTab = OverView.FindEntityOfStringByDictEntriesOfInterest("_setText", $"Overview ({TabName}");
            if (CurrentTab != null)
                return;

            // change tab
            var (XOverViewLoc, YOverViewLoc) = Finders1.FindLocWnd("OverView");
            int XTabLoc = 0;
            var TabEntries = GetUITrees().FindEntityOfString("OverView").FindEntityOfString("Tab").handleEntity("Tab");
            for (int i = 0; i < TabEntries.children.Length; i++)
            {
                if (TabEntries.children[i].dictEntriesOfInterest["_name"].ToString().Contains(TabName))
                {
                    XTabLoc = Convert.ToInt32(TabEntries.children[i].dictEntriesOfInterest["_displayX"]);
                    //Console.WriteLine("XTabLoc = {0}", XTabLoc);
                }
            }
            while (CurrentTab == null)
            {
                Console.WriteLine("change tab");
                Emulators.ClickLB(XOverViewLoc + XTabLoc + 20, YOverViewLoc + 35);
                Thread.Sleep(300);
                CurrentTab = GetUITrees().FindEntityOfString("OverView")
                    .FindEntityOfStringByDictEntriesOfInterest("_setText", $"Overview ({TabName}");
            }
        }

        static public (int, int, int) FindItemInCargo(string ItemName)
        {
            var CargoTree1 = GetUITrees().FindEntityOfString("InventoryPrimary");
            if (CargoTree1 == null)
            {
                Console.WriteLine("no InventoryPrimary");
                DockToStationAndExit();
            }
            CargoTree1 = CargoTree1.handleEntity("InventoryPrimary");

            var ItemInCargo = CargoTree1.FindEntityOfStringByDictEntriesOfInterest("_setText", ItemName);
            if (ItemInCargo == null)
                return (0, 0, 0);

            var (XInventory, YInventory) = Finders1.FindLocWnd("InventoryPrimary");
            var CargoTree = CargoTree1.FindEntityOfString("Row").handleEntity("Row");
            //Rows
            for (int i = 0; i < CargoTree.children.Length; i++)
            {
                if (CargoTree.children[i] == null)
                    continue;

                var XItem = 0;
                var YItem = 0;

                if (CargoTree.children[i].dictEntriesOfInterest["_displayY"] is Newtonsoft.Json.Linq.JObject)
                {
                    YItem = Convert.ToInt32(CargoTree.children[i].dictEntriesOfInterest["_displayY"]["int_low32"]);
                }
                else
                    YItem = Convert.ToInt32(CargoTree.children[i].dictEntriesOfInterest["_displayY"]);

                //Cols
                for (int k = 0; k < CargoTree.children[i].children.Length; k++)
                {
                    int index = 0;

                    if (CargoTree.children[i].children[k]?.children == null)
                        continue;
                    if (CargoTree.children[i].children[k].children.Length < 2)
                        continue;
                    if (CargoTree.children[i].children[k].children[1]?.children == null)
                        continue;

                    if (CargoTree.children[i].children[k].children[0].pythonObjectTypeName == "OmegaCloneOverlayIcon")
                        index = 1;
                    if (CargoTree.children[i].children[k].children[1 + index].children.Length < 2)
                        continue;
                    if (CargoTree.children[i].children[k].children[1 + index].children[1] == null)
                        continue;


                    //GetUITrees().FindEntityOfString("InventoryPrimary").handleEntity("InventoryPrimary").FindEntityOfString("OmegaCloneOverlayIcon") != null

                    var ChildItem = CargoTree.children[i].children[k].children[1 + index].children[1];


                    if (CargoTree.children[i].children[k].dictEntriesOfInterest["_displayX"] is Newtonsoft.Json.Linq.JObject)
                        XItem = Convert.ToInt32(CargoTree.children[i].children[k].dictEntriesOfInterest["_displayX"]["int_low32"]);
                    else
                        XItem = Convert.ToInt32(CargoTree.children[i].children[k].dictEntriesOfInterest["_displayX"]);

                    if (!ChildItem.dictEntriesOfInterest.ContainsKey("_setText"))
                        continue;

                    var ChildItemName = ChildItem
                        .dictEntriesOfInterest["_setText"].ToString();
                    if (ChildItemName.Contains(ItemName))
                    {
                        var WidthLeftSidebar = Window.GetWidthWindow("TreeViewEntryInventoryCargo");

                        var ChildQuantityStr = CargoTree.children[i].children[k].children[index].children[0]
                            .dictEntriesOfInterest["_setText"].ToString().Replace(" ", "");
                        //Console.WriteLine(ChildQuantityStr);

                        var ChildQuantity = Convert.ToInt32(ChildQuantityStr);
                        Console.WriteLine("found {0}, Quantity = {1}", ItemName, ChildQuantity);
                        return (XInventory + XItem + WidthLeftSidebar + 10 + 35, YInventory + YItem + 80 + 40, ChildQuantity);
                    }
                }
            }
            Console.WriteLine("not found {0}", ItemName);
            return (0, 0, 0);
        }

        static public bool CheckCargo(int price = 50)
        {
            if (CheckCargoPrice(price))
            {
                return true;
            }
            else if (CheckVolumeCargo(Config.LimiteCargoVolumeForUnload))
            {
                return true;
            }
            return false;
        }

        static public bool CheckCargoPrice(int NeedPrice)
        {
            int CargoPrice = Invent.GetPriceInfo();
            int KK = 1000 * 1000;
            if (CargoPrice > NeedPrice * KK)
            {
                return true;
            }
            return false;
        }

        static public bool CheckVolumeCargo(int NeedVolume = 240)
        {
            int CargoVolume = Invent.GetVolumeInfo();
            if (CargoVolume > NeedVolume)
            {
                return true;
            }
            return false;
        }

        static public void WarpToSpot(string SpotName)
        {
            for (int i = 0; i < 5; i++)
            {
                Emulators.ClickRB(500, 100);
                Thread.Sleep(AvgDeley + r.Next(-100, 100));

                if (!ClickInContextMenu("Locations"))
                    continue;
                Thread.Sleep(AvgDeley + r.Next(-100, 100));

                if (!ClickInContextMenu(SpotName))
                    continue;
                Thread.Sleep(AvgDeley + r.Next(-100, 100));

                //if (!ClickInContextMenu("Warp to Location Within 0 m"))
                //    continue;
                ClickInContextMenu("Warp to Location Within 0 m");
                Thread.Sleep(8 * 1000 + r.Next(-100, 100));

                if (Checkers.CheckState("Warp"))
                {
                    Checkers.WatchState();
                    return;
                }
            }
            Environment.Exit(0);
        }

        static public void SortItems()
        {
            var (XInventory, YInventory) = Finders1.FindLocWnd("InventoryPrimary");

            var WidthLeftSidebar = Window.GetWidthWindow("InventoryPrimary");

            Emulators.ClickRB(XInventory + WidthLeftSidebar - 18, YInventory + 100);
            Thread.Sleep(AvgDeley + r.Next(-100, 100));
            ClickInContextMenu("Stack All");
            Thread.Sleep(2 * 1000);
        }

        static public void UnlockNotEnemyTargets(string ExcludeTarget = "")
        {
            List<OverviewItem> OverviewInfo = OV.GetInfo();

            List<string> ExcludeSomeTargets = new List<string>() { "Radiating Telescope", "Serpentis Supply Stronghold" };

            if (ExcludeTarget.Length != 0)
                ExcludeSomeTargets.Add(ExcludeTarget);

            for (int i = 0; i < OverviewInfo.Count; i++)
            {
                var IsCont = false;
                for (int j = 0; j < ExcludeSomeTargets.Count; j++)
                {
                    if (
                        OverviewInfo[i].Name.Contains(ExcludeSomeTargets[j])
                        ||
                        OverviewInfo[i].Type.Contains(ExcludeSomeTargets[j])
                        )
                    {
                        IsCont = true;
                    }
                }
                if (OV.GetColorInfo(OverviewInfo[i].Colors) is "red" && !IsCont)
                    continue;


                if (OverviewInfo[i].TargetLocked)
                {
                    Console.WriteLine("not enemy target locked");

                    Emulators.ClickLB(OverviewInfo[i].Pos.x, OverviewInfo[i].Pos.y);
                    Thread.Sleep(1000 + r.Next(-100, 100));
                    var (XUnlockBtn, YUnlockBtn) = GetCoordsButtonActiveItem("UnLockTarget");
                    Emulators.ClickLB(XUnlockBtn, YUnlockBtn);

                    Thread.Sleep(500 + r.Next(-100, 100));
                    Console.WriteLine("not enemy target unlocked");
                }
            }
        }

        static public void EnsureTargetIsAvailable()
        {
            //var TargetsInBar = TB.GetInfo();
            var OverviewTargetsInfo = OV.GetInfo().FindAll(item => item.TargetLocked);
            if (OverviewTargetsInfo.Count == 0)
                return;

            if (!AttackingTargetIsAvailable(OverviewTargetsInfo))
            {
                ChangeTarget(OverviewTargetsInfo);
            }
        }

        static public bool AttackingTargetIsAvailable(List<OverviewItem> OverviewTargetsInfo)
        {
            foreach (var Target in OverviewTargetsInfo)
            {
                if (Target.AimOnTargetLocked &&
                    (Target.Distance.measure == "km" && Target.Distance.value < Config.WeaponsRange || Target.Distance.measure == "m"))
                {
                    return true;
                }
            }
            return false;
        }

        static public void ChangeTarget(List<OverviewItem> OverviewTargetsInfo)
        {
            OverviewItem Target = new OverviewItem();
            foreach (var item in OverviewTargetsInfo)
            {
                if (item.Distance.measure == "km" && item.Distance.value < Config.WeaponsRange || item.Distance.measure == "m")
                {
                    Target = item;
                    break;
                }
            }
            if (Target == null)
            {
                return;
            }
            ModuleActivityManager(ModulesInfo.MissileLauncher, false);

            for (int i = 0; i < 5; i++) // 15 sec
            {
                var AllModules = HI.GetAllModulesInfo(HI.GetHudContainer());
                bool IsIdle = false;
                foreach (var Module in AllModules)
                {
                    if (Module.Type == "high" && Module.Mode == "idle")
                    {
                        IsIdle = true;
                        break;
                    }
                }
                if (IsIdle)
                    break;

                Thread.Sleep(3 * 1000);
            }

            Emulators.ClickLB(Target.Pos.x, Target.Pos.y);
            Thread.Sleep(AvgDeley + r.Next(-100, 100));
            ModuleActivityManager(ModulesInfo.MissileLauncher, true);
            Console.WriteLine("target changed");
        }

        static public bool LockedTargetsAreAvailable()
        {
            var OverviewTargetsInfo = OV.GetInfo().FindAll(item => item.TargetLocked);
            //if (OverviewTargetsInfo.Count == 0 || OverviewTargetsInfo.Count < 4)
            //    return true;

            foreach (var Target in OverviewTargetsInfo)
            {
                if (Target.Distance.measure == "km" && Target.Distance.value < Config.WeaponsRange || Target.Distance.measure == "m")
                {
                    return true;
                }
            }
            Console.WriteLine("LockedTargets not available");
            return false;
        }

        static public void UnlockTargets()
        {
            var OverviewTargetsInfo = OV.GetInfo().FindAll(item => item.TargetLocked);
            //if (OverviewTargetsInfo.Count == 0 || OverviewTargetsInfo.Count < 4)
            //    return;

            foreach (var Target in OverviewTargetsInfo/*.SkipLast(1)*/)
            {
                Console.WriteLine("unlock targets");

                Emulators.ClickLB(Target.Pos.x, Target.Pos.y);
                Thread.Sleep(200 + r.Next(-100, 100));
                var (XUnlockBtn, YUnlockBtn) = GetCoordsButtonActiveItem("UnLockTarget");
                if (XUnlockBtn == 0)
                    continue;

                Emulators.ClickLB(XUnlockBtn, YUnlockBtn);

                Thread.Sleep(200 + r.Next(-100, 100));
                Console.WriteLine("target unlocked");
            }
        }

        static public UITreeNode GetUITrees()
        {
            return ReadMemory.GetUITrees(Window.RootAddress, Window.processId);
        }
    }
}
