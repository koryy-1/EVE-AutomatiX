using EVE_AutomatiX.Models;
using EVE_Bot.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EVE_Bot.Parsers
{
    static public class DS
    {
        static public List<DScanItem> GetInfo(ClientParams clientProcess)
        {
            var DScanWnd = UITreeReader.GetUITrees(clientProcess, "DirectionalScanner");
            if (DScanWnd == null) // docked
                return null;

            DScanWnd = DScanWnd.FindEntityOfStringByDictEntriesOfInterest("_name", "noResultsLabel");
            if (DScanWnd != null) // no Results
                return null;

            DScanWnd = UITreeReader.GetUITrees(clientProcess).FindEntityOfString("DirectionalScanResultEntry");
            if (DScanWnd == null) // xyeta
            {
                Console.WriteLine("dscan not work");
                return null;
            }
            var DScanWndEntries = DScanWnd.handleEntity("DirectionalScanResultEntry");

            List<DScanItem> DScanInfo = new List<DScanItem>();

            for (int i = 0; i < DScanWndEntries.children.Length; i++)
            {
                if (DScanWndEntries.children[i] == null)
                    continue;
                if (DScanWndEntries.children[i].children == null)
                    continue;
                if (DScanWndEntries.children[i].children.Length < 2)
                    continue;
                if (DScanWndEntries.children[i].children[2] == null)
                    continue;
                if (DScanWndEntries.children[i].children[2].children == null)
                    continue;
                if (DScanWndEntries.children[i].children[2].children.Length == 0)
                    continue;
                if (DScanWndEntries.children[i].children[2].children[0] == null)
                    continue;

                DScanItem DScanItemInfo = new DScanItem();

                DScanItemInfo.Name = DScanWndEntries.children[i].children[1].children[0]
                        .dictEntriesOfInterest["_setText"].ToString();

                DScanItemInfo.Type = DScanWndEntries.children[i].children[2].children[0]
                        .dictEntriesOfInterest["_setText"].ToString();

                DScanInfo.Add(DScanItemInfo);
            }
            return DScanInfo;
        }
    }
}
