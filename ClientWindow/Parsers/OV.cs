using EVE_AutomatiX.Models;
using EVE_AutomatiX.UIHandlers;
using EVE_Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EVE_Bot.Parsers
{
    static public class OV
    {
        static public List<OverviewItem> GetInfo(ClientParams clientProcess)
        {
            var (XlocOverview, YlocOverview) = InGameWnd.GetCoordWindow(clientProcess, "OverView");

            var Overview = UITreeReader.GetUITrees(clientProcess, "OverView");
            if (Overview == null)
                return null;
            Overview = Overview.FindEntityOfString("OverviewScrollEntry");
            if (Overview == null)
                return null;

            var OverviewEntry = Overview.handleEntity("OverviewScrollEntry");

            List<OverviewItem> OverviewInfo = new List<OverviewItem>();

            for (int i = 0; i < OverviewEntry.children.Length; i++)
            {
                if (OverviewEntry.children[i] == null)
                    continue;
                if (OverviewEntry.children[i].children == null)
                    continue;
                if (OverviewEntry.children[i].children.Last() == null)
                    continue;
                if (OverviewEntry.children[i].children.Last().children == null)
                    continue;
                if (OverviewEntry.children[i].children.Last().pythonObjectTypeName != "SpaceObjectIcon")
                    continue;

                OverviewItem OverviewItemInfo = new OverviewItem();

                int YLocGateRelOverview = 0;
                if (OverviewEntry.children[i].dictEntriesOfInterest["_displayY"] is Newtonsoft.Json.Linq.JObject)
                    YLocGateRelOverview = Convert.ToInt32(OverviewEntry.children[i].dictEntriesOfInterest["_displayY"]["int_low32"]);
                else
                    YLocGateRelOverview = Convert.ToInt32(OverviewEntry.children[i].dictEntriesOfInterest["_displayY"]);

                OverviewItemInfo.Pos.x = XlocOverview + 50;
                OverviewItemInfo.Pos.y = YlocOverview + YLocGateRelOverview + 77;

                bool NoColors = false;
                for (int j = 0; j < OverviewEntry.children[i].children.Last().children.Length; j++)
                {
                    if (OverviewEntry.children[i].children.Last().children[j].pythonObjectTypeName == "Sprite"
                        &&
                        OverviewEntry.children[i].children.Last().children[j].dictEntriesOfInterest["_name"].ToString() == "iconSprite")
                    {
                        var RGBColorIcon = OverviewEntry.children[i].children.Last().children[j].dictEntriesOfInterest["_color"];
                        if (RGBColorIcon == null)
                        {
                            NoColors = true;
                            break;
                        }

                        OverviewItemInfo.Colors.Red = Convert.ToInt32(RGBColorIcon["rPercent"]);
                        OverviewItemInfo.Colors.Green = Convert.ToInt32(RGBColorIcon["gPercent"]);
                        OverviewItemInfo.Colors.Blue = Convert.ToInt32(RGBColorIcon["bPercent"]);
                    }
                }
                if (NoColors)
                    continue;

                int FieldCount = OverviewEntry.children[i].children.Length;

                var DistanceStr = OverviewEntry.children[i].children[FieldCount - 2]
                    .dictEntriesOfInterest["_text"].ToString();

                //Console.WriteLine(DistanceStr);

                int Distance;
                int.TryParse(string.Join("", DistanceStr.Split()[0].Split(',')[0].Where(c => char.IsDigit(c))), out Distance);

                OverviewItemInfo.Distance.value = Distance;

                //Console.WriteLine(OverviewItemInfo.Distance.value);

                OverviewItemInfo.Distance.measure = DistanceStr.Split().Last();

                //Console.WriteLine(OverviewItemInfo.Distance.measure);

                OverviewItemInfo.Name = OverviewEntry.children[i].children[FieldCount - 3]
                    .dictEntriesOfInterest["_text"].ToString();

                OverviewItemInfo.Type = OverviewEntry.children[i].children[FieldCount - 4]
                    .dictEntriesOfInterest["_text"].ToString();

                for (int j = 0; j < OverviewEntry.children[i].children.Last().children.Length; j++)
                {
                    if (OverviewEntry.children[i].children.Last().children[j].pythonObjectTypeName == "Sprite"
                        &&
                        OverviewEntry.children[i].children.Last().children[j].dictEntriesOfInterest["_name"].ToString() == "targetedByMeIndicator")
                    {
                        OverviewItemInfo.TargetLocked = true;
                    }

                    if (OverviewEntry.children[i].children.Last().children[j].pythonObjectTypeName == "Sprite"
                        &&
                        OverviewEntry.children[i].children.Last().children[j].dictEntriesOfInterest["_name"].ToString() == "myActiveTargetIndicator")
                    {
                        OverviewItemInfo.AimOnTargetLocked = true;
                    }
                }

                if (FieldCount > 4)
                {
                    int MaxLength = 9;
                    var Speed = OverviewEntry.children[i].children[FieldCount - 5]
                    .dictEntriesOfInterest["_text"].ToString().Replace(" ", "");

                    if (Speed.Length > MaxLength)
                        Speed = Speed.Substring(0, MaxLength); // Speed.Length - 3

                    //Console.WriteLine(Speed);
                    //if (Speed.Split().Last() == OverviewItemInfo.Distance.measure)
                    //{
                    //    Console.WriteLine("xuy iz jopy");
                    //}

                    int value;
                    int.TryParse(string.Join("", Speed.Where(c => char.IsDigit(c))), out value);

                    OverviewItemInfo.Speed = value;
                }

                //Console.WriteLine("{0} on X : {1}, Y : {2}!", ObjectName, XlocOverview + 50, YlocOverview + YLocGateRelOverview + 77 + 23);
                OverviewInfo.Add(OverviewItemInfo);
            }
            return OverviewInfo;
        }
        static public string GetColorInfo(Colors ComparedColor)
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
