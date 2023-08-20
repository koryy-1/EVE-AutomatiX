using System;
using System.Collections.Generic;
using System.Text;

namespace EVE_Bot.Models
{
    public class ActiveItemInfo
    {
        public string Name { get; set; }
        public Distance Distance { get; set; }
        public AtciveItemButton[] Buttons { get; set; }
    }
}
