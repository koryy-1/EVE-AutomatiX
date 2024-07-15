using System;
using System.Collections.Generic;
using System.Text;

namespace Domen.Models
{
    public class TargetInBar
    {
        public TargetInBar()
        {
            Pos = new Point();
            Distance = new Distance();
        }
        public Point Pos { get; set; }
        public Distance Distance { get; set; }
        public string Name { get; set; }
        public bool AimOnTargetLocked { get; set; } = false;
        public bool WeaponWorking { get; set; } = false;
    }
}
