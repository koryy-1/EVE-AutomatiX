using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Starship.API
{
    public static class StationDocking
    {
        static public void EnsureUndocked()
        {
            if (IsDocked()) Undock();
        }

        public static bool IsDocked()
        {
            // parsing light ingame wnd for space and check present
            throw new NotImplementedException();
        }

        public static void Undock()
        {
            throw new NotImplementedException();
        }
    }
}
