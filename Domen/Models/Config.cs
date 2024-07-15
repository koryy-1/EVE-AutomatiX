using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Models
{
    public class Config
    {
        public int _protocol { get; set; } = Protocol.Autopilot;
        //public string _nickname { get; set; } = "nickname";
        public bool FarmExp { get; set; } = false;
        public int LockRange { get; set; } = 30;
        public int WeaponsRange { get; set; } = 20;
        public string Charges { get; set; } = "Scourge Light Missile";
        public string ShipName { get; set; } = "Caracal";
        public int AverageDelay { get; set; } = 500;
        public int LimiteCargoVolumeForUnload { get; set; } = 300;

        BotMode _botMode { get; set; }

        public ClientParams ClientParams { get; set; } = new ClientParams();

        public Config(
            string protocol,
            //string nickname,
            bool farmExp,
            int lockRange,
            int weaponRange,
            string charges,
            string shipName,
            int averageDelay,
            int limiteCargoVolumeForUnload
            )
        {
            _protocol = Convert.ToInt32(protocol);
            //_nickname = nickname;
            FarmExp = farmExp;
            LockRange = lockRange;
            WeaponsRange = weaponRange;
            Charges = charges;
            ShipName = shipName;
            AverageDelay = averageDelay;
            LimiteCargoVolumeForUnload = limiteCargoVolumeForUnload;
        }

        public Config()
        {
        }
    }
    public static class Protocol
    {
        public static int Autopilot = 0;
        public static int FarmNPC = 1;
    }
}
