using System;
using System.Collections.Generic;
using System.Text;

namespace Domen.Models
{
    public class HudInterface
    {
        public HudInterface()
        {
            Pos = new Point();
            HealthPoints = new HealthPoints();
        }
        public Point Pos { get; set; }
        public List<ShipModule> AllModules { get; set; }
        //public List<Module> HighModules { get; set; }
        //public List<Module> MedModules { get; set; }
        //public List<Module> LowModules { get; set; }
        public HealthPoints HealthPoints { get; set; }
        public int CurrentSpeed { get; set; }
        public ShipFlightMode ShipState { get; set; }
    }
}
