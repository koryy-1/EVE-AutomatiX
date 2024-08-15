using Domen.Enums;
using Domen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Starship.Modules
{
    public class Defense : StarShipModule
    {
        public Defense(List<Domen.Models.ShipModule> modules)
        {
            Modules = modules;
            Modules
                .ForEach(module => {
                    if (module.Name == ModuleNames.ThermalHardener ||
                    module.Name == ModuleNames.KineticHardener ||
                    module.Name == ModuleNames.MultispectrumHardener)
                        Name = module.Name;
                });
        }
    }
}
