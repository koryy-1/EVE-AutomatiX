using System;
using System.Collections.Generic;
using System.Text;

namespace Domen.Models
{
    public class ShipFlightMode
    {
        public string ItemAndDistance { get; set; }
        public string ObjectName { get; set; }
        public Distance Distance { get; set; }
        public FlightMode FlightMode { get; set; }
    }
}
