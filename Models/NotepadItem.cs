using System;
using System.Collections.Generic;
using System.Text;

namespace EVE_Bot.Models
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
