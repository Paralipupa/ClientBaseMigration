using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ClientBase;

namespace ClientBaseMigration
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {

            WindowService windows = WindowService.Instance;

            windows.Register<MainViewModel, MainWindow>();
            windows.Register<ParameterViewModel, ParameterWindow>();
            windows.Register<DataBaseNewViewModel,DataBaseNewWindow>();
            windows.Register<BusyViewModel, BusyWindow>();

            WindowService.Instance.Show(new MainViewModel());
        }
    }
}
