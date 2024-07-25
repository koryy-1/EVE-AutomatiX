using Domen.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domen.Constants
{
    public static class ModulesData
    {
        public static Dictionary<string, ModuleNames> ModuleNamesDict = new Dictionary<string, ModuleNames>()
        {
            { "ModuleButton_434", ModuleNames.MWD}, // 5MN I
            { "ModuleButton_35658", ModuleNames.MWD}, // 5MN
            { "ModuleButton_35660", ModuleNames.MWD}, // 50MN
            { "ModuleButton_6005", ModuleNames.AB}, // 10MN AB
            { "ModuleButton_2404", ModuleNames.MissileLauncher}, // Light Missile Launcher T2
            { "ModuleButton_1877", ModuleNames.MissileLauncher}, // Rapid Light Missile Launcher T2

            { "ModuleButton_8089", ModuleNames.MissileLauncher}, // Arbalest Light Missile Launcher
            { "ModuleButton_8027", ModuleNames.MissileLauncher}, // Arbalest Rapid Light Missile Launcher
            { "ModuleButton_8007", ModuleNames.MissileLauncher}, // Experimental Rapid Light Missile Launcher
            { "ModuleButton_8105", ModuleNames.MissileLauncher}, // Arbalest Heavy Missile Launcher

            { "ModuleButton_54295", ModuleNames.ThermalHardener},
            { "ModuleButton_54294", ModuleNames.KineticHardener},
            { "ModuleButton_54291", ModuleNames.MultispectrumHardener},
            { "ModuleButton_35789", ModuleNames.MissileComputer},
        };
    }
}
