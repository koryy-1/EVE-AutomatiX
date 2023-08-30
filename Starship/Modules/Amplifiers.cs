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
    public class Amplifiers : ShipModule
    {
        public Amplifiers(Client client)
        {
            _client = client;
            _client.Parser.HI.GetAllModulesInfo()
                .ForEach(module => {
                    if (module.Name == ModuleName.MissileComputer)
                        Name = module.Name;
                });
        }
    }
}
