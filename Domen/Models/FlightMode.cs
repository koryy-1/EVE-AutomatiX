using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Models
{
    public enum FlightMode
    {
        None,
        Approaching,
        Aligning,
        Orbiting,
        Warping,
        Jumping,
        ClickTarget,
        KeepingAtRange,
    }
}
