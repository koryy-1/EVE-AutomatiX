using Application.ClientWindow.UIHandlers;
using Domen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ClientWindow
{
    public class Emulators
    {
        public Mouse Mouse { get; set; }
        public Keyboard Keyboard { get; set; }

        IntPtr _hWnd;
        private readonly object _lock = new object();

        //static public WinApi.Rect windowRect = new WinApi.Rect();


        //volatile static public WinApi.Point LastCoords = new WinApi.Point();
        volatile static public int LastX = 0;
        volatile static public int LastY = 0;

        Mouse1 Mouse1;

        public Emulators(IntPtr hwnd)
        {
            _hWnd = hwnd;
            Mouse = new Mouse(_hWnd);
            Keyboard = new Keyboard(_hWnd);

            Mouse1 = new Mouse1(_hWnd);
        }

        static Random r = new Random();

        public void LockTargets(IEnumerable<OverviewItem> targets)
        {
            lock (_lock)
            {
                Keyboard.KeyDown((int)WinApi.VirtualKeyShort.VK_CONTROL);
                Thread.Sleep(500);
                foreach (var target in targets)
                {
                    ClickLBForLockTargets(target.Pos);
                }
                Keyboard.KeyUp((int)WinApi.VirtualKeyShort.VK_CONTROL);
                Thread.Sleep(500);
            }
        }

        public void ClickLBForLockTargets(Point point)
        {
            point.x += r.Next(-2, 2);
            point.y += r.Next(-2, 2);

            Mouse1.Move(point.x, point.y);
            Thread.Sleep(100);
            Mouse.ClickLB(point);
        }

        public void ClickLB(Point point)
        {
            point.x += r.Next(-2, 2);
            point.y += r.Next(-2, 2);

            lock (_lock)
            {
                Mouse1.Move(point.x, point.y);
                Thread.Sleep(100);
                Mouse.ClickLB(point);
            }
        }

        public void ClickRB(Point point)
        {
            point.x += r.Next(-2, 2);
            point.y += r.Next(-2, 2);

            lock (_lock)
            {
                Mouse1.Move(point.x, point.y);
                Thread.Sleep(100);
                Mouse.ClickRB(point);
            }
        }

        public void Drag(int DownX, int DownY, int UpX, int UpY)
        {
            DownX += r.Next(-2, 2);
            DownY += r.Next(-2, 2);

            UpX += r.Next(-2, 2);
            UpY += r.Next(-2, 2);

            //WinApi.PostMessage(hWnd, (uint)WinApi.MouseMessages.WM_MOUSEMOVE, 0, WinApi.MakeLParam(DownX, DownY));
            Mouse1.Move(DownX, DownY);
            System.Threading.Thread.Sleep(100);
            WinApi.PostMessage(_hWnd, (uint)WinApi.MouseMessages.WM_LBUTTONDOWN, (int)WinApi.VirtualKeyShort.LBUTTON, WinApi.MakeLParam(DownX, DownY));
            System.Threading.Thread.Sleep(100);

            //WinApi.PostMessage(hWnd, (uint)WinApi.MouseMessages.WM_MOUSEMOVE, 0, WinApi.MakeLParam(UpX, UpY));
            Mouse1.Move(UpX, UpY);
            System.Threading.Thread.Sleep(500);
            WinApi.PostMessage(_hWnd, (uint)WinApi.MouseMessages.WM_LBUTTONUP, (int)WinApi.VirtualKeyShort.LBUTTON, WinApi.MakeLParam(UpX, UpY));
            System.Threading.Thread.Sleep(100);
        }
    }

    public class Mouse1
    {
        static int X1;
        static int Y1;

        IntPtr hWnd;

        //static int X2;
        //static int Y2;

        static Random r = new Random();
        public Mouse1(IntPtr hwnd)
        {
            WinApi.Point LastCoords = new WinApi.Point();
            WinApi.GetCursorPos(out LastCoords);
            Emulators.LastX = LastCoords.x;
            Emulators.LastY = LastCoords.y;
            hWnd = hwnd;
        }
        public void Move(int x, int y)
        {
            //x = x + Emulators.windowRect.left + r.Next(-2, 2);
            //y = y + Emulators.windowRect.top + r.Next(-2, 2);
            X1 = Emulators.LastX;
            Y1 = Emulators.LastY;

            if (X1 == x && Y1 == y)
                return;

            //// константа по времени
            //var t = 100 + r.Next(-50, 50);

            //double vx = sx / t;
            //double vy = sy / t;

            //константа по скорости
            double v = 130 + r.Next(-2, 2);

            double sx = x - X1;
            double sy = y - Y1;

            double s = Math.Sqrt(Math.Pow(sx, 2) + Math.Pow(sy, 2));

            double t = s / v;

            var vx = sx / t;
            var vy = sy / t;

            double XСutOnScreen = vx; // время кадра = 7
            double XDistanceComplete = 0;

            double YСutOnScreen = vy;
            double YDistanceComplete = 0;

            // todo: may be WinApi.PostMessage(hWnd, (uint)WinApi.MouseMessages.WM_MOUSEMOVE, 0, WinApi.MakeLParam(x, y));
            for (; Math.Abs(XDistanceComplete) < Math.Abs(sx) || Math.Abs(YDistanceComplete) < Math.Abs(sy); XDistanceComplete += XСutOnScreen, YDistanceComplete += YСutOnScreen)
            {
                WinApi.PostMessage(hWnd, (uint)WinApi.MouseMessages.WM_MOUSEMOVE, 0, WinApi.MakeLParam(X1 + Convert.ToInt32(XDistanceComplete), Y1 + Convert.ToInt32(YDistanceComplete)));
                //WinApi.SetCursorPos(X1 + Convert.ToInt32(XDistanceComplete), Y1 + Convert.ToInt32(YDistanceComplete));
                //Console.WriteLine($"X {X1 + Convert.ToInt32(XDistanceComplete)} Y {Y1 + Convert.ToInt32(YDistanceComplete)}");
                System.Threading.Thread.Sleep(Convert.ToInt32(t) + r.Next(Convert.ToInt32(-t * 0.1), Convert.ToInt32(t * 0.1)));
            }
            WinApi.PostMessage(hWnd, (uint)WinApi.MouseMessages.WM_MOUSEMOVE, 0, WinApi.MakeLParam(x, y));
            //WinApi.SetCursorPos(x, y+23);
            //Console.WriteLine($"X {x} Y {y}");
            System.Threading.Thread.Sleep(Convert.ToInt32(t) + r.Next(Convert.ToInt32(-t * 0.1), Convert.ToInt32(t * 0.1)));
            Emulators.LastX = x;
            Emulators.LastY = y;
        }
    }
}
