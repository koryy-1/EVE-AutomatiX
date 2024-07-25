using Domen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Starship.Modules
{
    public class ShipModule : Domen.Models.ShipModule
    {
        public List<Domen.Models.ShipModule> Modules;

        public string GetMode()
        {
            return Modules.Find(module => module.Name == Name).Mode;
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
