using EVE_AutomatiX.Models;
using EVE_Bot.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EVE_Bot.Parsers
{
    public class AI
    {
        ClientParams _clientParams;

        public AI(ClientParams clientParams)
        {
            _clientParams = clientParams;
        }

        public ActiveItemInfo GetInfo()
        {
            throw new NotImplementedException();
            return new ActiveItemInfo();
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }

        public Distance GetDistance()
        {
            throw new NotImplementedException();
        }

        public ActiveItemButton GetButton(SpaceObjectAction objectAction)
        {
            throw new NotImplementedException();
        }
    }
}
