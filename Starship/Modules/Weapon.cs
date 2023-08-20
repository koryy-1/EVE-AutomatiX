using EVE_AutomatiX.Models;
using EVE_AutomatiX.Utils;
using EVE_Bot.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Starship.Modules
{
    public class Weapon : ShipModule
    {
        public Weapon(ClientParams clientProcess)
        {
            _clientProcess = clientProcess;
            HI.GetAllModulesInfo(clientProcess)
                .ForEach(module => {
                    if (module.Name == ModuleName.MissileLauncher)
                        Name = module.Name;
                });
        }
    }
}
