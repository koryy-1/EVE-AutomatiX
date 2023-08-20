using EVE_AutomatiX.Models;
using EVE_AutomatiX.UIHandlers;
using EVE_Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EVE_Bot.Parsers
{
    static public class Invent
    {
        static public List<InventoryItem> GetInfo(ClientParams clientProcess)
        {
            var CargoTree1 = UITreeReader.GetUITrees(clientProcess, "InventoryPrimary");
            if (CargoTree1 == null)
            {
                return null;
            }

            var CargoTree = CargoTree1.FindEntityOfString("Row");
            if (CargoTree == null)
            {
                return null;
            }
            CargoTree = CargoTree.handleEntity("Row");

            var (XInventory, YInventory) = InGameWnd.GetCoordWindow(clientProcess, "InventoryPrimary");

            List<InventoryItem> InventoryItems = new List<InventoryItem>();

            var LeftSidebarWidth = InGameWnd.GetWidthWindow(clientProcess, "TreeViewEntryInventoryCargo");

            //Rows
            for (int i = 0; i < CargoTree.children.Length; i++)
            {
                if (CargoTree.children[i] == null)
                    continue;

                var XItem = 0;
                var YItem = 0;

                if (CargoTree.children[i].dictEntriesOfInterest["_displayY"] is Newtonsoft.Json.Linq.JObject)
                {
                    YItem = Convert.ToInt32(CargoTree.children[i].dictEntriesOfInterest["_displayY"]["int_low32"]);
                }
                else
                    YItem = Convert.ToInt32(CargoTree.children[i].dictEntriesOfInterest["_displayY"]);

                //Cols
                for (int k = 0; k < CargoTree.children[i].children.Length; k++)
                {
                    int index = 0;

                    if (CargoTree.children[i].children[k]?.children == null)
                        continue;
                    if (CargoTree.children[i].children[k].children.Length < 2)
                        continue;
                    if (CargoTree.children[i].children[k].children[0] == null)
                        continue;

                    if (CargoTree.children[i].children[k].children[0].pythonObjectTypeName == "Icon")
                        index += 1;
                    if (CargoTree.children[i].children[k].children[index].pythonObjectTypeName == "OmegaCloneOverlayIcon")
                        index += 1;
                    if (CargoTree.children[i].children[k].children[index].dictEntriesOfInterest["_name"].ToString() == "qtypar")
                        index += 1;

                    if (CargoTree.children[i].children[k].children[1] == null)
                        continue;
                    if (CargoTree.children[i].children[k].children[1].children == null)
                        continue;
                    if (CargoTree.children[i].children[k].children[index] == null)
                        continue;
                    if (CargoTree.children[i].children[k].children[index].children == null)
                        continue;
                    //if (CargoTree.children[i].children[k].children[1 + index].children.Length < 2)
                    //    continue;
                    //if (CargoTree.children[i].children[k].children[1 + index].children[1] == null)
                    //    continue;


                    var ChildItem = CargoTree.children[i].children[k].children.Last().children.Last();


                    if (CargoTree.children[i].children[k].dictEntriesOfInterest["_displayX"] is Newtonsoft.Json.Linq.JObject)
                        XItem = Convert.ToInt32(CargoTree.children[i].children[k].dictEntriesOfInterest["_displayX"]["int_low32"]);
                    else
                        XItem = Convert.ToInt32(CargoTree.children[i].children[k].dictEntriesOfInterest["_displayX"]);

                    if (!ChildItem.dictEntriesOfInterest.ContainsKey("_setText"))
                        continue;

                    InventoryItem Item = new InventoryItem();

                    Item.Pos.x = XInventory + XItem + LeftSidebarWidth + 10 + 35;
                    Item.Pos.y = YInventory + YItem + 80 + 40;

                    var ChildItemName = ChildItem
                        .dictEntriesOfInterest["_setText"].ToString();

                    Item.Name = ChildItemName;

                    foreach (var InvItemParam in CargoTree.children[i].children[k].children)
                    {
                        if (InvItemParam.dictEntriesOfInterest.ContainsKey("_name") &&
                        InvItemParam.dictEntriesOfInterest["_name"].ToString() == "qtypar")
                        {
                            var ChildQuantityStr = InvItemParam.children[0].dictEntriesOfInterest["_setText"].ToString();
                            if (ChildQuantityStr.Contains("K"))
                            {
                                ChildQuantityStr = ChildQuantityStr.Split(',')[0] + "000";
                            }

                            var Quantity = Convert.ToInt32(ChildQuantityStr.Replace(" ", ""));
                            Item.Amount = Quantity;
                            break;
                        }
                    }
                    

                    InventoryItems.Add(Item);
                }
            }
            return InventoryItems;
        }


        static public int GetVolumeInfo(ClientParams clientProcess)
        {
            var InventoryPrimary = UITreeReader.GetUITrees(clientProcess, "InventoryPrimary");
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

        static public int GetPriceInfo(ClientParams clientProcess)
        {
            var InventoryPrimary = UITreeReader.GetUITrees(clientProcess, "InventoryPrimary");
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
