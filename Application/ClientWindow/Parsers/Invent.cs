using Application.ClientWindow.UIHandlers;
using Domen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.ClientWindow.Parsers
{
    public class Invent : InGameWnd
    {
        private ClientParams _clientParams;
        private int _leftSidebarWidth;

        public Invent(ClientParams clientParams)
        {
            _clientParams = clientParams;
        }

        public List<InventoryItem> GetInfo()
        {
            var inventoryPrimary = UITreeReader.GetUITrees(_clientParams, "InventoryPrimary");
            if (inventoryPrimary == null)
                return null;

            WndCoords = GetCoordsEntityOnScreen(inventoryPrimary);
            WndCoords2 = GetCoordsEntityOnScreen2(inventoryPrimary);
            _leftSidebarWidth = GetWidthWindow(_clientParams, "TreeViewEntryInventoryCargo") + 10;

            var cargoTree = FindNodesByObjectName(inventoryPrimary, "Row");

            List<InventoryItem> InventoryItems = new List<InventoryItem>();

            foreach (var rowNode in cargoTree)
            {
                if (!rowNode.HasValidChildren(new int[] { 0, 0, 0 }))
                    continue;

                var YItem = ExtractIntValue(rowNode, "_displayY");

                var rowItems = FindNodesByObjectName(rowNode, "InvItem");

                foreach (var itemNode in rowItems)
                {
                    if (!rowNode.HasValidChildren(new int[] { 0 }))
                        continue;

                    var XItem = ExtractIntValue(itemNode, "_displayX");

                    InventoryItem Item = new InventoryItem();

                    Item.Pos = new Point()
                    {
                        x = WndCoords.x + XItem + _leftSidebarWidth + 10 + 35,
                        y = WndCoords.y + YItem + 80 + 40
                    };

                    Item.Name = GetName(itemNode);

                    Item.Amount = GetAmount(itemNode);

                    InventoryItems.Add(Item);
                }
            }
            return InventoryItems;
        }

        private string GetName(UITreeNode itemNode)
        {
            var nameNode = FindNodesByInterestName(itemNode, "itemNameLabel").FirstOrDefault();
            if (nameNode == null)
                return null;

            return nameNode.dictEntriesOfInterest["_setText"].ToString();
        }

        private int GetAmount(UITreeNode itemNode)
        {
            var qtyparNode = FindNodesByInterestName(itemNode, "qtypar").FirstOrDefault();
            if (qtyparNode == null)
                return 1;

            var rawAmount = FindNodesByInterestKey(qtyparNode, "_setText").FirstOrDefault()
                .dictEntriesOfInterest["_setText"].ToString();

            if (rawAmount.Contains("K"))
            {
                rawAmount = rawAmount.Split('.')[0] + "000";
            }

            rawAmount = rawAmount.Replace(",", "");

            return Convert.ToInt32(rawAmount);
        }

        public Point GetLootAllBtnPos()
        {
            var inventoryPrimary = UITreeReader.GetUITrees(_clientParams, "InventoryPrimary");
            if (inventoryPrimary == null)
                return null;

            WndCoords = GetCoordsEntityOnScreen(inventoryPrimary);
            WndCoords2 = GetCoordsEntityOnScreen2(inventoryPrimary);

            var invLootAllBtnNode = FindNodesByInterestName(inventoryPrimary, "invLootAllBtn").FirstOrDefault();
            if (invLootAllBtnNode == null)
                return null;

            var point = new Point()
            {
                x = WndCoords2.x - 50,
                y = WndCoords2.y - 30
            };

            return point;
        }

        public int GetVolumeInfo()
        {
            var InventoryPrimary = UITreeReader.GetUITrees(_clientParams, "InventoryPrimary");
            if (InventoryPrimary == null)
            {
                Console.WriteLine("InventoryPrimary not found");
                return -1;
            }
            InventoryPrimary = InventoryPrimary.FindEntityOfStringByDictEntriesOfInterest("_name", "capacityText");
            if (InventoryPrimary == null)
            {
                Console.WriteLine("capacityText not found");
                return -1;
            }
            var InventoryPrimaryEntry = InventoryPrimary.handleEntityByDictEntriesOfInterest("_name", "capacityText");

            try
            {
                var totalPriceLabel = InventoryPrimaryEntry.children[Convert.ToInt32(InventoryPrimaryEntry.dictEntriesOfInterest["needIndex"])]
                .dictEntriesOfInterest;

                if (totalPriceLabel["_name"].ToString() == "capacityText")
                {
                    var CargoVolume = totalPriceLabel["_setText"].ToString();
                    int position = CargoVolume.IndexOf("/");
                    CargoVolume = CargoVolume.Substring(0, position);
                    position = CargoVolume.IndexOf(",");
                    if (position > -1)
                    {
                        CargoVolume = CargoVolume.Substring(0, position);
                    }

                    int Volume;
                    int.TryParse(string.Join("", CargoVolume.Where(c => char.IsDigit(c))), out Volume);
                    //Console.WriteLine("Volume = " + Volume);

                    return Volume;
                }
            }
            catch
            {
                Console.WriteLine("capacityText in InventoryPrimary not found");
            }
            return -1;
        }

        public int GetPriceInfo()
        {
            var InventoryPrimary = UITreeReader.GetUITrees(_clientParams, "InventoryPrimary");
            if (InventoryPrimary == null)
            {
                Console.WriteLine("InventoryPrimary not found");
                return -1;
            }
            InventoryPrimary = InventoryPrimary.FindEntityOfStringByDictEntriesOfInterest("_name", "totalPriceLabel");
            if (InventoryPrimary == null)
            {
                Console.WriteLine("totalPriceLabel not found");
                return -1;
            }
            var InventoryPrimaryEntry = InventoryPrimary.handleEntityByDictEntriesOfInterest("_name", "totalPriceLabel");

            try
            {
                var totalPriceLabel = InventoryPrimaryEntry.children[Convert.ToInt32(InventoryPrimaryEntry.dictEntriesOfInterest["needIndex"])]
                .dictEntriesOfInterest;

                if (totalPriceLabel["_name"].ToString() == "totalPriceLabel")
                {
                    var CargoPrice = totalPriceLabel["_setText"].ToString();
                    int PriceValue;
                    int.TryParse(string.Join("", CargoPrice.Where(c => char.IsDigit(c))), out PriceValue);
                    //Console.WriteLine("PriceValue = " + PriceValue + " CargoPrice = " + CargoPrice);
                    
                    return PriceValue;
                }
            }
            catch
            {
                Console.WriteLine("totalPriceLabel in InventoryPrimary not found");
            }
            return -1;
        }
    }
}
