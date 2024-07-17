using Application.ClientWindow.UIHandlers;
using Domen.Enums;
using Domen.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ClientWindow.Parsers
{
    public class SI : InGameWnd
    {
        ClientParams _clientParams;
        private List<string> _spaceObjectActions;

        public SI(ClientParams clientParams)
        {
            _clientParams = clientParams;

            _spaceObjectActions = new List<string>()
            {
                "Approach", "AlignTo", "WarpTo",
                "Orbit", "OpenCargo", "Dock", "Jump", "ActivateGate",
                "KeepAtRange", "LockTarget", "UnLockTarget"
            };
        }

        public SelectedItemInfo GetInfo()
        {
            var selectedItemWnd = UITreeReader.GetUITrees(_clientParams, "SelectedItemWnd");
            if (selectedItemWnd == null)
                return null;

            WndCoords = GetCoordsEntityOnScreen(selectedItemWnd);
            WndCoords2 = GetCoordsEntityOnScreen2(selectedItemWnd);

            if (IsNoObject(selectedItemWnd))
                return new SelectedItemInfo();

            var selectedItemInfo = new SelectedItemInfo();

            selectedItemInfo.Name = GetName(selectedItemWnd);
            selectedItemInfo.Distance = GetDistance(selectedItemWnd);
            selectedItemInfo.Buttons = GetButtons(selectedItemWnd);

            return selectedItemInfo;
        }

        private bool IsNoObject(UITreeNode selectedItemWnd)
        {
            return FindNodesByInterestKey(selectedItemWnd, "_setText").ToList()
                .Exists(node => node.dictEntriesOfInterest["_setText"].ToString() == "No Object Selected");
        }

        private string GetName(UITreeNode selectedItemWnd)
        {
            var rawName = FindNodesByInterestKey(selectedItemWnd, "_setText").ToList()
                .Find(node => node.pythonObjectTypeName == "EveLabelMedium")
                .dictEntriesOfInterest["_setText"].ToString();

            return rawName.Split("<br>")[0];
        }

        private Distance GetDistance(UITreeNode selectedItemWnd)
        {
            var rawName = FindNodesByInterestKey(selectedItemWnd, "_setText").ToList()
                .Find(node => node.pythonObjectTypeName == "EveLabelMedium")
                .dictEntriesOfInterest["_setText"].ToString();

            if (rawName.Split("<br>").Length == 1)
                return null;

            var rawDistance = rawName.Split("<br>")[1];

            var distance = ParseDistance(rawDistance);

            return distance;
        }

        private List<SelectedItemButton> GetButtons(UITreeNode selectedItemWnd)
        {
            var selectedItemButtonNodes = FindNodesByObjectName(selectedItemWnd, "SelectedItemButton");

            List<SelectedItemButton> selectedItemButtons = new List<SelectedItemButton>();

            foreach (var item in selectedItemButtonNodes)
            {
                var btn = new SelectedItemButton();

                btn.Action = GetAction(item);

                btn.IsEnable = IsEnableBtn(item);

                btn.Pos = GetPosOnWindow(item);

                selectedItemButtons.Add(btn);
            }

            return selectedItemButtons;
        }

        private bool IsEnableBtn(UITreeNode item)
        {
            return GetColor(item.children[0]).Alpha > 70;
        }

        private string GetAction(UITreeNode item)
        {
            var btnName = item.dictEntriesOfInterest["_name"].ToString().Substring(12);

            if (_spaceObjectActions.Contains(btnName))
                return btnName;

            return null;
        }

        private Point GetPosOnWindow(UITreeNode node)
        {
            var xRelationPos = ExtractIntValue(node, "_displayX");

            var point = new Point()
            {
                x = WndCoords.x + 34 + xRelationPos,
                y = WndCoords2.y - 33
            };

            return point;
        }
    }
}
