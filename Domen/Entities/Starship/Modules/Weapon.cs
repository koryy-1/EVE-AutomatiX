using Domen.Constants;
using Domen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Starship.Modules
{
    public class Weapon : ShipModule
    {
        int _weaponsRange;
        string _charges;
        public Weapon(List<Module> modules, int weaponsRange, string charges)
        {
            Modules = modules;
            _weaponsRange = weaponsRange;
            _charges = charges;
            Modules
                .ForEach(module => {
                    if (module.Name == ModuleName.MissileLauncher)
                        Name = module.Name;
                });
        }
    }
}
