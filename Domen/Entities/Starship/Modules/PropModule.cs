using Domen.Enums;
using Domen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Starship.Modules
{
    public class PropModule : ShipModule
    {
        public PropModule(List<Domen.Models.ShipModule> modules)
        {
            Modules = modules;
            Modules
                .ForEach(module => {
                    if (module.Name == ModuleNames.AB || module.Name == ModuleNames.MWD)
                        Name = module.Name;
            });
        }
    }
}
