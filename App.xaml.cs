using AssignmentPRN212.Views;
using System.Configuration;
using System.Data;
using System.Windows;

namespace AssignmentPRN212
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Mở HomeWindow thay vì LoginWindow
            var homeWindow = new HomeWindow();
            homeWindow.Show();
        }
    }

}
