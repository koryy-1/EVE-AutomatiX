using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Models
{
    public class ClientParams
    {
        public string ProcessName { get; set; }
        public int ProcessId { get; set; }
        public string RootAddress { get; set; }
        public IntPtr hWnd { get; set; }
    }
}
