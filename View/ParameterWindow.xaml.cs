using System.Windows;
using ClientBaseMigration.Properties;
using ClientBase;

namespace ClientBaseMigration
{
    /// <summary>
    /// Логика взаимодействия для Parameters.xaml
    /// </summary>
    public partial class ParameterWindow : Window
    {
        public ParameterWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_Ok(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
            Button_Click_Cancel(sender, e);
        }

        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            WindowService.Instance.Close((ViewModel)DataContext);
        }
    }
}
