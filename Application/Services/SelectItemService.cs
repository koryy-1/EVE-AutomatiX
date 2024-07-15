using Application.ClientWindow;
using Domen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SelectItemService
    {
        private static Client _client;

        public SelectItemService()
        {
            var nick = "Yavisha";
            _client = Client.GetInstance(nick);
        }

        public SelectedItemInfo GetInfo()
        {
            return _client.Parser.SI.GetInfo();
        }

        /// <summary>
        /// success if button exist and clicked
        /// </summary>
        /// <param name="btnName"></param>
        /// <returns>success</returns>
        public bool ClickButton(string btnName)
        {
            var selectedItemInfo = _client.Parser.SI.GetInfo();
            if (selectedItemInfo == null)
                return false;

            var button = selectedItemInfo.Buttons.Find(btn => btn.Action == btnName);
            if (button == null)
                return false;

            _client.Emulators.ClickLB(button.Pos);
            return true;
        }
    }
}
