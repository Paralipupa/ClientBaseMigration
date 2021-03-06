﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ClientBase;
using ClientBaseMigration.Properties;


namespace ClientBaseMigration
{
    class DataBaseNewViewModel : ViewModel
    {
        public DataBaseNewViewModel()
        {
            Database = new DataBaseNew();
            Excel = new ExcelData();
            IsClick = true;
        }

        private bool _isclick;
        public bool IsClick
        {
            get { return _isclick; }
            set
            {
                _isclick = value;
                base.RaisePropertyChanged("IsClick");
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set 
            { 
                _message = value;
                base.RaisePropertyChanged("Message");
            }
        }

        public DataBaseNew Database { get; set; }

        public ExcelData Excel { get; set; }

        public Sheet CurrentSheet { get; set; }

        public ICommand BusyCommand => new Command(
            _ =>
            {
                Task.Run(() =>
                {
                    BusyViewModel vm = new BusyViewModel(150, 150, 100, 100);
                    vm.ShowBusy();
                    System.Threading.Thread.Sleep(1000);
                    vm.HideBusy();
                });
            });

        public ICommand ToggleClickCommand => new Command(
            _ =>
            {
                Task.Run(delegate
                {
                    BusyViewModel vm = new BusyViewModel(100, 100, 100, 100);
                    vm.ShowBusy();
                    System.Threading.Thread.Sleep(5000);
                    vm.HideBusy();
                });
                Task.Run(() =>
                {
                    BusyViewModel vm = new BusyViewModel(200, 200, 200, 200);
                    vm.ShowBusy();
                    System.Threading.Thread.Sleep(3000);
                    vm.HideBusy();
                });
                Task.Run(() =>
                {
                    BusyViewModel vm = new BusyViewModel(300, 300, 300, 300);
                    vm.ShowBusy();
                    System.Threading.Thread.Sleep(4000);
                    vm.HideBusy();
                });

            });

        public ICommand ReadDataBaseCommand => new Command(
            _ =>
            {
                Task.Run(() =>
               {
                   BusyViewModel vm = new BusyViewModel(100, 100,100,100);
                   vm.ShowBusy();
                   IsClick = !IsClick;
                   Message = "Процесс...";
                   string filename = Settings.Default.PathExcelFile;
                   Database = new DataBaseNew();
                   if (Database.ReadFromExcel(filename))
                   {
                       if (Database.GenMigrate())
                       {
                           Database.JSonSerialization();
                       }


                   }
                   IsClick = !IsClick;
                   Message = "Ok.Завершен";
                   vm.HideBusy();
                   vm = null;
               }
                    );
            },
            _ => { return IsClick; }
            );
    
        private void Busy()
        {

        }

    }
}
