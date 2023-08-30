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
    public class PropModule : ShipModule
    {
        public PropModule(Client client)
        {
            _client = client;
            _client.Parser.HI.GetAllModulesInfo()
                .ForEach(module => {
                    if (module.Name == ModuleName.AB || module.Name == ModuleName.MWD)
                        Name = module.Name;
            });
        }
    }
}
