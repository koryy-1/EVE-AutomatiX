using EVE_AutomatiX.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace EVE_AutomatiX.Models
{
    public class Module
    {
        public ModuleName Name { get; set; } = ModuleName.None;
        public string Type { get; set; } // high / med / low
        public string Mode { get; set; } // glow / busy / idle / reloading
        public int SlotNum { get; set; }
        public int AmountСharges { get; set; } = 0;
        public int VirtualKey { get; set; } = 0;
    }
}
