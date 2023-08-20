using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EVE_AutomatiX
{
    public abstract class ThreadWrapper
    {
        private Thread Worker;
        private Thread WorkerWrapper;

        public ThreadWrapper()
        {
            Worker = new Thread(Wrapper);
            WorkerWrapper = new Thread(() =>
            {
                while (true)
                {
                    if (ConditionToStartWorker())
                    {
                        EnsureStartThread();
                    }
                    else
                    {
                        EnsureDestroyThread();
                    }

                    Thread.Sleep(300);
                }
            });
        }
        public void StartThread()
        {
            WorkerWrapper.Start();
        }

        public abstract void Work();
        //{
        //    while (true)
        //    {
        //        Console.WriteLine("function for thread is working");

        //        Thread.Sleep(300);
        //    }
        //}

        public abstract bool ConditionToStartWorker();

        public virtual void EnsureDestroyThread()
        {
            if (Worker.IsAlive)
            {
                Console.WriteLine("stoping thread Worker");
                Worker.Interrupt();
                Worker.Join();
                //Emulators.AllowControlEmulator = true;
                //Emulators.ClickRB(500, 100);
            }
        }

        private void EnsureStartThread()
        {
            if (!Worker.IsAlive)
            {
                Console.WriteLine("starting thread Worker");
                Worker = new Thread(Wrapper);
                Worker.Start();
            }
        }

        private void Wrapper()
        {
            try
            {
                Work();
            }
            catch (ThreadInterruptedException) { }
        }
    }
}
