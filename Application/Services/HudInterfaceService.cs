using Application.ClientWindow;
using Domen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class HudInterfaceService
    {
        private static Client _client;

        public HudInterfaceService()
        {
            var nick = "Yavisha";
            _client = Client.GetInstance(nick);
        }

        public HudInterface GetInfo()
        {
            //todo: enum.tostring() via automapper
            return _client.Parser.HI.GetInfo();
        }

        public HealthPoints GetShipHP()
        {
            return _client.Parser.HI.GetShipHP();
        }

        public int GetCurrentSpeed()
        {
            return _client.Parser.HI.GetCurrentSpeed();
        }

        public IEnumerable<ShipModule> GetAllModules()
        {
            return _client.Parser.HI.GetAllModulesInfo();
        }

        public ShipFlightMode GetShipFlightMode()
        {
            return _client.Parser.HI.GetShipFlightMode();
        }

        public Point GetCenterPos()
        {
            return _client.Parser.HI.GetCenterPos();
        }

        public void ToggleActivationModule(string moduleName)
        {
            var module = _client.Parser.HI.GetAllModulesInfo()
                .Find(item => item.Name.ToString() == moduleName);
            if (module == null)
                return;

            _client.Emulators.Keyboard.PressButton(module.VirtualKey);
        }
    }
}
