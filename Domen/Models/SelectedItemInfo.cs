using System;
using System.Collections.Generic;
using System.Text;

namespace Domen.Models
{
    public class SelectedItemInfo
    {
        public string Name { get; set; }
        public Distance Distance { get; set; }
        public List<SelectedItemButton> Buttons { get; set; }
    }
}
