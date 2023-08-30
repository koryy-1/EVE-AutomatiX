using EVE_AutomatiX.ClientWindow;
using EVE_AutomatiX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Starship.API
{
    public class Looting
    {
        Client _client;

        public Looting(Client client)
        {
            _client = client;
        }

        public void GotoLoot()
        {
            throw new NotImplementedException();
        }

        public void GotoLoot(string contName)
        {
            throw new NotImplementedException();
        }

        //cargo
        void EnsureCargoUnloaded()
        {
            throw new NotImplementedException();
        }

        int GetCargoPrice()
        {
            throw new NotImplementedException();
        }
    }
}
