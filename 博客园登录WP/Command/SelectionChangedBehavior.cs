using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace 博客园登录.Command
{
    public class SelectionChangedBehavior
    {
        public static readonly DependencyProperty SelectionChangedCommandProperty =
           DependencyProperty.RegisterAttached(
           "SelectionChangedCommand",
            typeof(ICommand),
            typeof(SelectionChangedBehavior),
            new PropertyMetadata(null, new PropertyChangedCallback(SelectionChangedPropertyChangedCallback)));

        public static ICommand GetSelectionChangedCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(SelectionChangedCommandProperty);
        }

        public static void SetSelectionChangedCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(SelectionChangedCommandProperty, value);
        }
        private static void SelectionChangedPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListView listview = (ListView)d;
            listview.SelectionChanged += listview_SelectionChanged;

        }

        static void listview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listview = (ListView)sender;
            GetSelectionChangedCommand(listview).Execute(listview.SelectedItem);
        }

    }
}
