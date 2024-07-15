using Application.ClientWindow.UIHandlers;
using Domen.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ClientWindow.Parsers
{
    public class InGameWnd
    {
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

        public Distance parseDistance(string rawDistance)
        {
            var value = Convert.ToInt32(rawDistance.Split()[0].Replace(",", "").Split('.')[0]);
            var distance = new Distance()
            {
                value = value,
                measure = rawDistance.Split().Last()
            };

            return distance;
        }

        public Color GetColor(UITreeNode colorNode)
        {
            var Color = new Color();

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
