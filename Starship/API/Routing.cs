using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Starship.API
{
    public static class Routing
    {
        public static void EnsureRouteBuilt()
        {
            if (!IsRouteLaid()) GetDirections();
        }

        public static void GotoNextSystem()
        {
            throw new NotImplementedException();
        }

        public static bool IsRouteLaid()
        {
            throw new NotImplementedException();
        }

        public static void GetDirections()
        {
            throw new NotImplementedException();
        }
    }
}
