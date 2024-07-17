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
        private Dictionary<string, int> _columnIdxes;

        public OV(ClientParams clientParams)
        {
            _clientParams = clientParams;
        }

        public List<OverviewItem> GetInfo()
        {
            var Overview = UITreeReader.GetUITrees(_clientParams, "OverviewWindow");
            if (Overview == null)
                return null;

            WndCoords = GetCoordsEntityOnScreen(Overview);
            _columnIdxes = getColumnIdxes();

            var OverviewEntries = FindNodesByObjectName(Overview, "OverviewScrollEntry");

            List<OverviewItem> OverviewInfo = new List<OverviewItem>();

            foreach (var OverviewEntry in OverviewEntries)
            {
                if (OverviewEntry == null)
                    continue;
                if (!OverviewEntry.HasValidChildren(new int[] { 0 }))
                    continue;

                OverviewItem OverviewItemInfo = new OverviewItem();

                OverviewItemInfo.Pos = GetPosOnWindow(OverviewEntry, WndCoords);

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
            var distanceNode = FindNodeByColumnName(node, "Velocity");
            if (distanceNode == null)
                return 0;

            var rawValue = distanceNode.dictEntriesOfInterest["_text"].ToString();

            //todo: parse int64 from difference culture
            var speed = rawValue == "-" ? "0" : rawValue.Replace(",", "").Replace(" ", "");

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
            var distanceNode = FindNodeByColumnName(node, "Type");
            if (distanceNode == null)
                return null;

            return distanceNode.dictEntriesOfInterest["_text"].ToString();
        }

        private string GetName(UITreeNode node)
        {
            var distanceNode = FindNodeByColumnName(node, "Name");
            if (distanceNode == null)
                return null;

            return distanceNode.dictEntriesOfInterest["_text"].ToString();
        }

        private Distance GetDistance(UITreeNode node)
        {
            var distanceNode = FindNodeByColumnName(node, "Distance");
            if (distanceNode == null)
                return null;

            var rawDistance = distanceNode.dictEntriesOfInterest["_text"].ToString();

            var distance = ParseDistance(rawDistance);

            return distance;
        }

        private UITreeNode FindNodeByColumnName(UITreeNode node, string colName)
        {
            //foreach (var child in node.children)
            //{
            //    var offset = ExtractIntValue(child, "_displayX");
            //    if (offset > _columnIdxes[colName] &&
            //        offset < _columnIdxes[secondCol])
            //    {
            //        return child;
            //    }
            //}
            var child = node.children[_columnIdxes[colName]];
            return child;
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

        private Dictionary<string, int> getColumnIdxes()
        {
            // todo: parse column info from Headers
            // or it must be in config
            var columns = new Dictionary<string, int>()
            {
                { "Icon", 6 },
                { "Distance", 5 },
                { "Name", 4 },
                { "Type", 3 },
                { "Size", 2 },
                { "Velocity", 1 },
                { "Angular Velocity", 0 },
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
