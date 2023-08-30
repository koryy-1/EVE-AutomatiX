using EVE_AutomatiX.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EVE_Bot.Models
{
    public class ShipFlightMode
    {
        public string CurrentItemAndDistance { get; set; } = string.Empty;
        public FlightMode CurrentFlightMode { get; set; } = FlightMode.None;
    }
}
