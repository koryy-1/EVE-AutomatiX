using EVE_AutomatiX.ClientWindow;
using EVE_AutomatiX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Starship.API
{
    public class Docking
    {
        Client _client;

        public Docking(Client client)
        {
            _client = client;
        }

        public void EnsureUndocked()
        {
            if (IsDocked()) Undock();
        }

        public bool IsDocked()
        {
            // parsing light ingame wnd for space and check present
            throw new NotImplementedException();
        }

        public void Undock()
        {
            throw new NotImplementedException();
        }
    }
}
