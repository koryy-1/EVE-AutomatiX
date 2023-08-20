using System;
using System.Collections.Generic;
using System.Text;

namespace EVE_Bot.Models
{
    public class RoutePanel
    {
        public RoutePanel()
        {
            ButtonLoc = new Point();
        }
        public List<Colors> Systems { get; set; }
        public Point ButtonLoc { get; set; }
    }
}
