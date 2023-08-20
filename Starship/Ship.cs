using EVE_AutomatiX.Models;
using EVE_AutomatiX.Starship.API;
using EVE_AutomatiX.Starship.Modules;
using EVE_Bot.Models;
using EVE_Bot.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX
{
    public class Ship
    {
        public State state;
        public Location location;
        public Mode mode;

        public PropModule propModule;
        public Weapon weapon;
        public Defense defense;
        public Amplifiers amplifiers;


        public Ship(Config config, BotBehavior behavior)
        {
            clientProcess = config.ClientParams;

            state = State.NotAvailable;

            if (StationDocking.IsDocked())
                location = Location.Dock;
            if (HI.GetShipState(clientProcess).CurrentState.Contains("Warp"))
                location = Location.Warp;
            else
                location = Location.Space;

            mode = Mode.Lead;


            propModule = new PropModule(clientProcess);
            weapon = new Weapon(clientProcess);
            // todo: defense may be list
            defense = new Defense(clientProcess);
            // todo: too
            amplifiers = new Amplifiers(clientProcess);

            // StartThread Controllers and Monitors
        }
        void SetSpeed()
        {
            throw new NotImplementedException();
        }

        HealthPoints GetHealthPoints()
        {
            return HI.GetShipHP(clientProcess);
        }

        // anomalies
        List<ProbeScanItem> GetAnomalyList()
        {
            return PS.GetInfo(clientProcess);
        }

        void WarpToAnomaly()
        {
            throw new NotImplementedException();
        }

        // targets on overview
        List<OverviewItem> GetOverviewList()
        {
            return OV.GetInfo(clientProcess);
        }

        void DESTRXY_EVERYXNE()
        {
            while (Finder.AreHostileObjectsInOverview())
            {
                // lock next target
                // destroy target
            }
        }

        void LockEnemies()
        {
            throw new NotImplementedException();
        }

        void LockTargets(List<OverviewItem> overviewItems)
        {
            throw new NotImplementedException();
        }

        void OpenFire()
        {
            throw new NotImplementedException();
        }

        // movement
        void OrbitObject(OverviewItem Object)
        {
            throw new NotImplementedException();
        }

        void ApproachObject(OverviewItem Object)
        {
            throw new NotImplementedException();
        }

        // looting
        void GotoLoot()
        {
            throw new NotImplementedException();
        }

        void GotoLoot(string contName)
        {
            throw new NotImplementedException();
        }

        //cargo
        void EnsureCargoUnloaded()
        {
            throw new NotImplementedException();
        }

        int GetCargoPrice()
        {
            throw new NotImplementedException();
        }
    }

    public enum Mode
    {
        Master,
        Emergency
    }

    public enum State
    {
        NotAvailable,
        Fighting,
        Looting,
        Traveling
    }

    public enum Location
    {
        Dock,
        Space,
        Warp
    }
}
