using System;
using System.Collections.Generic;
using System.Text;

namespace Domen.Constants
{
    public static class ModulesData
    {
        public static Dictionary<string, ModuleName> ModuleNamesDict = new Dictionary<string, ModuleName>()
        {
            { "ModuleButton_434", ModuleName.MWD}, // 5MN I
            { "ModuleButton_35658", ModuleName.MWD}, // 5MN
            { "ModuleButton_35660", ModuleName.MWD}, // 50MN
            { "ModuleButton_6005", ModuleName.AB}, // 10MN AB
            { "ModuleButton_2404", ModuleName.MissileLauncher}, // Light Missile Launcher T2
            { "ModuleButton_1877", ModuleName.MissileLauncher}, // Rapid Light Missile Launcher T2

            { "ModuleButton_8089", ModuleName.MissileLauncher}, // Arbalest Light Missile Launcher
            { "ModuleButton_8027", ModuleName.MissileLauncher}, // Arbalest Rapid Light Missile Launcher
            { "ModuleButton_8007", ModuleName.MissileLauncher}, // Experimental Rapid Light Missile Launcher
            { "ModuleButton_8105", ModuleName.MissileLauncher}, // Arbalest Heavy Missile Launcher

            { "ModuleButton_54295", ModuleName.ThermalHardener},
            { "ModuleButton_54294", ModuleName.KineticHardener},
            { "ModuleButton_54291", ModuleName.MultispectrumHardener},
            { "ModuleButton_35789", ModuleName.MissileComputer},
        };
    }
    public enum ModuleName
    {
        None,
        MissileLauncher,
        //RapidMissileLauncher,
        //HeavyMissileLauncher,
        MWD,
        AB,
        ThermalHardener,
        KineticHardener,
        MultispectrumHardener,
        MissileComputer
    }
}
