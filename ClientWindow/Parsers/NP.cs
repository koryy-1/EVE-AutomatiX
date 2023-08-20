using EVE_AutomatiX.Models;
using EVE_AutomatiX.UIHandlers;
using EVE_Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EVE_Bot.Parsers
{
    static public class NP
    {
        static public List<NotepadItem> GetInfo(ClientParams clientProcess)
        {
            var (XlocOverview, YlocOverview) = InGameWnd.GetCoordWindow(clientProcess, "NotepadWindow");

            var NotepadWindow = UITreeReader.GetUITrees(clientProcess, "NotepadWindow");
            if (NotepadWindow == null)
                return null;

            var SE_EditTextlineCore = NotepadWindow.FindEntityOfString("SE_EditTextlineCore");
            if (SE_EditTextlineCore == null)
                return null;

            var SE_EditTextlineCoreEntry = SE_EditTextlineCore.handleEntity("SE_EditTextlineCore");

            int LeftSidebarWidth = InGameWnd.GetWidthWindow(clientProcess, "ListGroup");

            List<NotepadItem> NotepadInfo = new List<NotepadItem>();

            for (int i = 0; i < SE_EditTextlineCoreEntry.children.Length; i++)
            {
                if (SE_EditTextlineCoreEntry.children[i] == null)
                    continue;
                if (SE_EditTextlineCoreEntry.children[i].children == null)
                    continue;
                if (SE_EditTextlineCoreEntry.children[i].children.Length < 2)
                    continue;
                if (SE_EditTextlineCoreEntry.children[i].children[1] == null)
                    continue;
                if (SE_EditTextlineCoreEntry.children[i].children[1].children == null)
                    continue;

                int YItem = 0;
                if (SE_EditTextlineCoreEntry.children[i].dictEntriesOfInterest["_displayY"] is Newtonsoft.Json.Linq.JObject)
                    YItem = Convert.ToInt32(SE_EditTextlineCoreEntry.children[i].dictEntriesOfInterest["_displayY"]["int_low32"]);
                else
                    YItem = Convert.ToInt32(SE_EditTextlineCoreEntry.children[i].dictEntriesOfInterest["_displayY"]);

                if (i == 0)
                {
                    YItem = YItem + 5;
                }

                for (int j = 0; j < SE_EditTextlineCoreEntry.children[i].children[1].children.Length; j++)
                {
                    NotepadItem NotepadItemInfo = new NotepadItem();

                    int XItem = 0;
                    if (SE_EditTextlineCoreEntry.children[i].children[1].children[j].dictEntriesOfInterest["_displayX"] is Newtonsoft.Json.Linq.JObject)
                        XItem = Convert.ToInt32(SE_EditTextlineCoreEntry.children[i].children[1].children[j]
                            .dictEntriesOfInterest["_displayX"]["int_low32"]);
                    else
                        XItem = Convert.ToInt32(SE_EditTextlineCoreEntry.children[i].children[1].children[j]
                            .dictEntriesOfInterest["_displayX"]);

                    NotepadItemInfo.Pos.x = XlocOverview + LeftSidebarWidth + 10 + XItem + 10;
                    NotepadItemInfo.Pos.y = YlocOverview + YItem + 107;

                    NotepadInfo.Add(NotepadItemInfo);
                }
            }

            return NotepadInfo;
        }
    }
}
