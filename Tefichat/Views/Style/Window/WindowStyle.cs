using System;
using System.Windows;
using System.Windows.Controls;

namespace Tefichat.Views.Style.Window
{
    partial class WindowStyle
    {
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as System.Windows.Window).StateChanged += Window_StateChanged;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            System.Windows.Window me = (sender as System.Windows.Window);
            Button maximizeCaptionButton = me.Template.FindName("btnMaxRestore", me) as Button;
            if (maximizeCaptionButton != null)
            {
                maximizeCaptionButton.Content = me.WindowState == WindowState.Maximized ? "2" : "1";
            }
            me.MaxHeight = SystemParameters.WorkArea.Height + 14;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            ((sender as FrameworkElement).TemplatedParent as System.Windows.Window).Close();
        }

        private void btnMaxRestore_Click(object sender, RoutedEventArgs e)
        {
            var window = ((sender as FrameworkElement).TemplatedParent as System.Windows.Window);
            if (window != null)
                window.WindowState = window.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;

            //((sender as FrameworkElement).TemplatedParent as System.Windows.Window)
            //    .WindowState = (((sender as FrameworkElement).TemplatedParent as System.Windows.Window)
            //    .WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            
            ((sender as FrameworkElement).TemplatedParent as System.Windows.Window)
                .WindowState = WindowState.Minimized;
        }
    }
}
