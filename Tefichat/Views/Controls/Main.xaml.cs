using System;
using System.Windows;
using System.Windows.Controls;

namespace Tefichat.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class Main : UserControl
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Тут будет меню большое!");
        }
    }
}
