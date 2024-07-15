using Domen.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domen.Models
{
    public class Drone
    {
        public Drone()
        {
            Pos = new Point();
            HealthPoints = new HealthPoints();
        }
        public Point Pos { get; set; }
        public string Location { get; set; } // space / bay
        public string WorkMode { get; set; } // Idle / Fighting / Returning
        public HealthPoints HealthPoints { get; set; }
    }
}
