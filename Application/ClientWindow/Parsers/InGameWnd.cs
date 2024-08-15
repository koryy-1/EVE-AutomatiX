using Application.ClientWindow.UIHandlers;
using Domen.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ClientWindow.Parsers
{
    public class InGameWnd
    {
        public Point WndCoords { get; set; }
        public Point WndCoords2 { get; set; }

        // todo: make protected instead public
        public Point GetCoordWindow(ClientParams clientProcess, string windowName)
        {
            var windowEntry = FindUiTree(clientProcess, windowName);
            if (windowEntry == null)
            {
                return new Point();
            }

            var point = new Point()
            {
                x = ExtractIntValue(windowEntry, "_displayX"),
                y = ExtractIntValue(windowEntry, "_displayY")
            };

            return point;
        }

        public int GetHeightWindow(ClientParams clientProcess, string windowName)
        {
            var windowEntry = FindUiTree(clientProcess, windowName);

            var windowHeight = ExtractIntValue(windowEntry, "_height");

            return windowHeight;
        }

        public int GetHeightEntity(UITreeNode entity)
        {
            return ExtractIntValue(entity, "_height");
        }

        public int GetWidthWindow(ClientParams clientProcess, string windowName)
        {
            var windowEntry = FindUiTree(clientProcess, windowName);

            var windowWidth = ExtractIntValue(windowEntry, "_displayWidth");

            return windowWidth;
        }

        public int GetWidthEntity(UITreeNode entity)
        {
            return ExtractIntValue(entity, "_displayWidth");
        }

        public Point GetCoordsEntityOnScreen(UITreeNode entity)
        {
            var point = new Point()
            {
                x = ExtractIntValue(entity, "_displayX"),
                y = ExtractIntValue(entity, "_displayY")
            };

            return point;
        }

        public Point GetCoordsEntityOnScreen2(UITreeNode entity)
        {
            var point = new Point()
            {
                x = ExtractIntValue(entity, "_displayX") + ExtractIntValue(entity, "_displayWidth"),
                y = ExtractIntValue(entity, "_displayY") + ExtractIntValue(entity, "_height")
            };

            return point;
        }

        public int ExtractIntValue(UITreeNode node, string key)
        {
            if (node.dictEntriesOfInterest[key] is JObject jObject)
            {
                return Convert.ToInt32(jObject["int_low32"]);
            }
            return Convert.ToInt32(node.dictEntriesOfInterest[key]);
        }

        public Distance ParseDistance(string rawDistance)
        {
            var numberPart = string.Join("", rawDistance.Split().SkipLast(1));
            var measurePart = rawDistance.Split().Last();
            // non-breaking space
            //.Replace("\u00A0", "");

            CultureInfo culture = new CultureInfo("en-US");
            if (measurePart == "AU")
            {
                if (numberPart.Contains(",") && !numberPart.Contains("."))
                    culture = new CultureInfo("ru-RU");
            }

            var value = double.Parse(numberPart, culture);

            var distance = new Distance()
            {
                Value = value,
                Measure = measurePart
            };

            return distance;
        }

        public Color GetColor(UITreeNode colorNode)
        {
            var Color = new Color();

            if (!colorNode.dictEntriesOfInterest.ContainsKey("_color"))
                return Color;

            Color.Alpha = Convert.ToInt32(colorNode.dictEntriesOfInterest["_color"]["aPercent"]);
            Color.Red = Convert.ToInt32(colorNode.dictEntriesOfInterest["_color"]["rPercent"]);
            Color.Green = Convert.ToInt32(colorNode.dictEntriesOfInterest["_color"]["gPercent"]);
            Color.Blue = Convert.ToInt32(colorNode.dictEntriesOfInterest["_color"]["bPercent"]);

            return Color;
        }

        public IEnumerable<UITreeNode> FindNodesByInterestKey(UITreeNode root, string key)
        {
            var nodes = new List<UITreeNode>();
            TraverseTree(root, node =>
            {
                if (node.dictEntriesOfInterest.ContainsKey(key))
                {
                    nodes.Add(node);
                }
            });
            return nodes;
        }

        public IEnumerable<UITreeNode> FindNodesByInterestName(UITreeNode root, string name)
        {
            var nodes = new List<UITreeNode>();
            TraverseTree(root, node =>
            {
                if (node.dictEntriesOfInterest.ContainsKey("_name") && node.dictEntriesOfInterest["_name"].ToString() == name)
                {
                    nodes.Add(node);
                }
            });
            return nodes;
        }

        public IEnumerable<UITreeNode> FindNodesByObjectName(UITreeNode root, string name)
        {
            var nodes = new List<UITreeNode>();
            TraverseTree(root, node =>
            {
                if (node.pythonObjectTypeName == name)
                {
                    nodes.Add(node);
                }
            });
            return nodes;
        }

        public void TraverseTree(UITreeNode node, Action<UITreeNode> action)
        {
            if (node == null)
                return;

            action(node);

            if (node.children != null)
            {
                foreach (var child in node.children)
                {
                    TraverseTree(child, action);
                }
            }
        }

        private UITreeNode FindUiTree(ClientParams clientProcess, string windowName)
        {
            var notHandledEntity = ReadMemory.GetUITrees(clientProcess.RootAddress, clientProcess.ProcessId)
                .FindEntityOfString(windowName);
            if (notHandledEntity == null)
            {
                return null;
            }
            return notHandledEntity.handleEntity(windowName)
                .GetMarkedChildEntity();
        }
    }
}
