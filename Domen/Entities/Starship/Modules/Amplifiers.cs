using Domen.Enums;
using Domen.Models;
using EVE_AutomatiX.Starship.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities.Starship.Modules
{
    public class Amplifiers : EVE_AutomatiX.Starship.Modules.ShipModule
    {
        public Amplifiers(List<Models.ShipModule> modules)
        {
            Modules = modules;
            Modules
                .ForEach(module => {
                    if (module.Name == ModuleNames.MissileComputer)
                        Name = module.Name;
                });
        }
    }
}
