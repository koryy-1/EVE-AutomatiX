using Application.ClientWindow.UIHandlers;
using Domen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ClientWindow
{
    public class Mouse
    {
        private IntPtr _hWnd;

        public Mouse(IntPtr hWnd)
        {
            _hWnd = hWnd;
        }

        public void ClickLB(Point point)
        {
            WinApi.PostMessage(_hWnd, (uint)WinApi.MouseMessages.WM_LBUTTONDOWN, (int)WinApi.VirtualKeyShort.LBUTTON, WinApi.MakeLParam(point.x, point.y));
            Thread.Sleep(100);
            WinApi.PostMessage(_hWnd, (uint)WinApi.MouseMessages.WM_LBUTTONUP, (int)WinApi.VirtualKeyShort.LBUTTON, WinApi.MakeLParam(point.x, point.y));
            Thread.Sleep(100);
        }

        public void ClickRB(Point point)
        {
            WinApi.PostMessage(_hWnd, (uint)WinApi.MouseMessages.WM_RBUTTONDOWN, (int)WinApi.VirtualKeyShort.RBUTTON, WinApi.MakeLParam(point.x, point.y));
            Thread.Sleep(100);
            WinApi.PostMessage(_hWnd, (uint)WinApi.MouseMessages.WM_RBUTTONUP, (int)WinApi.VirtualKeyShort.RBUTTON, WinApi.MakeLParam(point.x, point.y));
            Thread.Sleep(100);
        }
    }
}
