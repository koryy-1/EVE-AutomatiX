using EVE_Bot.Searchers;
using System;
using System.Collections.Generic;
using System.Text;

namespace EVE_Bot.Scripts
{
    static public class Autopilot
    {
        static public void Start()
        {
            General.EnsureUndocked();
            for (int i = 0; i < 100; i++)
            {
                if (i % 10 == 0)
                    MainScripts.CheckForConnectionLost();
                if (!MainScripts.GotoNextSystem(NeedToLayRoute: false))
                {
                    Console.WriteLine("route completed");
                    Environment.Exit(5);
                }
            }
        }
    }
}
