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
    public partial class NewsPages : PhoneApplicationPage
    {
        NewsCollection hotNewsSource;
        NewsCollection recommNewsSource;
        private string newsUrl = string.Empty;
        private string newsType = "HOTNEWS";
        private int newsPageIndex = 1;
        public NewsPages()
        {
            InitializeComponent();
            hotNewsSource = new NewsCollection();
            recommNewsSource = new NewsCollection();
            lbhotNews.ItemsSource = hotNewsSource;
            lbrecommNews.ItemsSource = recommNewsSource;
            Loaded += NewsPages_Loaded;
        }

        void NewsPages_Loaded(object sender, RoutedEventArgs e)
        {
            RegisterScrollListBoxEvent(lbrecommNews);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.New)
            {
                if (NavigationContext.QueryString.ContainsKey("NewsType"))
                {
                    newsType = NavigationContext.QueryString["NewsType"];
                    if (newsType.ToUpper() != "HOTNEWS")
                        newsPivot.SelectedIndex = 1;
                   // await GetNewsActicle(1);
                }
            }
        }

        private void Border_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Border border = sender as Border;
            NavigationService.Navigate(new Uri(string.Format("/ArticleShow.xaml?ArticelType={0}&ArticleId={1}", "news", border.Tag), UriKind.Relative));
        }

        async Task GetNewsActicle(int pageIndex)
        {
            if (newsType.ToUpper() == "HOTNEWS")
            {
                newsUrl = until.HOTNEWS + 30;
            }
            else
            {
                newsUrl = until.RecommendNEWS.Replace("{PAGEINDEX}", pageIndex.ToString());
            }
            await Task.Run(() =>
            {
                FetchHelper.HttpGetAsync(newsUrl, html =>
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
                    List<News> news = new List<News>();
                    XDocument doc = XDocument.Parse(html);
                    XNamespace d = @"http://www.w3.org/2005/Atom";

                    var newslist = from query in doc.Descendants(d + "entry")
                                   select new News
                                   {
                                       Id = (string)query.Element(d + "id"),
                                       Title = (string)query.Element(d + "title"),
                                       Summary = (string)query.Element(d + "summary"),
                                       Published = (string)query.Element(d + "published"),
                                       Link = query.Element(d + "link").FirstAttribute.NextAttribute.Value.ToString(),
                                       Diggs = (string)query.Element(d + "diggs"),
                                       Views = (string)query.Element(d + "views"),
                                       Comments = (string)query.Element(d + "comments"),
                                       Topic = (string)query.Element(d + "topic"),
                                       TopicIcon = (string)query.Element(d + "topicIcon"),
                                       SourceName = (string)query.Element(d + "sourceName")
                                   };
                    news = newslist.ToList<News>();
                    Dispatcher.BeginInvoke(() =>
                    {
                        for (int i = 0; i < news.Count; i++)
                        {
                            if (newsType.ToUpper() == "HOTNEWS")
                                hotNewsSource.Add(news[i]);
                            else
                                recommNewsSource.Add(news[i]);
                        }
                        progressbar.Visibility = System.Windows.Visibility.Collapsed;
                    });
                });

            });
        }

        private async void newsPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PivotItem item = ((Microsoft.Phone.Controls.PivotItem)(((Pivot)sender).SelectedItem));
            newsType = item.Name;
            if (hotNewsSource.Count == 0 && newsType.ToUpper()=="HOTNEWS")
            {
                progressbar.Visibility = System.Windows.Visibility.Visible;
                newsType = "HOTNEWS";
                await GetNewsActicle(1);
            }
            else if (recommNewsSource.Count == 0 && newsType.ToUpper() == "RECOMMNEWS")
            {
                progressbar.Visibility = System.Windows.Visibility.Visible;
                newsType = "RECOMMNEWS";
                await GetNewsActicle(1);
            }
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
                    //if (articleType.ToUpper() == "INDEX")
                    //{
                      newsPageIndex += 1;
                      await GetNewsActicle(newsPageIndex);
                    //}
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
            progressbar.Visibility = System.Windows.Visibility.Visible;
            if (newsType.ToUpper() == "HOTNEWS")
            {
                hotNewsSource.Clear();
            }
            else
            {
                recommNewsSource.Clear();
            }
            newsPageIndex = 1;
            await GetNewsActicle(newsPageIndex);
        }

        private void barTopIconBtn_Click(object sender, EventArgs e)
        {
            if (newsType.ToUpper() == "HOTNEWS")
            {
                this.lbhotNews.ScrollIntoView(this.lbhotNews.Items[0]);
            }
            else
            {
                this.lbrecommNews.ScrollIntoView(this.lbrecommNews.Items[0]);
            }
        }
    }
}