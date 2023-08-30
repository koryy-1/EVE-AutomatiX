using EVE_AutomatiX.ClientWindow;
using EVE_AutomatiX.Models;
using EVE_Bot.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Starship.Modules
{
    public class ShipModule : Module
    {
        public Client _client;

        public string GetMode()
        {
            return _client.Parser.HI.GetAllModulesInfo().Find(module => module.Name == Name).Mode;
        }

        public void Enable()
        {
            throw new NotImplementedException();
        }

        public void Disable()
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled()
        {
            throw new NotImplementedException();
        }
    }
}
