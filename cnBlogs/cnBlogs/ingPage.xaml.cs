using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using cnBlogs.Model;

namespace cnBlogs
{
    public partial class ingPage : PhoneApplicationPage
    {
        IngCollection ingSource;
        public ingPage()
        {
            InitializeComponent();
            ingSource = new IngCollection();
            lbIngs.ItemsSource = ingSource;
            Loaded += ingPage_Loaded;
        }

        async void ingPage_Loaded(object sender, RoutedEventArgs e)
        {
            string url = until.GETINGLINE.Replace("{1}", "1");
            await GetIngs(url);
        }

        async Task GetIngs(string url)
        {
            await Task.Run(() =>
            {
                
                FetchHelper.HttpGetAsync(url, html =>
                {
                    if (html == "no network")
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(() => { MessageBox.Show("提醒：很抱歉，您的网络已断开。"); });
                        });
                        return;
                    }
                    if (html == "network anomaly")
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(() => { MessageBox.Show("提醒：很抱歉，您的网络貌似有异常。"); });
                        });
                        return;
                    }
                    JObject jsonObject = JObject.Parse(html);
                    string dd = (jsonObject["data"]).ToString();
                    Ing data = (Ing)JsonConvert.DeserializeObject((jsonObject["data"]).ToString(), typeof(Ing));
                    
                });
            });
        }
    }
}