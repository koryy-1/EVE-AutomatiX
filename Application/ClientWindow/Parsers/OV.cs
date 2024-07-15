using Application.ClientWindow.UIHandlers;
using Domen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.ClientWindow.Parsers
{
    public class OV : InGameWnd
    {
        private ClientParams _clientParams;
        private Point _wndCoords;
        private Dictionary<string, int> _columnInfo;

        public OV(ClientParams clientParams)
        {
            _clientParams = clientParams;
            _wndCoords = GetCoordWindow(_clientParams, "OverviewWindow");
            _columnInfo = getColumns();
        }
        public List<OverviewItem> GetInfo()
        {
            var Overview = UITreeReader.GetUITrees(_clientParams, "OverviewWindow");
            if (Overview == null)
                return null;

            var OverviewEntries = FindNodesByObjectName(Overview, "OverviewScrollEntry");

            List<OverviewItem> OverviewInfo = new List<OverviewItem>();

            foreach (var OverviewEntry in OverviewEntries)
            {
                if (OverviewEntry == null)
                    continue;
                if (!OverviewEntry.HasValidChildren(new int[] { 0 }))
                    continue;

                OverviewItem OverviewItemInfo = new OverviewItem();

                OverviewItemInfo.Pos = GetPosOnWindow(OverviewEntry, _wndCoords);

                OverviewItemInfo.Color = GetIconColor(OverviewEntry);

                OverviewItemInfo.IconType = GetIconType(OverviewEntry);

                OverviewItemInfo.TargetLocked = IsObjectLocked(OverviewEntry);

                OverviewItemInfo.AimOnTargetLocked = IsAimOnObject(OverviewEntry);

                OverviewItemInfo.Distance = GetDistance(OverviewEntry);

                OverviewItemInfo.Name = GetName(OverviewEntry);

                OverviewItemInfo.Type = GetSpaceObjectType(OverviewEntry);

                OverviewItemInfo.Speed = GetSpeed(OverviewEntry);

                OverviewInfo.Add(OverviewItemInfo);
            }

            return OverviewInfo;
        }

        private string GetIconType(UITreeNode overviewEntry)
        {
            // todo: aggressive / hostile / neutral
            return string.Empty;
        }

        private int GetSpeed(UITreeNode node)
        {
            var distanceNode = FindNodeByColumnWidth(node, "Velocity", "Angular Velocity");
            if (distanceNode == null)
                return 0;

            var rawValue = distanceNode.dictEntriesOfInterest["_text"].ToString();

            var speed = rawValue == "-" ? "0" : rawValue.Replace(",", "");

            return Convert.ToInt32(speed);
        }

        private bool IsAimOnObject(UITreeNode node)
        {
            var check = FindNodesByInterestName(node, "myActiveTargetIndicator").FirstOrDefault();

            return check != null;
        }

        private bool IsObjectLocked(UITreeNode node)
        {
            var check = FindNodesByInterestName(node, "targetedByMeIndicator");
            if (check.Any())
            {
                return true;
            }
            return false;
        }

        private string GetSpaceObjectType(UITreeNode node)
        {
            var distanceNode = FindNodeByColumnWidth(node, "Type", "Size");
            if (distanceNode == null)
                return null;

            return distanceNode.dictEntriesOfInterest["_text"].ToString();
        }

        private string GetName(UITreeNode node)
        {
            var distanceNode = FindNodeByColumnWidth(node, "Name", "Type");
            if (distanceNode == null)
                return null;

            return distanceNode.dictEntriesOfInterest["_text"].ToString();
        }

        private Distance GetDistance(UITreeNode node)
        {
            var distanceNode = FindNodeByColumnWidth(node, "Distance", "Name");
            if (distanceNode == null)
                return null;

            var rawDistance = distanceNode.dictEntriesOfInterest["_text"].ToString();

            var distance = parseDistance(rawDistance);

            return distance;
        }

        private UITreeNode FindNodeByColumnWidth(UITreeNode node, string firstCol, string secondCol)
        {
            foreach (var child in node.children)
            {
                var offset = ExtractIntValue(child, "_displayX");
                if (offset > _columnInfo[firstCol] &&
                    offset < _columnInfo[secondCol])
                {
                    return child;
                }
            }
            return null;
        }

        private Color GetIconColor(UITreeNode node)
        {
            var iconColorEntry = FindNodesByInterestName(node, "iconSprite").FirstOrDefault();

            return GetColor(iconColorEntry);
        }

        private Point GetPosOnWindow(UITreeNode node, Point wndCoords)
        {
            var yRelationPos = ExtractIntValue(node, "_displayY");

            var point = new Point()
            {
                x = wndCoords.x + 80,
                y = wndCoords.y + 100 + yRelationPos
            };

            return point;
        }

        private Dictionary<string, int> getColumns()
        {
            // todo: it must be in config
            var columns = new Dictionary<string, int>()
            {
                { "Icon", 3 },
                { "Distance", 30 },
                { "Name", 130 },
                { "Type", 240 },
                { "Size", 355 },
                { "Velocity", 430 },
                { "Angular Velocity", 490 },
            };

            return columns;
        }

        public string GetColorInfo(Color ComparedColor)
        {
            if (ComparedColor.Red == 100
            && ComparedColor.Green == 100
            && ComparedColor.Blue == 0)
                return "yellow";

            if (ComparedColor.Red == 55
            && ComparedColor.Green == 55
            && ComparedColor.Blue == 55)
                return "gray";

            if (ComparedColor.Red == 100
            && ComparedColor.Green == 10
            && ComparedColor.Blue == 10)
                return "red";

            if (ComparedColor.Red == 100
            && ComparedColor.Green == 100
            && ComparedColor.Blue == 100)
                return "white";

            return null;
        }
    }
}
