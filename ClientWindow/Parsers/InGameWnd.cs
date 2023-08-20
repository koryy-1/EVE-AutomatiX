using EVE_AutomatiX.Models;
using EVE_AutomatiX.UIHandlers;
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

namespace EVE_AutomatiX.UIHandlers
{
    static public class InGameWnd
    {
        // todo: полностью перелопатить тут все
        static public (int, int) GetCoordWindow(ClientParams ClientProcess, string WindowName)
        {
            var uiTreeWithPathToWindow = ReadMemory.GetUITrees(ClientProcess.RootAddress, ClientProcess.ProcessId).FindEntityOfString(WindowName);
            if (uiTreeWithPathToWindow == null)
            {
                Console.WriteLine("failed to find {0}", WindowName);
                return (0, 0);
            }
            var WindowEntry = uiTreeWithPathToWindow.handleEntity(WindowName);
            int XlocWindow = 0;
            int YlocWindow = 0;

            if (WindowEntry.children[Convert.ToInt32(WindowEntry.dictEntriesOfInterest["needIndex"])]
                .dictEntriesOfInterest["_displayX"] is Newtonsoft.Json.Linq.JObject)
            {
                XlocWindow = Convert.ToInt32(WindowEntry.children[Convert.ToInt32(WindowEntry.dictEntriesOfInterest["needIndex"])]
                .dictEntriesOfInterest["_displayX"]["int_low32"]);
            }
            else
                XlocWindow = Convert.ToInt32(WindowEntry.children[Convert.ToInt32(WindowEntry.dictEntriesOfInterest["needIndex"])]
                .dictEntriesOfInterest["_displayX"]);

            if (WindowEntry.children[Convert.ToInt32(WindowEntry.dictEntriesOfInterest["needIndex"])]
                .dictEntriesOfInterest["_displayY"] is Newtonsoft.Json.Linq.JObject)
            {
                YlocWindow = Convert.ToInt32(WindowEntry.children[Convert.ToInt32(WindowEntry.dictEntriesOfInterest["needIndex"])]
                .dictEntriesOfInterest["_displayY"]["int_low32"]);
            }
            else
                YlocWindow = Convert.ToInt32(WindowEntry.children[Convert.ToInt32(WindowEntry.dictEntriesOfInterest["needIndex"])]
                .dictEntriesOfInterest["_displayY"]);

            //Console.WriteLine("X location {0}: {1}", WindowName, XlocWindow);
            //Console.WriteLine("Y location {0}: {1}", WindowName, YlocWindow);
            return (XlocWindow, YlocWindow);
        }

        static public int GetHeightWindow(ClientParams ClientProcess, string WindowName)
        {
            var uiTreeWithPathToWindow = ReadMemory.GetUITrees(ClientProcess.RootAddress, ClientProcess.ProcessId).FindEntityOfString(WindowName);
            var WindowEntry = uiTreeWithPathToWindow.handleEntity(WindowName);

            var WindowHeight = 0;

            if (WindowEntry.children[Convert.ToInt32(WindowEntry.dictEntriesOfInterest["needIndex"])]
                .dictEntriesOfInterest["_height"] is Newtonsoft.Json.Linq.JObject)
            {
                WindowHeight = Convert.ToInt32(WindowEntry.children[Convert.ToInt32(WindowEntry.dictEntriesOfInterest["needIndex"])]
                .dictEntriesOfInterest["_height"]["int_low32"]);
            }
            else
                WindowHeight = Convert.ToInt32(WindowEntry.children[Convert.ToInt32(WindowEntry.dictEntriesOfInterest["needIndex"])]
                .dictEntriesOfInterest["_height"]);

            //Console.WriteLine("Height of {0}: {1}", WindowName, WindowHeight);
            return WindowHeight;
        }

        static public int GetWidthWindow(ClientParams ClientProcess, string WindowName)
        {
            var TreeViewEntryInventory = ReadMemory.GetUITrees(ClientProcess.RootAddress, ClientProcess.ProcessId).FindEntityOfString(WindowName).handleEntity(WindowName);

            var WindowWidth = 0;

            if (TreeViewEntryInventory.children[Convert.ToInt32(TreeViewEntryInventory.dictEntriesOfInterest["needIndex"])]
                .dictEntriesOfInterest["_displayWidth"] is Newtonsoft.Json.Linq.JObject)
            {
                WindowWidth = Convert.ToInt32(TreeViewEntryInventory.children[Convert.ToInt32(TreeViewEntryInventory.dictEntriesOfInterest["needIndex"])]
                .dictEntriesOfInterest["_displayWidth"]["int_low32"]);
            }
            else
                WindowWidth = Convert.ToInt32(TreeViewEntryInventory.children[Convert.ToInt32(TreeViewEntryInventory.dictEntriesOfInterest["needIndex"])]
                .dictEntriesOfInterest["_displayWidth"]);

            //Console.WriteLine("Width of {0}: {1}", WindowName, WindowWidth);
            return WindowWidth;
        }

        static public(int, int) GetCoordsEntityOnScreen(UITreeNode Entity)
        {
            int XEntity = 0;
            int YEntity = 0;

            if (Entity.dictEntriesOfInterest["_displayX"] is Newtonsoft.Json.Linq.JObject)
                XEntity = Convert.ToInt32(Entity.dictEntriesOfInterest["_displayX"]["int_low32"]);
            else
                XEntity = Convert.ToInt32(Entity.dictEntriesOfInterest["_displayX"]);


            if (Entity.dictEntriesOfInterest["_displayY"] is Newtonsoft.Json.Linq.JObject)
                YEntity = Convert.ToInt32(Entity.dictEntriesOfInterest["_displayY"]["int_low32"]);
            else
                YEntity = Convert.ToInt32(Entity.dictEntriesOfInterest["_displayY"]);

            return (XEntity, YEntity);
        }
    }
}
