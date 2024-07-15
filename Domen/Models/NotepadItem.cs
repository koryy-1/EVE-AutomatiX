using System;
using System.Collections.Generic;
using System.Text;

namespace Domen.Models
{
    public class NotepadItem
    {
        public NotepadItem()
        {
            Pos = new Point();
        }
        public Point Pos { get; set; }
    }
}
