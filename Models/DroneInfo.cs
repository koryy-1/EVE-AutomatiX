using System;
using System.Collections.Generic;
using System.Text;

namespace EVE_Bot.Models
{
    public class DroneInfo
    {
        public DroneInfo()
        {
            Pos = new Point();
            HealthPoints = new HealthPoints();
        }
        public Point Pos { get; set; }
        public string WorkMode { get; set; }
        public HealthPoints HealthPoints { get; set; }
    }
}
