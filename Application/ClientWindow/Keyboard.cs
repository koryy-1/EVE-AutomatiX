using Application.ClientWindow.UIHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ClientWindow
{
    public class Keyboard
    {
        private IntPtr _hWnd;

        public Keyboard(IntPtr hWnd)
        {
            _hWnd = hWnd;
        }

        public void PressButton(int Button)
        {
            KeyDown(Button);
            KeyUp(Button);
        }

        public void KeyDown(int Button)
        {
            WinApi.PostMessage(_hWnd, (uint)WinApi.KeyboardMessages.WM_IME_KEYDOWN, Button, 0);
            Thread.Sleep(100);
        }

        public void KeyUp(int Button)
        {
            WinApi.PostMessage(_hWnd, (uint)WinApi.KeyboardMessages.WM_IME_KEYUP, Button, 0);
            Thread.Sleep(100);
        }
    }
}
