using EVE_AutomatiX.ClientWindow;
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
        int _weaponsRange;
        string _charges;
        public Weapon(Client client, int weaponsRange, string charges)
        {
            _client = client;
            _weaponsRange = weaponsRange;
            _charges = charges;
            _client.Parser.HI.GetAllModulesInfo()
                .ForEach(module => {
                    if (module.Name == ModuleName.MissileLauncher)
                        Name = module.Name;
                });
        }
    }
}
