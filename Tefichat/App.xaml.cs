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
        RootVM rootVM = new RootVM();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);           
            rootVM.Start();
            MainWindow mainWindow = new MainWindow() { DataContext = rootVM };
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            rootVM.Close();
        }
    }
}
