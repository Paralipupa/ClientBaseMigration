using System.Windows;
using ClientBase;

namespace ClientBaseMigration
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
          
        }

        private void MenuItem_Click_Cancel(object sender, RoutedEventArgs e)
        {
            WindowService.Instance.Close((ViewModel)DataContext);

        }
    }
}
