using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;
using Coding4Fun.Toolkit.Controls;
using System.Windows.Media;
using cnBlogs.Model;
using System.Xml.Linq;
using System.Windows.Controls.Primitives;

namespace cnBlogs
{
    public partial class BloggersPage : PhoneApplicationPage
    {
        BloggerCollection bloggersSources;
        private int pageIndex;
        private bool isLoad = true;
        public BloggersPage()
        {
            InitializeComponent();
            pageIndex = 1;
            bloggersSources = new BloggerCollection();
            this.lbBloggers.ItemsSource = bloggersSources;
            Loaded += BloggersPage_Loaded;
        }

        protected async void BloggersPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLoad)
                await GetBloggers(pageIndex);

            RegisterScrollListBoxEvent(lbBloggers);
        }

        private void Border_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Border boder = sender as Border;
            NavigationService.Navigate(new Uri("/BlogsPage.xaml?Blogapp="+boder.Tag, UriKind.Relative));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //isLoad = false;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            
            base.OnNavigatingFrom(e);
            isLoad = false;
        }

        async Task GetBloggers(int pageIndex)
        {
            string url = until.GETBLOGGERS.Replace("{PAGEINDEX}", pageIndex.ToString());
            await Task.Run(() =>
            {
                FetchHelper.HttpGetAsync(url, html =>
                {
                    if (html == "no network")
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            var toast = new ToastPrompt
                            {
                                Message = "提醒：很抱歉，您的网络已断开。",
                                Background = (Brush)Application.Current.Resources["PromptColor"],
                                Foreground = (Brush)Application.Current.Resources["Fontground"]
                            };
                            toast.Show();
                        });
                        return;
                    }
                    if (html == "network anomaly")
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            var toast = new ToastPrompt
                            {
                                Message = "提醒：很抱歉，您的网络貌似有异常。",
                                Background = (Brush)Application.Current.Resources["PromptColor"],
                                Foreground = (Brush)Application.Current.Resources["Fontground"]
                            };
                            toast.Show();
                        });
                        return;
                    }
                    List<Blogger> bloggers = new List<Blogger>();
                    XDocument doc = XDocument.Parse(html);
                    XNamespace d = @"http://www.w3.org/2005/Atom";

                    var bloggerslist = from query in doc.Descendants(d + "entry")
                                   select new Blogger
                                   {
                                       Id = (string)query.Element(d + "id"),
                                       Title = (string)query.Element(d + "title"),
                                       Updated = (string)query.Element(d + "updated"),
                                       Link = query.Element(d + "link").FirstAttribute.NextAttribute.Value.ToString(),
                                       Blogapp = (string)query.Element(d + "blogapp"),
                                       Avatar = (string)query.Element(d + "avatar"),
                                       Postcount = (string)query.Element(d + "postcount")
                                   };
                    bloggers = bloggerslist.ToList<Blogger>();
                    Dispatcher.BeginInvoke(() =>
                    {
                        for (int i = 0; i < bloggers.Count; i++)
                        {
                            bloggersSources.Add(bloggers[i]);
                        }
                        progressbar.Visibility = System.Windows.Visibility.Collapsed;
                    });
                });

            });
        }

        private void RegisterScrollListBoxEvent(ListBox listBox)
        {
            List<ScrollBar> controlScrollBarList = ScrollHelper.GetVisualChildCollection<ScrollBar>(listBox);
            if (controlScrollBarList == null)
                return;

            foreach (ScrollBar queryBar in controlScrollBarList)
            {
                if (queryBar.Orientation == System.Windows.Controls.Orientation.Vertical)
                    queryBar.ValueChanged += queryBar_ValueChanged;
            }
        }

        private async void queryBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ScrollBar scrollBar = (ScrollBar)sender;
            object valueObj = scrollBar.GetValue(ScrollBar.ValueProperty);
            object maxObj = scrollBar.GetValue(ScrollBar.MaximumProperty);
            object minObj = scrollBar.GetValue(ScrollBar.MinimumProperty);
            if (valueObj != null && maxObj != null)
            {
                double value = (double)valueObj;
                double max = (double)maxObj;
                double min = (double)minObj;
                if (value >= max)
                {
                    #region Load Old
                    progressbar.Visibility = System.Windows.Visibility.Visible;

                    pageIndex += 1;
                    await GetBloggers(pageIndex);
                    #endregion
                }

                if (value <= min)
                {
                    #region Load New

                    #endregion
                }
            }
        }

        private async void barRefreshIconBtn_Click(object sender, EventArgs e)
        {
            bloggersSources.Clear();
            pageIndex = 1;
            await GetBloggers(pageIndex);
        }

        private void barTopIconBtn_Click(object sender, EventArgs e)
        {
            this.lbBloggers.ScrollIntoView(lbBloggers.Items[0]);
        }
    }
}