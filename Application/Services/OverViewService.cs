using Application.ClientWindow;
using Application.ClientWindow.UIHandlers;
using Domen.Models;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class OverViewService
    {
        private static Client _client;

        public OverViewService()
        {
            var nick = "Yavisha";
            _client = Client.GetInstance(nick);
        }

        public IEnumerable<OverviewItem> GetInfo()
        {
            return _client.Parser.OV.GetInfo();
        }

        public void ClickOnObject(OverviewItem spaceObject)
        {
            _client.Emulators.ClickLB(spaceObject.Pos);
        }

        public void LockTargetByName(string targetName)
        {
            var target = _client.Parser.OV.GetInfo().Find(item => item.Name == targetName);
            if (target == null)
                return;

            _client.Emulators.ClickLB(target.Pos);
        }

        public void LockTargets(IEnumerable<OverviewItem> targets)
        {
            _client.Emulators.LockTargets(targets);
        }
    }
}
