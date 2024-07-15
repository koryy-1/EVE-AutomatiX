using Application.ClientWindow;
using Application.ClientWindow.UIHandlers;
using Domen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DroneService
    {
        private static Client _client;
        public DroneService()
        {
            var nick = "Yavisha";
            _client = Client.GetInstance(nick);
        }

        public void Launch()
        {
            _client.Emulators.Keyboard.PressButton((int)WinApi.VirtualKeyShort.VK_G);
        }

        public void Engage()
        {
            _client.Emulators.Keyboard.PressButton((int)WinApi.VirtualKeyShort.VK_F);
        }

        public void Scoop()
        {
            _client.Emulators.Keyboard.PressButton((int)WinApi.VirtualKeyShort.VK_H);
        }

        public IEnumerable<Drone> GetInfo()
        {
            return _client.Parser.Drones.GetInfo();
        }
    }
}
