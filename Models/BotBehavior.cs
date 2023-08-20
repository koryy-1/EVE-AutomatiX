using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Models
{
    public class BotBehavior
    {
        public volatile bool DangerAnalyzerEnable = true;
        public volatile bool AllowCheckConLost = true;
        public volatile bool AllowDScan = false;

        // forbidden

        // stopFlags


        public DronesFlags DronesFlags = new DronesFlags();
        public CombatFlags CombatFlags = new CombatFlags();
        public ShipHPFlags ShipHPFlags = new ShipHPFlags();
        public ShipControlFlags ShipControlFlags = new ShipControlFlags();
    }

    public class DronesFlags
    {
        volatile public bool AllowDroneControl = false;
        volatile public bool AllowDroneRescoop = false;
        volatile public bool SpecialFocusOnDrones = false;
    }

    public class CombatFlags
    {
        volatile public bool BotEnable = false;
        volatile static public bool AllowToAttack = false;
        volatile static public bool EnemiesInGrid = false;
        volatile static public bool AllowCombat = false;
    }

    public class ShipHPFlags
    {
        volatile static public bool AllowShieldHPControl = false;
        volatile static public bool ShipShieldIsLow = false;
    }

    public class ShipControlFlags
    {
        volatile static public bool AllowShipControl = true;
        volatile static public bool AllowDocking = true;
        volatile static public bool AllowNavigationControl = false;
        volatile static public bool CloseDistanceToEnemy = false;
        volatile static public string ItemInSpace = "";
        volatile static public string FlightManeuver = "";
        volatile static public string ExpectedState = "";
    }
}
