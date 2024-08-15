using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Domen.Models
{
    public class RoutePanel
    {
        public string NextSystemInRoute { get; set; }
        public string CurrentDestination { get; set; }
        public List<Color> Systems { get; set; }
        public Point ButtonLoc { get; set; }

        public RoutePanel()
        {
            ButtonLoc = new Point();
        }
    }
}
