using EVE_Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Starship.API
{
    public static class Finder
    {
        public static List<OverviewItem> GetEnemies()
        {
            throw new NotImplementedException();
        }

        static public int Km2m(Distance Distance)
        {
            if (Distance.measure == "km")
            {
                return Distance.value * 1000;
            }
            else
                return Distance.value;
        }

        static public bool CheckDistance(OverviewItem ItemInSpace, Distance MinDistance)
        {
            if (Km2m(ItemInSpace.Distance) < Km2m(MinDistance))
            {
                return true;
            }
            return false;
        }

        public static bool AreHostileObjectsInOverview()
        {
            throw new NotImplementedException();
        }
    }
}
