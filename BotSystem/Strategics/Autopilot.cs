using EVE_AutomatiX.ClientWindow;
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
        public Ship _ship { get; set; }
        public Client _client { get; set; }
        public void Start()
        {
            Docking.EnsureUndocked();
            while (Routing.IsRouteLaid())
            {
                Routing.GotoNextSystem();
            }
        }
    }
}
