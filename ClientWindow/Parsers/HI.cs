﻿using EVE_AutomatiX.Models;
using EVE_AutomatiX.UIHandlers;
using EVE_AutomatiX.Utils;
using EVE_Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EVE_Bot.Parsers
{
    public class HI : InGameWnd
    {
        ClientParams _clientParams;

        public HI(ClientParams clientParams)
        {
            _clientParams = clientParams;
        }
        public HudInterface GetInfo()
        {
            HudInterface HudInterface = new HudInterface();

            //Pos
            HudInterface.Pos = GetCenterPos();


            //HP
            HudInterface.HealthPoints = GetShipHP();


            //CurrentSpeed
            HudInterface.CurrentSpeed = GetCurrentSpeed();


            //Module
            HudInterface.AllModules = GetAllModulesInfo();


            return HudInterface;
        }
        public Point GetCenterPos()
        {
            var HudContainer = GetHudContainer(_clientParams);
            if (HudContainer == null)
            {
                return null;
            }

            var (XHudContainer, YHudContainer) = GetCoordsEntityOnScreen(HudContainer
                    .children[Convert.ToInt32(HudContainer.dictEntriesOfInterest["needIndex"])]
                    );

            var CenterHudContainerEntity = HudContainer.children[0].children[0];
            var (XCenterHudContainer, _) = GetCoordsEntityOnScreen(CenterHudContainerEntity);

            Point Pos = new Point();

            Pos.x = XHudContainer + XCenterHudContainer + 95;
            Pos.y = YHudContainer + 100;

            return Pos;
        }
        public HealthPoints GetShipHP()
        {
            var HudContainer = GetHudContainer(_clientParams);
            if (HudContainer == null)
            {
                return null;
            }

            var idx = 0;
            var StanceButtons = HudContainer.children[0].children[2].pythonObjectTypeName;
            if (StanceButtons == "StanceButtons")
                idx += 1;

            var HudReadoutEntry = HudContainer.children[0].children[5 + idx];

            var ShieldStr = HudReadoutEntry
                .children[0].children[0].dictEntriesOfInterest["_setText"].ToString();
            int Shield;
            int.TryParse(string.Join("", ShieldStr.Where(c => char.IsDigit(c))), out Shield);

            var ArmorStr = HudReadoutEntry
                .children[1].children[0].dictEntriesOfInterest["_setText"].ToString();
            int Armor;
            int.TryParse(string.Join("", ArmorStr.Where(c => char.IsDigit(c))), out Armor);

            var StructureStr = HudReadoutEntry
                .children[2].children[0].dictEntriesOfInterest["_setText"].ToString();
            int Structure;
            int.TryParse(string.Join("", StructureStr.Where(c => char.IsDigit(c))), out Structure);

            HealthPoints HealthPoints = new HealthPoints();
            HealthPoints.Shield = Shield;
            HealthPoints.Armor = Armor;
            HealthPoints.Structure = Structure;

            return HealthPoints;
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
            {
                return null;
            }

            var SlotsContainer = HudContainer.FindEntityOfString("ShipSlot").handleEntity("ShipSlot");

            List<Module> AllModules = new List<Module>();

            for (int i = 0; i < SlotsContainer.children.Length; i++)
            {
                if (SlotsContainer.children[i] == null)
                    continue;

                Module Module = new Module();

                //Name
                var RawModuleName = SlotsContainer.children[i].children[0].dictEntriesOfInterest["_name"].ToString();

                if (ModulesData.ModuleNamesDict.ContainsKey(RawModuleName))
                    Module.Name = ModulesData.ModuleNamesDict[RawModuleName];

                //Virtual key
                var (X, Y) = GetCoordsEntityOnScreen(SlotsContainer.children[i]);
                if (Y == 0)
                    Module.VirtualKey = X / 51 + 0x70;

                //SlotNum
                var ModuleType = SlotsContainer.children[i].dictEntriesOfInterest["_name"].ToString();

                int SlotNum;
                int.TryParse(string.Join("", ModuleType.Where(c => char.IsDigit(c))), out SlotNum);
                Module.SlotNum = SlotNum;

                //quantityParent
                var QuantityParent = SlotsContainer.children[i].children[0].children[0];
                if (QuantityParent.dictEntriesOfInterest.ContainsKey("_name")
                    &&
                    QuantityParent.dictEntriesOfInterest["_name"].ToString() == "quantityParent")
                {
                    Module.AmountСharges = Convert.ToInt32(QuantityParent.children[0]
                    .dictEntriesOfInterest["_setText"]);
                }

                //Mode
                var RampIsActive = SlotsContainer.children[i].children[2].pythonObjectTypeName;

                var LeftRampStr = "";
                var RightRampStr = "";

                double LeftRamp = 0.0;
                double RightRamp = 0.0;
                if (RampIsActive == "ShipModuleButtonRamps")
                {
                    LeftRampStr = SlotsContainer.children[i].children[2].children[0].children[0]
                        .dictEntriesOfInterest["_rotation"].ToString() + "1";
                    RightRampStr = SlotsContainer.children[i].children[2].children[1].children[0]
                        .dictEntriesOfInterest["_rotation"].ToString() + "1";

                    double.TryParse(LeftRampStr, out LeftRamp);
                    double.TryParse(RightRampStr, out RightRamp);
                }


                var IsGlowInActiveMode = SlotsContainer.children[i].children.Last().dictEntriesOfInterest["_name"].ToString();

                if (IsGlowInActiveMode == "glow")
                {
                    Module.Mode = "glow";
                }
                else if (IsGlowInActiveMode == "busy")
                {
                    Module.Mode = "busy";
                }
                else if (RampIsActive == "ShipModuleButtonRamps"
                    &&
                    !(LeftRamp == Math.PI && RightRamp == Math.PI)
                    &&
                    IsGlowInActiveMode != "glow" && IsGlowInActiveMode != "busy")
                {
                    Module.Mode = "reloading";
                }
                else if (
                    //RampIsActive != "ShipModuleButtonRamps"
                    //&&
                    IsGlowInActiveMode != "glow" && IsGlowInActiveMode != "busy")
                {
                    Module.Mode = "idle";
                }

                //Type
                if (ModuleType.Contains("High"))
                {
                    Module.Type = "high";
                }
                else if (ModuleType.Contains("Medium"))
                {
                    Module.Type = "med";
                }
                else if (ModuleType.Contains("Low"))
                {
                    Module.Type = "low";
                }
                AllModules.Add(Module);
            }

            return AllModules;
        }
        public ShipFlightMode GetShipFlightMode()
        {
            UITreeNode HudContainer = GetHudContainer(_clientParams);

            ShipFlightMode ShipState = new ShipFlightMode();

            if (HudContainer == null)
            {
                return ShipState;
            }

            var idx = 0;
            var StanceButtons = HudContainer.children[0].children[2].pythonObjectTypeName;
            if (StanceButtons == "StanceButtons")
                idx += 1;

            var indicationContainer = HudContainer.children[Convert.ToInt32(HudContainer.dictEntriesOfInterest["needIndex"])]
                .children[4 + idx];

            if (indicationContainer == null)
                return ShipState;
            if (indicationContainer.children == null)
                return ShipState;
            if (indicationContainer.children.Length < 2)
                return ShipState;
            if (indicationContainer.children[0] == null)
                return ShipState;
            if (!indicationContainer.children[0].dictEntriesOfInterest.ContainsKey("_setText"))
                return ShipState;
            if (indicationContainer.children[1] == null)
                return ShipState;
            if (!indicationContainer.children[1].dictEntriesOfInterest.ContainsKey("_setText"))
                return ShipState;


            var CurrentItemAndDistance = indicationContainer.children[0].dictEntriesOfInterest["_setText"].ToString();
            var CurrentFlightMode = indicationContainer.children[1].dictEntriesOfInterest["_setText"].ToString();

            ShipState.CurrentItemAndDistance = CurrentItemAndDistance;
            ShipState.CurrentFlightMode = GetFlightMode(CurrentFlightMode);

            // todo: separate cur item (type OVItem) and cur distance (type Distance) in output GetShipFlightMode()

            return ShipState;
        }

        private FlightMode GetFlightMode(string RawFlightMode)
        {
            //Approaching
            //Aligning
            //Orbiting
            //Keeping at Range
            //Warping
            //Jumping
            //Click target

            if (RawFlightMode.Contains("Approaching"))
            {
                return FlightMode.Approaching;
            }
            if (RawFlightMode.Contains("Aligning"))
            {
                return FlightMode.Aligning;
            }
            else if (RawFlightMode.Contains("Orbiting"))
            {
                return FlightMode.Orbiting;
            }
            else if (RawFlightMode.Contains("Keeping at Range"))
            {
                return FlightMode.KeepingAtRange;
            }
            else if (RawFlightMode.Contains("Warp"))
            {
                return FlightMode.Warping;
            }
            else if (RawFlightMode.Contains("Jumping"))
            {
                return FlightMode.Jumping;
            }
            else if (RawFlightMode.Contains("Click target"))
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
