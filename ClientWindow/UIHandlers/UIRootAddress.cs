using EVE_AutomatiX.Models;
using EVE_AutomatiX.UIHandlers;
using EVE_AutomatiX.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX
{
    static class UIRootAddress
    {
        public static bool UpdateRootAddress(ref Config config, string nickName)
        {
            Console.WriteLine("pid in config = {0}", config.ClientParams.ProcessId);
            Console.WriteLine("RootAddress in config = {0}", config.ClientParams.RootAddress);

            config.ClientParams.hWnd = WinApi.FindWindow("trinityWindow", config.ClientParams.ProcessName);

            if (config.ClientParams.hWnd.ToInt32() == 0)
            {
                Console.WriteLine("failed to find HWND by window name");
                return false;
            }

            uint uintprocessId = 0;
            WinApi.GetWindowThreadProcessId(config.ClientParams.hWnd, out uintprocessId);

            Console.WriteLine("pid by hWnd {0}", uintprocessId);

            if (config.ClientParams.ProcessId != Convert.ToInt32(uintprocessId))
            {
                Console.WriteLine("please wait...");
                config.ClientParams.RootAddress = getUIRootAddressByProcessName(config.ClientParams.ProcessId);

                if (string.IsNullOrEmpty(config.ClientParams.RootAddress))
                {
                    Console.WriteLine("failed to find root address by pid or HWND");
                    return false;
                }

                config.ClientParams.ProcessId = Convert.ToInt32(uintprocessId);

                ConfigReader.UpdateConfig(nickName, config);

                Console.WriteLine("new PID in file = {0}", config.ClientParams.ProcessId);
                Console.WriteLine("new RootAddress in file = {0}", config.ClientParams.RootAddress);
            }
            return true;
        }

        private static string getUIRootAddressByProcessName(int processId)
        {

            var (uiRootCandidatesAddresses, memoryReader) = ReadMemory.GetRootAddressesAndMemoryReader(string.Empty, processId);

            IImmutableList<UITreeNode> ReadUITrees() =>
                    uiRootCandidatesAddresses
                    .Select(uiTreeRoot => EveOnline64.ReadUITreeFromAddress(uiTreeRoot, memoryReader, 99))
                    .Where(uiTree => uiTree != null)
                    .ToImmutableList();

            var readUiTreesStopwatch = System.Diagnostics.Stopwatch.StartNew();

            var uiTrees = ReadUITrees();

            readUiTreesStopwatch.Stop();

            var uiTreesWithStats =
                uiTrees
                .Select(uiTree =>
                new
                {
                    uiTree = uiTree,
                    nodeCount = uiTree.EnumerateSelfAndDescendants().Count()
                })
                .OrderByDescending(uiTreeWithStats => uiTreeWithStats.nodeCount)
                .ToImmutableList();

            var largestUiTree =
                uiTreesWithStats
                .OrderByDescending(uiTreeWithStats => uiTreeWithStats.nodeCount)
                .FirstOrDefault().uiTree;

            if (largestUiTree != null)
            {
                var ReadyRootAdress = largestUiTree.pythonObjectAddress.ToString();
                return ReadyRootAdress;
            }
            else
            {
                Console.WriteLine("No largest UI tree.");
                return null;
            }
        }
    }
}
