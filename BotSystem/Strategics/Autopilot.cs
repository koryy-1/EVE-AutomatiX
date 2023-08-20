using EVE_AutomatiX.Starship.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX
{
    class Autopilot
    {
        public void Start()
        {
            StationDocking.EnsureUndocked();
            while (Routing.IsRouteLaid())
            {
                Routing.GotoNextSystem();
            }
        }
    }
}
