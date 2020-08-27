using ClientBase;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace ClientBaseMigration
{
    public class BusyViewModel : ViewModel
    {
        BusyWindow _busyWindow = null;

        private double _left;
        public double Left
        {
            get { return _left; }
            set { _left = value; }
        }

        private double _top;
        public double Top
        {
            get { return _top; }
            set { _top = value; }
        }

        private double _width;
        public double Width
        {
            get { return _width; }
            set { _width = value; }
        }

        private double _height;
        public double Height
        {
            get { return _height; }
            set { _height = value; }
        }

        object _busyWindowSync = new object();

        public BusyViewModel(double left, double top, double width = 300, double height = 300)
        {
            _left = left;
            _top = top;
            _width = width;
            _height = height;
        }

        public void ShowBusy()
        {
            //lock (_busyWindowSync)
            //{
            if (_busyWindow == null)
            {
                Thread newWindowThread = new Thread(AnimationThreadStartingPoint);
                newWindowThread.SetApartmentState(ApartmentState.STA);
                newWindowThread.IsBackground = true;
                newWindowThread.Name = "Buzy";
                newWindowThread.Start();
            }
            //}
        }

        private void AnimationThreadStartingPoint()
        {
            lock (_busyWindowSync)
            {
                if (_busyWindow == null)
                {
                    _busyWindow = new BusyWindow
                    {
                        DataContext = this,
                        Left = Left,
                        Top = Top,
                        Width = Width,
                        Height = Height
                    };
                    _busyWindow.Show();
                }
            }
            System.Windows.Threading.Dispatcher.Run();
        }

        public void HideBusy()
        {
            lock (_busyWindowSync)
            {
                if (_busyWindow != null)
                {
                    _busyWindow.Dispatcher.BeginInvoke((Action)_busyWindow.Close);



                }
            }
        }

    }
}
