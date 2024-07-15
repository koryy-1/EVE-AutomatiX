using System;
using System.Collections.Generic;
using System.Text;

namespace Domen.Models
{
    public class RoutePanel
    {
        public RoutePanel()
        {
            ButtonLoc = new Point();
        }
        public List<Color> Systems { get; set; }
        public Point ButtonLoc { get; set; }
    }
}
