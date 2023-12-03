using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TL;

namespace Tefichat.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для Chat.xaml
    /// </summary>
    public partial class Chat : UserControl
    {
        double offset;
        double viewport;
        ListBoxItem? cont;
        Point point;

        public Chat()
        {
            InitializeComponent();
        }

        private void MessageItems_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //var height = MessageItems.ActualHeight;
            offset = e.VerticalOffset;
            viewport = e.ViewportHeight;

            //var topTreshold = offset;
            var bottomTreshold = offset + viewport;

            Point pointItem;

            int i = 0;
            var items = MessageItems.Items;

            if (items != null && items.Count > 1)
            {
                foreach (var item in items)
                {
                    var cont = (ListBoxItem)MessageItems.ItemContainerGenerator.ContainerFromItem(item);
                    if (cont != null)
                        pointItem = cont.TransformToAncestor(MessageItems).Transform(new Point(0, 0));
                    else return;

                    if (pointItem.Y > 0)
                    {
                        if (pointItem.Y > bottomTreshold) break;

                        //var top = pointItem.Y ; //listBoxItem.;
                        // - 20 + 4 is Margin in Style 
                        var bottom = pointItem.Y + cont.ActualHeight + offset - 12; //listBoxItem.Bounds.BottomLeft.Y;
                        if (bottom <= bottomTreshold) //top >= topTreshold && 
                        {
                            i++;
                            MessageItems.SelectedItem = item;
                            //var vm = (MainVM)DataContext;
                            //if (vm != null)
                            //{
                            //    if (vm.ReadMessageCommand.CanExecute(null))
                            //        vm.ReadMessageCommand.Execute(null);
                            //}
                        }
                    }
                }
            }
            //string path = @"C:\Users\Sazke\Documents\scrollposintion2.txt";
            //string pos = $"Height: {height}\nOffset: {offset}\nTopTreshold: {topTreshold}\nBottomTreshold: {bottomTreshold}\nPointItem: {pointItem.Y}\n\n";
            //File.AppendAllTextAsync(path, pos);
            //MessageBox.Show($"Height: {height}\nOffset: {offset}\nViewPort: {viewport}\nTopTreshold: {topTreshold}\nBottomTreshold: {bottomTreshold}\nPointItem: {pointItem.Y}\nItemsView {i}");

            /*if (items.Count > 2)
            {
                if (e.VerticalOffset < 3)
                {
                    MessageItems.SelectedItem = items[2];
                    //MessageItems.SelectedItem = items[(int)(items.Count / 2)];
                    //MessageBox.Show("Top" + e.VerticalOffset);
                }

                cont = (ListBoxItem)MessageItems.ItemContainerGenerator.ContainerFromItem(items[^1]);
                if (cont != null)
                    point = cont.TransformToAncestor(MessageItems).Transform(new Point(0, 0));

                if ((offset + viewport) > (point.Y + offset))
                {
                    MessageItems.SelectedItem = items[items.Count - 1];
                    //MessageBox.Show($"{bottomTreshold}\n{point.Y + offset}");
                }
            }*/
        }
    }
}
