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
        public ClientParams _clientProcess;

        public string GetMode()
        {
            return HI.GetAllModulesInfo(_clientProcess).Find(module => module.Name == Name).Mode;
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
