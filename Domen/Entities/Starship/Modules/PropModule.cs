using Domen.Constants;
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
        public PropModule(List<Module> modules)
        {
            Modules = modules;
            Modules
                .ForEach(module => {
                    if (module.Name == ModuleName.AB || module.Name == ModuleName.MWD)
                        Name = module.Name;
            });
        }
    }
}
