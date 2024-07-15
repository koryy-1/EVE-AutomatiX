using Domen.Constants;
using Domen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Starship.Modules
{
    public class Defense : ShipModule
    {
        public Defense(List<Module> modules)
        {
            Modules = modules;
            Modules
                .ForEach(module => {
                    if (module.Name == ModuleName.ThermalHardener ||
                    module.Name == ModuleName.KineticHardener ||
                    module.Name == ModuleName.MultispectrumHardener)
                        Name = module.Name;
                });
        }
    }
}
