using EVE_AutomatiX.ClientWindow;
using EVE_AutomatiX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Starship.API
{
    public class Diagnostics
    {
        Client _client;

        public Diagnostics(Client client)
        {
            _client = client;
        }

        public bool Run()
        {
            return false;
        }
    }
}
