using EVE_AutomatiX.ClientWindow;
using EVE_AutomatiX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Starship.API
{
    public class Routing
    {
        Client _client;

        public Routing(Client client)
        {
            _client = client;
        }

        public void EnsureRouteBuilt()
        {
            if (!IsRouteLaid()) GetDirections();
        }

        public void GotoNextSystem()
        {
            throw new NotImplementedException();
        }

        public bool IsRouteLaid()
        {
            throw new NotImplementedException();
        }

        public void GetDirections()
        {
            throw new NotImplementedException();
        }
    }
}
