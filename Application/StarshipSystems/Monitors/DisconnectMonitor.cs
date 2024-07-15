using EVE_AutomatiX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.StarshipSystems.Monitors
{
    public class DisconnectMonitor
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

        public bool ConditionToStartWorker()
        {
            throw new NotImplementedException();
        }

        public void Work()
        {
            throw new NotImplementedException();
        }
    }
}
