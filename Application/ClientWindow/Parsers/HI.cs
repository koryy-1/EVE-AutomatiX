using Application.ClientWindow.Parsers;
using Application.ClientWindow.UIHandlers;
using Domen.Constants;
using Domen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.ClientWindow.Parsers
{
    public class HI : InGameWnd
    {
        private ClientParams _clientParams;

        public HI(ClientParams clientParams)
        {
            _clientParams = clientParams;
        }

        public HudInterface GetInfo()
        {
            HudInterface HudInterface = new HudInterface();

            HudInterface.Pos = GetCenterPos();

            HudInterface.HealthPoints = GetShipHP();

            HudInterface.CurrentSpeed = GetCurrentSpeed();

            HudInterface.AllModules = GetAllModulesInfo();

            HudInterface.ShipState = GetShipFlightMode();

            return HudInterface;
        }
        public Point GetCenterPos()
        {
            var HudContainer = GetHudContainer(_clientParams);
            if (HudContainer == null)
                return null;

            var hudContainerCoords = GetCoordsEntityOnScreen(HudContainer
                    .children[Convert.ToInt32(HudContainer.dictEntriesOfInterest["needIndex"])]
                    );

            var CenterHudContainerEntity = HudContainer.children[0].children[0];
            var centerHudContainerEntityCoords = GetCoordsEntityOnScreen(CenterHudContainerEntity);

            Point Pos = new Point();

            Pos.x = hudContainerCoords.x + centerHudContainerEntityCoords.x + 95;
            Pos.y = hudContainerCoords.y + 100;

            return Pos;
        }
        public HealthPoints GetShipHP()
        {
            var hudContainer = GetHudContainer(_clientParams);
            if (hudContainer == null)
                return null;

            var hudReadoutEntry = FindNodesByObjectName(hudContainer, "HudReadout").FirstOrDefault();
            if (hudReadoutEntry == null)
                return null;

            HealthPoints healthPoints = new HealthPoints()
            {
                Shield = GetHpValue(hudReadoutEntry, "shield"),
                Armor = GetHpValue(hudReadoutEntry, "armor"),
                Structure = GetHpValue(hudReadoutEntry, "structure")
            };

            return healthPoints;
        }

        private int GetHpValue(UITreeNode node, string hpType)
        {
            var hpNode = FindNodesByInterestName(node, hpType).FirstOrDefault();
            if (hpNode != null && hpNode.children != null && hpNode.children.Length > 1 && hpNode.children[0] != null)
            {
                var rawHp = hpNode.children[0].dictEntriesOfInterest["_setText"].ToString();
                return Convert.ToInt32(string.Join("", rawHp.Where(c => char.IsDigit(c))));
            }
            return 0;
        }

        public int GetCurrentSpeed()
        {
            var HudContainer = GetHudContainer(_clientParams);
            if (HudContainer == null)
            {
                return -1;
            }

            var CenterHudContainerEntity = HudContainer.children[0].children[0];

            if (CenterHudContainerEntity.children[5].children[0].children[0].children[0]
                .dictEntriesOfInterest["_setText"].ToString() != "(Warping)")
            {
                return Convert.ToInt32(CenterHudContainerEntity
                    .children[5].children[0].children[0].children[0]
                    .dictEntriesOfInterest["_setText"].ToString().Split('.')[0].Split()[0]);
            }
            return -1;
        }
        public List<Module> GetAllModulesInfo()
        {
            UITreeNode HudContainer = GetHudContainer(_clientParams);
            if (HudContainer == null)
                return null;

            var SlotsContainer = FindNodesByObjectName(HudContainer, "ShipSlot");

            List<Module> AllModules = new List<Module>();

            foreach (var slotNode in SlotsContainer)
            {
                if (slotNode == null)
                    continue;

                Module Module = new Module();

                Module.Name = GetModuleName(slotNode);

                Module.VirtualKey = GetVirtualKey(slotNode);

                Module.SlotNum = GetSlotNum(slotNode);

                Module.AmountСharges = GetAmountСharges(slotNode);

                Module.Mode = GetModuleWorkingMode(slotNode);

                Module.Type = GetModuleType(slotNode);

                AllModules.Add(Module);
            }

            return AllModules;
        }

        private string GetModuleWorkingMode(UITreeNode slotNode)
        {
            var buttonRampsNode = FindNodesByObjectName(slotNode, "ShipModuleButtonRamps").FirstOrDefault();

            var rotationNode = FindNodesByInterestKey(slotNode, "_rotation");

            double LeftRamp = buttonRampsNode != null
                ? GetRampValue(rotationNode.FirstOrDefault())
                : 0.0;

            double RightRamp = buttonRampsNode != null
                ? GetRampValue(rotationNode.LastOrDefault())
                : 0.0;

            var IsGlowInActiveMode = slotNode.children.Last().dictEntriesOfInterest["_name"].ToString();

            if (IsGlowInActiveMode == "glow")
            {
                return "glow";
            }
            else if (IsGlowInActiveMode == "busy")
            {
                return "busy";
            }
            else if (buttonRampsNode != null
                &&
                !(LeftRamp == Math.PI && RightRamp == Math.PI)
                &&
                IsGlowInActiveMode != "glow" && IsGlowInActiveMode != "busy")
            {
                return "reloading";
            }
            else if (
                //buttonRampsNode == null
                //&&
                IsGlowInActiveMode != "glow" && IsGlowInActiveMode != "busy")
            {
                return "idle";
            }
            else
            {
                return "N/A";
            }
        }

        private double GetRampValue(UITreeNode rotationNode)
        {
            return Convert.ToDouble(rotationNode.dictEntriesOfInterest["_rotation"].ToString() + "1");
        }

        private string GetModuleType(UITreeNode slotNode)
        {
            var moduleType = slotNode.dictEntriesOfInterest["_name"].ToString();
            if (moduleType.Contains("High"))
                return "high";
            else if (moduleType.Contains("Medium"))
                return "med";
            else if (moduleType.Contains("Low"))
                return "low";
            else
                return "N/A";
        }

        private int GetAmountСharges(UITreeNode slotNode)
        {
            var QuantityParent = FindNodesByInterestName(slotNode, "quantityParent").FirstOrDefault();
            if (QuantityParent == null)
                return 0;

            return Convert.ToInt32(QuantityParent.children[0].dictEntriesOfInterest["_setText"]);
        }

        private int GetSlotNum(UITreeNode slotNode)
        {
            var rawSlotNum = slotNode.dictEntriesOfInterest["_name"].ToString();

            return Convert.ToInt32(string.Join("", rawSlotNum.Where(c => char.IsDigit(c))));
        }

        private int GetVirtualKey(UITreeNode slotNode)
        {
            var slotNodeCoords = GetCoordsEntityOnScreen(slotNode);
            if (slotNodeCoords.y == 0)
                return slotNodeCoords.x / 51 + 0x70;
            return 0;
        }

        private ModuleName GetModuleName(UITreeNode slotNode)
        {
            var RawModuleName = slotNode.children[0].dictEntriesOfInterest["_name"].ToString();

            if (ModulesData.ModuleNamesDict.ContainsKey(RawModuleName))
                return ModulesData.ModuleNamesDict[RawModuleName];
            return ModuleName.None;
        }

        public ShipFlightMode GetShipFlightMode()
        {
            UITreeNode HudContainer = GetHudContainer(_clientParams);

            ShipFlightMode shipState = new ShipFlightMode();

            if (HudContainer == null)
                return shipState;

            var idx = 0;
            var StanceButtons = HudContainer.children[0].children[2].pythonObjectTypeName;
            if (StanceButtons == "StanceButtons")
                idx += 1;

            var indicationContainer = HudContainer.children[Convert.ToInt32(HudContainer.dictEntriesOfInterest["needIndex"])]
                .children[4 + idx];

            if (indicationContainer == null)
                return shipState;
            if (indicationContainer.children == null)
                return shipState;
            if (indicationContainer.children.Length < 2)
                return shipState;
            if (indicationContainer.children[0] == null)
                return shipState;
            if (!indicationContainer.children[0].dictEntriesOfInterest.ContainsKey("_setText"))
                return shipState;
            if (indicationContainer.children[1] == null)
                return shipState;
            if (!indicationContainer.children[1].dictEntriesOfInterest.ContainsKey("_setText"))
                return shipState;


            var itemAndDistance = indicationContainer.children[0].dictEntriesOfInterest["_setText"].ToString();
            var flightMode = indicationContainer.children[1].dictEntriesOfInterest["_setText"].ToString();

            shipState.ItemAndDistance = itemAndDistance;
            shipState.FlightMode = GetFlightMode(flightMode);

            // todo: separate cur item (type OVItem) and cur distance (type Distance) in output GetShipFlightMode()

            return shipState;
        }

        private FlightMode GetFlightMode(string rawFlightMode)
        {
            //Approaching
            //Aligning
            //Orbiting
            //Keeping at Range
            //Warping
            //Jumping
            //Click target

            if (rawFlightMode.Contains("Approaching"))
            {
                return FlightMode.Approaching;
            }
            if (rawFlightMode.Contains("Aligning"))
            {
                return FlightMode.Aligning;
            }
            else if (rawFlightMode.Contains("Orbiting"))
            {
                return FlightMode.Orbiting;
            }
            else if (rawFlightMode.Contains("Keeping at Range"))
            {
                return FlightMode.KeepingAtRange;
            }
            else if (rawFlightMode.Contains("Warp"))
            {
                return FlightMode.Warping;
            }
            else if (rawFlightMode.Contains("Jumping"))
            {
                return FlightMode.Jumping;
            }
            else if (rawFlightMode.Contains("Click target"))
            {
                return FlightMode.ClickTarget;
            }
            return FlightMode.None;
        }

        private UITreeNode GetHudContainer(ClientParams clientParams)
        {
            var HudContainer = UITreeReader.GetUITrees(clientParams, "ShipUI", 3);

            return HudContainer;
        }
    }
}
