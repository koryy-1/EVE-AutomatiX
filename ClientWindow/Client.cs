using EVE_AutomatiX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVE_AutomatiX.ClientWindow
{
    public class Client
    {
        public Parser Parser;
        private string _nickName;
        private Config _config;

        public Client(ref Config config, string nickName)
        {
            _config = config;
            _nickName = nickName;
            Parser = new Parser(config.ClientParams);
        }

        public bool UpdateRootAddress()
        {
            return UIRootAddress.UpdateRootAddress(ref _config, _nickName);
        }
    }
}
