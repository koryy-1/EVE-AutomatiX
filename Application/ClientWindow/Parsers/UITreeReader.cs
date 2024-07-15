using Application.ClientWindow.UIHandlers;
using Domen.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ClientWindow.Parsers
{
    static public class UITreeReader
    {
        static public UITreeNode GetUITrees(ClientParams clientProcess, string WindowName = null, int initialDepth = 2, bool TakeHigher = false)
        {
            if (WindowName == null)
            {
                return ReadMemory.GetUITrees(clientProcess.RootAddress, clientProcess.ProcessId);
            }
            var UnfinishedUITree = ReadMemory.GetUITrees(clientProcess.RootAddress, clientProcess.ProcessId, initialDepth);

            var UnfinishedWindowTree = UnfinishedUITree.FindEntityOfString(WindowName);
            if (UnfinishedWindowTree == null)
            {
                return null;
            }
            UnfinishedWindowTree = UnfinishedWindowTree.handleEntity(WindowName);

            string WindowAddress = "";
            if (TakeHigher)
            {
                WindowAddress = UnfinishedWindowTree.pythonObjectAddress.ToString();
            }
            else
            {
                WindowAddress = UnfinishedWindowTree.children[Convert.ToInt32(UnfinishedWindowTree.dictEntriesOfInterest["needIndex"])]
                    .pythonObjectAddress.ToString();
            }


            var WindowUITree = ReadMemory.GetUITrees(WindowAddress, clientProcess.ProcessId);

            return WindowUITree;
        }
    }
}
