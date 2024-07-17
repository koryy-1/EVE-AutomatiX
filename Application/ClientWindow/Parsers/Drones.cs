using Application.ClientWindow.Parsers;
using Application.ClientWindow.UIHandlers;
using Domen.Enums;
using Domen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.ClientWindow.Parsers
{
    public class Drones : InGameWnd
    {
        private ClientParams _clientParams;

        public Drones(ClientParams clientParams)
        {
            _clientParams = clientParams;
        }

        public List<Drone> GetInfo()
        {
            var dronesWindowEntry = UITreeReader.GetUITrees(_clientParams, "DronesWindow");
            if (dronesWindowEntry == null)
                return null;

            WndCoords = GetCoordsEntityOnScreen(dronesWindowEntry);
            WndCoords2 = GetCoordsEntityOnScreen2(dronesWindowEntry);

            var dronesInSpaceCount = GetDronesInSpaceCount(dronesWindowEntry);

            List<Drone> droneList = new List<Drone>();

            var droneInSpaceEntries = FindNodesByObjectName(dronesWindowEntry, "DroneInSpaceEntry");
            foreach (var droneNode in droneInSpaceEntries)
            {
                if (droneNode == null)
                    continue;
                if (!droneNode.HasValidChildren(new int[] { 0, 0, 0, 0 }))
                    continue;

                var drone = ExtractDroneInfo(droneNode, WndCoords, WndCoords2, dronesInSpaceCount);
                droneList.Add(drone);
            }

            var droneInBayEntries = FindNodesByObjectName(dronesWindowEntry, "DroneInBayEntry");
            foreach (var droneNode in droneInBayEntries)
            {
                if (droneNode == null)
                    continue;
                if (!droneNode.HasValidChildren(new int[] { 0, 0, 0, 0 }))
                    continue;

                var drone = ExtractDroneInfo(droneNode, WndCoords, WndCoords2);
                droneList.Add(drone);
            }
            
            return droneList;
        }

        private int GetDronesInSpaceCount(UITreeNode dronesWindowEntry)
        {
            var droneGroupHeaderInSpace = FindNodesByObjectName(dronesWindowEntry, "DroneGroupHeaderInSpace").FirstOrDefault();

            var dronesInSpaceCount = FindNodesByObjectName(droneGroupHeaderInSpace, "EveLabelMedium").FirstOrDefault()
                .dictEntriesOfInterest["_setText"].ToString()
                .Where(c => char.IsDigit(c)).FirstOrDefault().ToString();

            return Convert.ToInt32(dronesInSpaceCount);
        }

        private Drone ExtractDroneInfo(UITreeNode droneNode, Point wndCoords, Point wndCoords2, int dronesInSpaceCount = 0)
        {
            Drone drone = new Drone();

            drone.Pos = droneNode.pythonObjectTypeName == "DroneInSpaceEntry"
                ? GetPosOnWindowForSpace(droneNode, wndCoords2, dronesInSpaceCount)
                : GetPosOnWindowForBay(droneNode, wndCoords, wndCoords2);

            // todo: make automapping for drones for enum DroneLocation and workingMode
            //drone.Location = droneNode.pythonObjectTypeName == "DroneInSpaceEntry" ? DroneLocation.Space : DroneLocation.Bay;

            drone.Location = droneNode.pythonObjectTypeName == "DroneInSpaceEntry" ? "space" : "bay";

            drone.WorkMode = GetWorkMode(droneNode);

            // todo: make float values for hp
            drone.HealthPoints.Shield = GetGaugeValue(droneNode, "shieldGauge");
            drone.HealthPoints.Armor = GetGaugeValue(droneNode, "armorGauge");
            drone.HealthPoints.Structure = GetGaugeValue(droneNode, "structGauge");

            return drone;
        }

        private Point GetPosOnWindowForSpace(UITreeNode node, Point wndCoords2, int dronesInSpaceCount)
        {
            var yRelationPos = ExtractIntValue(node, "_displayY");

            var pos = new Point()
            {
                x = wndCoords2.x - 130,
                y = wndCoords2.y - 20 - (dronesInSpaceCount * 40) + yRelationPos + 20
            };

            return pos;
        }

        private Point GetPosOnWindowForBay(UITreeNode node, Point wndCoords, Point wndCoords2)
        {
            var yRelationPos = ExtractIntValue(node, "_displayY");

            var pos = new Point()
            {
                x = wndCoords2.x - 130,
                y = wndCoords.y + 56 + yRelationPos + 12
            };

            return pos;
        }

        private int GetGaugeValue(UITreeNode node, string gaugeName)
        {
            var gaugeNode = FindNodesByInterestName(node, gaugeName).FirstOrDefault();
            if (gaugeNode != null && gaugeNode.children != null && gaugeNode.children.Length > 1 && gaugeNode.children[1] != null)
            {
                return Convert.ToInt32(gaugeNode.children[0].dictEntriesOfInterest["_displayWidth"]);
                // todo: 32 px = 100%
            }
            return 0;
        }

        private string GetWorkMode(UITreeNode node)
        {
            var entryLabelNode = FindNodesByInterestName(node, "entryLabel").FirstOrDefault();

            if (entryLabelNode.dictEntriesOfInterest["_setText"].ToString().Contains("Idle"))
                return "Idle";
            if (entryLabelNode.dictEntriesOfInterest["_setText"].ToString().Contains("Fighting"))
                return "Fighting";
            if (entryLabelNode.dictEntriesOfInterest["_setText"].ToString().Contains("Returning"))
                return "Returning";

            return null;
        }

        // todo: make method for Update drone window Position on screen
    }
}
