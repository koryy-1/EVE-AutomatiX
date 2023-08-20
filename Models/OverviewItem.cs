using System;
using System.Collections.Generic;
using System.Text;

namespace EVE_Bot.Models
{
    public class OverviewItem
    {
        public OverviewItem()
        {
            Pos = new Point();
            Colors = new Colors();
            Distance = new Distance();
        }
        public Point Pos { get; set; }

        //aggressive / hostile / neutral
        public string IconType { get; set; }
        public bool TargetLocked { get; set; } = false;
        public bool AimOnTargetLocked { get; set; } = false;
        public Colors Colors { get; set; }
        public Distance Distance { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Speed { get; set; }
    }
}
