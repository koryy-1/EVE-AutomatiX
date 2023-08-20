using EVE_Bot.Controllers;
using read_memory_64_bit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EVE_Bot.Models;
using EVE_Bot.Parsers;

namespace EVE_Bot.Searchers
{
    static public class Finders1
    {
        

        static public (int, int) CheckCurrentSystemForCombatSites()
        {
            //System.NullReferenceException: Object reference not set to an instance of an object.
            UITreeNode CheckNoResults;
            try
            {
                CheckNoResults = GetUITrees().FindEntityOfString("ProbeScannerWindow")
                .FindEntityOfString("BasicDynamicScroll");
            }
            catch (Exception)
            {
                return (1, 1);
            }
            if (CheckNoResults == null)
            {
                return (1, 1);
            }
            var CheckNoResultsEntry = CheckNoResults.handleEntity("BasicDynamicScroll");

            var InterestCombatSite1 = "Refuge";
            var InterestCombatSite2 = "Hideaway";

            if (CheckNoResultsEntry.children[0].children[0].pythonObjectTypeName != "ScrollColumnHeader")
            {
                if (CheckNoResultsEntry.children[0].children[0].children[0].children[1].dictEntriesOfInterest["_name"].ToString() == "noResultsContainer")
                {
                    Console.WriteLine("no {0} or {1} in this system", InterestCombatSite1, InterestCombatSite2);
                    return (0, 0);
                }

            }


            (int XlocProbeScannerWindow, int YlocProbeScannerWindow) = Finders1.FindLocWnd("ProbeScannerWindow");

            var ProbeScanner = GetUITrees().FindEntityOfString("ScanResultNew");
            if (ProbeScanner == null)
                return (1, 1);
            var ProbeScannerEntries = ProbeScanner.handleEntity("ScanResultNew");

            for (int k = 0; k < ProbeScannerEntries.children.Length; k++)
            {
                int YLocScanResults = 0;
                int XLocBtnWarpToAnomaly = 0;
                string CurentAnimaly = "";
                if (ProbeScannerEntries.children[k].dictEntriesOfInterest["_displayY"] is Newtonsoft.Json.Linq.JObject)
                {
                    YLocScanResults = Convert.ToInt32(ProbeScannerEntries.children[k].dictEntriesOfInterest["_displayY"]["int_low32"]);
                    Console.WriteLine("{0} signature has loc Y : {1}", k + 1, YLocScanResults); // расоложение строки по Y
                }
                else
                {
                    YLocScanResults = Convert.ToInt32(ProbeScannerEntries.children[k].dictEntriesOfInterest["_displayY"]);
                    Console.WriteLine("{0} signature has loc Y : {1}", k + 1, YLocScanResults);
                }
                if (ProbeScannerEntries.children[k].dictEntriesOfInterest["_width"] is Newtonsoft.Json.Linq.JObject)
                {
                    XLocBtnWarpToAnomaly = Convert.ToInt32(ProbeScannerEntries.children[k].dictEntriesOfInterest["_width"]["int_low32"]);
                }
                else
                {
                    XLocBtnWarpToAnomaly = Convert.ToInt32(ProbeScannerEntries.children[k].dictEntriesOfInterest["_width"]);
                }
                //Hideaway
                // NullReferenceException
                //Unhandled exception. System.IndexOutOfRangeException: Index was outside the bounds of the array.
                CurentAnimaly = ProbeScannerEntries.children[k].children[0].children[1].children[3].children[0].dictEntriesOfInterest["_setText"].ToString();
                if (CurentAnimaly.Contains("Guristas Refuge")
                    ||
                    CurentAnimaly.Contains("Serpentis Refuge"))
                {
                    //Console.WriteLine("WarpTo {0} on X : {1}, Y : {2}!", InterestCombatSite1, XlocProbeScannerWindow + XLocBtnWarpToAnomaly - 10, YlocProbeScannerWindow + YLocScanResults + 104 + 23);
                    return (XlocProbeScannerWindow + XLocBtnWarpToAnomaly - 10, YlocProbeScannerWindow + YLocScanResults + 104);
                }
                //Refuge
                if (CurentAnimaly.Contains("Guristas Hideaway")
                    ||
                    CurentAnimaly.Contains("Serpentis Hideaway"))
                {
                    //Console.WriteLine("WarpTo {0} on X : {1}, Y : {2}!", InterestCombatSite2, XlocProbeScannerWindow + XLocBtnWarpToAnomaly - 10, YlocProbeScannerWindow + YLocScanResults + 104 + 23);
                    return (XlocProbeScannerWindow + XLocBtnWarpToAnomaly - 10, YlocProbeScannerWindow + YLocScanResults + 104);
                }
                //Den
                //if (CurentAnimaly.Contains("Guristas Den")
                //    ||
                //    CurentAnimaly.Contains("Serpentis Den"))
                //{
                //    //Console.WriteLine("WarpTo {0} on X : {1}, Y : {2}!", InterestCombatSite2, XlocProbeScannerWindow + XLocBtnWarpToAnomaly - 10, YlocProbeScannerWindow + YLocScanResults + 104 + 23);
                //    return (XlocProbeScannerWindow + XLocBtnWarpToAnomaly - 10, YlocProbeScannerWindow + YLocScanResults + 104);
                //}

            }
            Console.WriteLine("no {0} or {1} in this system", InterestCombatSite1, InterestCombatSite2);
            return (0, 0);
        }

