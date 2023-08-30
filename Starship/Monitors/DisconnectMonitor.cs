using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Monitors
{
    public class DisconnectMonitor : ThreadWrapper
    {
        public void CheckConnLost()
        {
            throw new NotImplementedException();
            while (true)
            {
                //MainScripts.CheckForConnectionLost();

                Thread.Sleep(60 * 1000);
            }
        }

        public override bool ConditionToStartWorker()
        {
            throw new NotImplementedException();
        }

        public override void Work()
        {
            throw new NotImplementedException();
        }
    }
}
