using Application.ClientWindow.Parsers;
using Domen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ClientWindow
{
    public class Parser
    {
        public OV OV { get; set; }
        public HI HI { get; set; }
        public Drones Drones { get; set; }
        public Invent Invent { get; set; }
        public SI SI { get; set; }
        public NavPanel NavPanel { get; set; }
        public NP NP { get; set; }
        public PS PS { get; set; }
        public TB TB { get; set; }
        public DS DS { get; set; }
        public Chat Chat { get; set; }

        public Parser(ClientParams clientProcess)
        {
            OV = new OV(clientProcess);
            HI = new HI(clientProcess);
            Drones = new Drones(clientProcess);
            Invent = new Invent(clientProcess);
            SI = new SI(clientProcess);
            NavPanel = new NavPanel(clientProcess);
            NP = new NP(clientProcess);
            PS = new PS(clientProcess);
            TB = new TB(clientProcess);
            DS = new DS(clientProcess);
            Chat = new Chat(clientProcess);
        }
    }
}
