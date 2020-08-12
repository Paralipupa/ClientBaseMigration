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

        private float _left;
        public float Left
        {
            get { return _left; }
            set { _left = value; }
        }

        private float _top;
            
        public float Top
        {
            get { return _top; }
            set { _top = value; }
        }

        private float _width;
        public float Width
        {
            get { return _width; }
            set { _width = value; }
        }

        private float _height;

        public float Height
        {
            get { return _height; }
            set { _height = value; }
        }

        object _busyWindowSync = new object();

        public BusyViewModel(float left, float top) 
        {
            Left = left;
            Top = top;
        }

        public void ShowBusy()
        {
            lock (_busyWindowSync)
            {
                if (_busyWindow == null)
                {
                    Thread newWindowThread = new Thread(new ThreadStart(AnimationThreadStartingPoint));
                    newWindowThread.SetApartmentState(ApartmentState.STA);
                    newWindowThread.IsBackground = true;
                    newWindowThread.Start();
                }
            }
        }

        private void AnimationThreadStartingPoint()
        {
            lock (_busyWindowSync)
            {
                if (_busyWindow == null)
                {
                    _busyWindow = new BusyWindow();
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
