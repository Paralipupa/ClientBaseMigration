using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ClientBase;


namespace ClientBaseMigration
{
    public static class WindowServiceExtention
    {

        public static void ShowThread(this WindowService ws, ViewModel viewmodel, ViewModel owner = null)
        {

            
            // создаем новый поток
            Thread myThread = new Thread(new ThreadStart(()=>
            {
                Window window = ws.CreateWindow(viewmodel, owner);
                window.Show(); 

            }));
            myThread.Start(); // запускаем поток

            //lock (_busyWindowSync)
            //{
            //    if (_busyWindow == null)
            //    {
            //        double left = Dispatcher.Invoke((Func<double>)(() => this.Left + this.Width / 2));
            //        double top = Dispatcher.Invoke((Func<double>)(() => this.Top + this.Height / 2));
            //        Thread newWindowThread = new Thread(new ParameterizedThreadStart(AnimationThreadStartingPoint));
            //        newWindowThread.SetApartmentState(ApartmentState.STA);
            //        newWindowThread.IsBackground = true;
            //        newWindowThread.Start(new Point() { X = left, Y = top });
            //    }
            //}
        }

        public static void CloseThread(this WindowService ws, ViewModel viewmodel)
        {
            Window window = ws.OpenWindows[viewmodel];
            window.Dispatcher.BeginInvoke((Action)window.Close);
        }

    }
}
