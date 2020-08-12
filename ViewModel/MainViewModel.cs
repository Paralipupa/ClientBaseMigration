using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ClientBase;
using ClientBaseMigration.Properties;

namespace ClientBaseMigration
{

    class MainViewModel : ViewModel
    {
        public ICommand ParameterCommand => new Command(
            _ =>
            {
                WindowService.Instance.ShowModal(new ParameterViewModel());
            });

        public ICommand DatabaseNewCommand => new Command(
            _ =>
            {
                DataBaseNewViewModel vm = new DataBaseNewViewModel();
                WindowService.Instance.Show(vm, this);

            });


    }
}
