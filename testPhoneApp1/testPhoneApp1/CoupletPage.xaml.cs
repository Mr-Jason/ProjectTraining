using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using testPhoneApp1.DataModel;
using testPhoneApp1.Utility;

namespace testPhoneApp1
{
    public partial class CoupletPage : PhoneApplicationPage
    {
        
        public CoupletPage()
        {
            InitializeComponent();
            Loaded += CoupletPage_Loaded;
        }

        void CoupletPage_Loaded(object sender, RoutedEventArgs e)
        {
            BindHapplyElements();
        }

        public async void BindHapplyElements()
        {
            var sampleDataSources = await SprigCoupletsDataSource.GetGroupsAsync();
            this.lbHappyElements.ItemsSource = sampleDataSources;
        }

        private void RoundButton_Click(object sender, RoutedEventArgs e)
        {
            HappyElement selected = ((sender as Button).DataContext as HappyElement);
            TileHelper._tileHelper.createTile(selected.ImagePath);
        }
    }
}