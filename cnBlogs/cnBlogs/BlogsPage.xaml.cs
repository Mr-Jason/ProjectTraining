using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using cnBlogs.Model;
using System.Threading.Tasks;
using Coding4Fun.Toolkit.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using System.Windows.Controls.Primitives;

namespace cnBlogs
{
    public partial class BlogsPage : PhoneApplicationPage
    {
        private int indexPage = 1;
        ArticleCollection blogsSource;
        private string blogapp;
        private bool isLoad = true;
        public BlogsPage()
        {
            InitializeComponent();
            blogsSource = new ArticleCollection();
            this.lbBlogs.ItemsSource = blogsSource;
            Loaded += BlogsPage_Loaded;
        }

        protected async void BlogsPage_Loaded(object sender, RoutedEventArgs e)
        {
            //注册listBox的ScrollView的滚动事件
            RegisterScrollListBoxEvent(lbBlogs);
            if (isLoad)
                await GetArtcle(indexPage);
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
                    indexPage += 1;
                    await GetArtcle(indexPage);
                    #endregion
                }

                if (value <= min)
                {
                    #region Load New

                    #endregion
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.New)
            {
                if (NavigationContext.QueryString.ContainsKey("Blogapp"))
                {
                    blogapp = NavigationContext.QueryString["Blogapp"];
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            isLoad = false;
        }

        async Task GetArtcle(int pageIndex)
        {
            string url = until.GETBLOGSBYBLOGGER.Replace("{BLOGAPP}", blogapp).Replace("{PAGEINDEX}", pageIndex.ToString());
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
                    List<Blogs> blogs = new List<Blogs>();
                    XDocument doc = XDocument.Parse(html);
                    XNamespace d = @"http://www.w3.org/2005/Atom";

                    var bloglist = from query in doc.Descendants(d + "entry")
                                   select new Blogs
                                   {
                                       Id = (string)query.Element(d + "id") +"|"+(string)query.Element(d + "comments"),
                                       Title = (string)query.Element(d + "title"),
                                       Summary = (string)query.Element(d + "summary"),
                                       Published = (string)query.Element(d + "published"),

                                       Author = new Author
                                       {
                                           AuthorName = query.Element(d + "author").Element(d + "name").Value,
                                           AddressBlog = query.Element(d + "author").Element(d + "uri").Value,
                                           Avatar = "null"
                                       },
                                       Link = query.Element(d + "link").FirstAttribute.NextAttribute.Value.ToString(),
                                       Blogapp = (string)query.Element(d + "blogapp"),
                                       Diggs = (string)query.Element(d + "diggs"),
                                       Views = (string)query.Element(d + "views"),
                                       Comments = (string)query.Element(d + "comments")
                                   };
                    blogs = bloglist.ToList<Blogs>();
                    Dispatcher.BeginInvoke(() =>
                    {
                        for (int i = 0; i < blogs.Count; i++)
                        {
                            blogsSource.Add(blogs[i]);
                        }
                        tbBlogerName.Text = blogs[0].Author.AuthorName;
                        progressbar.Visibility = System.Windows.Visibility.Collapsed;
                    });
                });

            });
        }

        private void Border_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Border border = sender as Border;
            string[] str = border.Tag.ToString().Split('|');
            NavigationService.Navigate(new Uri(string.Format("/ArticleShow.xaml?ArticelType={0}&ArticleId={1}&CommentCount={2}", "Index",str[0], str[1]), UriKind.Relative));
        }

        private async void barRefreshIconBtn_Click(object sender, EventArgs e)
        {
            indexPage = 1;
            blogsSource.Clear();
            await GetArtcle(indexPage);
        }

        private void barTopIconBtn_Click(object sender, EventArgs e)
        {
            this.lbBlogs.ScrollIntoView(lbBlogs.Items[0]);
        }
    }
}