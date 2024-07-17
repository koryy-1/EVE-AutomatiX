using Application.ClientWindow.UIHandlers;
using Domen.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ClientWindow.Parsers
{
    public class PS : InGameWnd
    {
        private ClientParams _clientParams;
        private int _wndWidth;

        public PS(ClientParams clientParams)
        {
            _clientParams = clientParams;
        }

        public List<ProbeScanItem> GetInfo()
        {
            var probeScannerWindow = UITreeReader.GetUITrees(_clientParams, "ProbeScannerWindow");
            if (probeScannerWindow == null)
                return null;

            WndCoords = GetCoordsEntityOnScreen(probeScannerWindow);
            _wndWidth = GetWidthEntity(probeScannerWindow);

            var scanResultEntries = FindNodesByObjectName(probeScannerWindow, "ScanResultNew");

            List<ProbeScanItem> probeScanResults = new List<ProbeScanItem>();

            foreach (var scanResultEntry in scanResultEntries)
            {
                if (scanResultEntry == null)
                    continue;
                if (!scanResultEntry.HasValidChildren(new int[] { 0 }))
                    continue;

                ProbeScanItem probeScanItem = new ProbeScanItem();

                probeScanItem.Pos = GetPosOnWindow(scanResultEntry, WndCoords);

                probeScanItem.Distance = GetDistance(scanResultEntry);

                probeScanItem.ID = GetID(scanResultEntry);

                probeScanItem.Name = GetName(scanResultEntry);

                probeScanItem.Group = GetGroup(scanResultEntry);

                probeScanResults.Add(probeScanItem);
            }

            return probeScanResults;
        }

        private Distance GetDistance(UITreeNode scanResultEntry)
        {
            var fields = FindNodesByObjectName(scanResultEntry, "EveLabelMedium").Skip(1);

            var rawValue = fields.ToList()[0].dictEntriesOfInterest["_setText"].ToString();

            var distance = ParseDistance(rawValue);

            return distance;
        }

        private string GetID(UITreeNode scanResultEntry)
        {
            var fields = FindNodesByObjectName(scanResultEntry, "EveLabelMedium").Skip(1).ToList();

            return fields[1].dictEntriesOfInterest["_setText"].ToString();
        }

        private string GetName(UITreeNode scanResultEntry)
        {
            var fields = FindNodesByObjectName(scanResultEntry, "EveLabelMedium").Skip(1).ToList();

            return fields[2].dictEntriesOfInterest["_setText"].ToString();
        }

        private string GetGroup(UITreeNode scanResultEntry)
        {
            var fields = FindNodesByObjectName(scanResultEntry, "EveLabelMedium").Skip(1).ToList();
            if (!fields[3].dictEntriesOfInterest.ContainsKey("_setText"))
                return null;

            return fields[3].dictEntriesOfInterest["_setText"].ToString();
        }

        private Point GetPosOnWindow(UITreeNode scanResultEntry, Point wndCoords)
        {
            var yRelationPos = ExtractIntValue(scanResultEntry, "_displayY");

            var point = new Point()
            {
                x = wndCoords.x + _wndWidth - 30,
                y = wndCoords.y + 93 + yRelationPos
            };

            return point;
        }
    }
}
