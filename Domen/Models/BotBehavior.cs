using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Models
{
    public class BotBehavior
    {
        volatile public bool BotEnable = false;
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
        volatile public bool BattleModeEnabled = false;
        volatile public bool AllowToOpenFire = false;
        volatile public bool EnemiesInGrid = false;
        volatile public bool TargetControllerEnabled = false;
        volatile public bool CloseDistanceToEnemy = false;
    }

    public class ShipHPFlags
    {
        volatile public bool AllowShieldHPControl = false;
        volatile public bool IsLowShield = false;
    }

    public class ShipControlFlags
    {
        volatile public bool AllowDocking = true;
    }
}
