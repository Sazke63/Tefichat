using System.Windows;
using Tefichat.ViewModels;
using Tefichat.Views;

namespace Tefichat
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            RootVM rootVM = new RootVM();
            rootVM.Start();
            MainWindow mainWindow = new MainWindow() { DataContext = rootVM };
            mainWindow.Show();
        }
    }
}
