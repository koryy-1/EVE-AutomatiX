﻿using Domen.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domen.Models
{
    public class ShipModule
    {
        public ModuleNames Name { get; set; }
        public string Type { get; set; } // high / med / low
        public string Mode { get; set; } // glow / busy / idle / reloading
        public int SlotNum { get; set; }
        public int AmountСharges { get; set; } = 0;
        public int VirtualKey { get; set; } = 0;
    }
}