        static public (int, int) FindObjectByWordInOverview(string ObjectName)
        {
            List<OverviewItem> OverviewInfo = OV.GetInfo();
            if (OverviewInfo == null)
                return (0, 0);

            for (int i = 0; i < OverviewInfo.Count; i++)
            {
                // check cont color
                if (GetColorInfo(OverviewInfo[i].Colors) is "yellow"
                    ||
                    GetColorInfo(OverviewInfo[i].Colors) is "gray")
                {
                    continue;
                }

                if (OverviewInfo[i].Name.Contains(ObjectName) || OverviewInfo[i].Type.Contains(ObjectName))
                {
                    return (OverviewInfo[i].Pos.x, OverviewInfo[i].Pos.y);
                }
            }
            return (0, 0);
        }

        static public (int, int) FindObjectByWordsInOverview(List<string> EnemyTypes)
        {
            List<OverviewItem> OverviewInfo = OV.GetInfo();
            if (OverviewInfo == null)
                return (0, 0);

            for (int i = 0; i < OverviewInfo.Count; i++)
            {
                foreach (var item in EnemyTypes)
                {
                    if (OverviewInfo[i].Type.Contains(item))
                        return (OverviewInfo[i].Pos.x, OverviewInfo[i].Pos.y);
                }
            }
            return (0, 0);
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

        static public OverviewItem FindAccelerationGate()
        {
            List<OverviewItem> OverviewInfo = OV.GetInfo();
            if (OverviewInfo == null)
                return null;

            foreach (var item in OverviewInfo)
            {
                if (item.Name.Contains("Acceleration Gate") || item.Type.Contains("Acceleration Gate"))
                    return item;
            }
            return null;
        }

        static public OverviewItem FindExpBlock()
        {
            List<OverviewItem> OverviewInfo = OV.GetInfo();
            if (OverviewInfo == null)
                return null;

            foreach (var item in OverviewInfo)
            {
                if (item.Name.Contains("Radiating Telescope") || item.Type.Contains("Radiating Telescope")
                    || item.Name.Contains("Serpentis Supply Stronghold") || item.Type.Contains("Serpentis Supply Stronghold"))
                    return item;
            }
            return null;
        }

        static public string GetDirectionIconFilter()
        {
            var Overview = OV.GetInfo();

            var Sun = Overview.Find(item => item.Type.StartsWith("Sun"));
            if (Sun == null)
                return "no info";
            var Stargate = Overview.Find(item => GetColorInfo(item.Colors) is "white" && item.Type.Contains("Stargate"));

            if (Sun.Pos.y < Stargate.Pos.y)
                return "up";
            else
                return "down";
        }

        static public void SetIconFilter(string NeedDirection)
        {
            var CurrentDirection = GetDirectionIconFilter();

            if (CurrentDirection == "no info")
            {
                Console.WriteLine("no info about icon filter");
                var IconFilter = FindIconFilter();
                Emulators.ClickLB(IconFilter.Item1, IconFilter.Item2);
                return;
            }
            if (CurrentDirection != NeedDirection)
            {
                var IconFilter = FindIconFilter();
                Emulators.ClickLB(IconFilter.Item1, IconFilter.Item2);
                return;
            }
        }

        static public (int, int) FindIconFilter()
        {
            var (XlocOverview, YlocOverview) = Finders1.FindLocWnd("OverView");

            //Console.WriteLine("Icon filter on X : {0}, Y : {1}", XlocOverview + 16, YlocOverview + 57 + 23);
            return (XlocOverview + 16, YlocOverview + 57);
        }


        static public (int, int) FindLocWnd(string WindowName)
        {
            return Window.GetCoordWindow(WindowName);
        }

        static public UITreeNode GetUITrees()
        {
            return ReadMemory.GetUITrees(Window.RootAddress, Window.processId);
        }
    }
}
