using Domen.Enums;
using Domen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Starship.Modules
{
    public class Weapon : StarShipModule
    {
        int _weaponsRange;
        string _charges;
        public Weapon(List<Domen.Models.ShipModule> modules, int weaponsRange, string charges)
        {
            Modules = modules;
            _weaponsRange = weaponsRange;
            _charges = charges;
            Modules
                .ForEach(module => {
                    if (module.Name == ModuleNames.MissileLauncher)
                        Name = module.Name;
                });
        }
    }
}
