using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.dto
{
    public class ClientParamsDto
    {
        public string ProcessName { get; set; }
        public int ProcessId { get; set; }
        public string RootAddress { get; set; }
        public int hWnd { get; set; }
    }
}
