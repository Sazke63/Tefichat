using System;
using System.Collections.Generic;
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

namespace Tefichat.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для Chat.xaml
    /// </summary>
    public partial class Chat : UserControl
    {
        public Chat()
        {
            InitializeComponent();
        }

        private void MessageItems_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //var height = MessageItems.ActualHeight;
            var offset = e.VerticalOffset;
            var viewport = e.ViewportHeight;

            //var topTreshold = offset;
            var bottomTreshold = offset + viewport;

            Point pointItem = new Point();

            int i = 0;
            var items = MessageItems.Items;

            //int from = Int32.MaxValue;
            //int to = Int32.MinValue;

            if (items != null && items.Count > 1)
            {
                foreach (var item in items)
                {
                    //var listBoxItem = (ListBoxItem)MessageItems.ItemContainerGenerator.ContainerFromIndex(i);
                    var cont = (ListBoxItem)MessageItems.ItemContainerGenerator.ContainerFromItem(item);
                    pointItem = cont.TransformToAncestor(MessageItems).Transform(new Point(0, 0));
                    //var data = cont;


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
                            //if (i < from)
                            //{
                            //    from = i;
                            //}

                            //if (i > to)
                            //{
                            //    to = i;
                            //}
                        }
                    }
                }
            }

            //MessageBox.Show($"Offset: {offset}\nViewPort: {viewport}\nTopTreshold: {topTreshold}\nBottomTreshold: {bottomTreshold}\nPointItem: {pointItem.Y}\nItemsView {i}");

            //if (e.VerticalOffset < 10 && items.Count > 2)
            //{
            //    MessageBox.Show("OPA");
            //}

        }
    }
}
