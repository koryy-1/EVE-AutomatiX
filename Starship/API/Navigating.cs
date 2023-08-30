using EVE_AutomatiX.ClientWindow;
using EVE_AutomatiX.Models;
using EVE_Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Starship.API
{
    public class Navigating
    {
        Client _client;

        volatile public OverviewItem _spaceObject;
        volatile public string FlightManeuver = "";
        volatile public FlightMode _flightMode;

        public Navigating(Client client)
        {
            _client = client;
        }

        internal void SetFlightModeOnObject()
        {
            // Click(_spaceObject.Pos);
            // click in active item need flightMode
            throw new NotImplementedException();
        }

        public void SetFlightModeOnObject(FlightMode flightMode, OverviewItem spaceObject)
        {
            // Click(spaceObject.Pos);
            // click in active item need flightMode
            throw new NotImplementedException();
        }

        public bool CheckFlightMode()
        {
            var flightModeOnObject = _client.Parser.HI.GetShipFlightMode();

            return flightModeOnObject.CurrentFlightMode == _flightMode &&
                flightModeOnObject.CurrentItemAndDistance.Contains(_spaceObject.Name);
        }

        public bool CheckFlightMode(FlightMode flightMode)
        {
            return _client.Parser.HI.GetShipFlightMode().CurrentFlightMode == flightMode;
        }

        public bool CheckFlightMode(FlightMode flightMode, OverviewItem spaceObject)
        {
            var flightModeOnObject = _client.Parser.HI.GetShipFlightMode();

            return flightModeOnObject.CurrentFlightMode == flightMode && 
                flightModeOnObject.CurrentItemAndDistance.Contains(spaceObject.Name);
        }

        public bool CheckFlightMode(FlightMode flightMode, OverviewItem spaceObject, Distance distance)
        {
            var flightModeOnObject = _client.Parser.HI.GetShipFlightMode();

            return flightModeOnObject.CurrentFlightMode == flightMode &&
                flightModeOnObject.CurrentItemAndDistance.Contains(spaceObject.Name) &&
                flightModeOnObject.CurrentItemAndDistance.Contains(distance.value.ToString());
        }

        public void OrbitObject(OverviewItem Object)
        {
            throw new NotImplementedException();
        }

        public void ApproachObject(OverviewItem Object)
        {
            throw new NotImplementedException();
        }

        public void SetSpeed()
        {
            throw new NotImplementedException();
        }

        internal void ShipStop()
        {
            throw new NotImplementedException();
        }
    }
}
