using Application.ClientWindow;
using Domen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class InventoryService
    {
        private static Client _client;

        public InventoryService()
        {
            var nick = "Yavisha";
            _client = Client.GetInstance(nick);
        }

        public IEnumerable<InventoryItem> GetInfo()
        {
            return _client.Parser.Invent.GetInfo();
        }

        public bool IsContainerOpened()
        {
            var btnPos = _client.Parser.Invent.GetLootAllBtnPos();
            if (btnPos != null)
                return true;

            return false;
        }

        public void LootAll()
        {
            var btnPos = _client.Parser.Invent.GetLootAllBtnPos();
            _client.Emulators.ClickLB(btnPos);
        }

        public void StackAll()
        {
            throw new NotImplementedException();
        }
    }
}
