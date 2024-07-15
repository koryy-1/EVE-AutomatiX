﻿using Domen.Constants;
using Domen.Models;
using EVE_AutomatiX.Starship.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities.Starship.Modules
{
    public class Amplifiers : ShipModule
    {
        public Amplifiers(List<Module> modules)
        {
            Modules = modules;
            Modules
                .ForEach(module => {
                    if (module.Name == ModuleName.MissileComputer)
                        Name = module.Name;
                });
        }
    }
}