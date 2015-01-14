using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Coding4Fun.Toolkit.Controls;
using System.Windows.Media;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using System.IO;
using cnBlogs.Model;
using System.Xml.Linq;

namespace cnBlogs
{
    public partial class MainPage : PhoneApplicationPage
    {
        //List<blog> blogs;
        //private string url;
        //private int pageNum = 0;
        //BackgroundWorker backgroundWorker;
        ////private string articleType = "http://www.cnblogs.com/";
        //private List<ListBox> lbs;
        //Popup _popUp;
        IsolatedStorageSettings aPPSetting = IsolatedStorageSettings.ApplicationSettings;  
        private int indexPage = 1;
        private int newsPage = 1;
        private string articleType = string.Empty;
        ArticleCollection artsource;
        NewsCollection newsArtSource;
        bool isExit = false;
        // 构造函数
        public MainPage()
        {
            InitializeComponent();
            ApplicationBar = new ApplicationBar();
            artsource = new ArticleCollection();
            newsArtSource = new NewsCollection();

            lbDFArticleLists.ItemsSource = artsource;
            lbNewsArticleLists.ItemsSource = newsArtSource;
            this.Loaded += MainPage_Loaded;
            ApplicationBar = ((ApplicationBar)this.Resources["defaultAppBar"]);
        }

        protected async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            RegisterScrollListBoxEvent(lbDFArticleLists);//注册滚动条事件
            RegisterScrollListBoxEvent(lbNewsArticleLists);
            articleType = "Index";
            await GetHomeArtcle(indexPage);
        }

        async Task GetHomeArtcle(int pageIndex)
        {
            string url = until.RECENTBLOG.Replace("{PAGEINDEX}", pageIndex.ToString());
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
                                      Id = (string)query.Element(d + "id")+ "|" + (string)query.Element(d + "blogapp"),
                                      Title = (string)query.Element(d + "title"),
                                      Summary = (string)query.Element(d + "summary"),
                                      Published = (string)query.Element(d + "published"),

                                      Author = new Author
                                      {
                                          AuthorName = query.Element(d + "author").Element(d + "name").Value,
                                          AddressBlog = query.Element(d + "author").Element(d + "uri").Value,
                                          Avatar = query.Element(d + "author").Element(d + "avatar").Value
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
                              artsource.Add(blogs[i]);
                          }
                          progressbar.Visibility = System.Windows.Visibility.Collapsed;
                      });
                   #region
                   //List<blog> blogs = new List<blog>();
                   //HtmlDocument htmlDoc = new HtmlDocument
                   //{
                   //    OptionAddDebuggingAttributes = false,
                   //    OptionAutoCloseOnEnd = true,
                   //    OptionFixNestedTags = true,
                   //    OptionReadEncoding = true
                   //};
                   //htmlDoc.LoadHtml(html);

                   //HtmlNode rootNode = htmlDoc.DocumentNode;
                   //HtmlNodeCollection categoryNodeList = rootNode.SelectNodes("//body//div[@class='post_item']");
                   //HtmlNode temp = null;
                   //foreach (HtmlNode categoryNode in categoryNodeList)
                   //{
                   //    temp = HtmlNode.CreateNode(categoryNode.OuterHtml);
                   //    blog blog = new blog();
                   //    blog.Title = temp.SelectSingleNode("/div[1]//a").InnerText.Trim();
                   //    blog.Author = temp.SelectSingleNode("/div[1]/div[2]/div[1]/a").InnerText.Trim().Trim();
                   //    blog.Link = temp.SelectSingleNode("/div[1]/div[2]/h3[1]/a[1]/@href[1]").Attributes["href"].Value.Trim();
                   //    blog.CreateTime = temp.SelectSingleNode("/div[1]/div[2]/div[1]").ChildNodes[2].InnerText.Trim();
                   //    blog.Summary = temp.SelectSingleNode("/div[1]/div[2]/p[1]").InnerText.Trim();
                   //    blog.Reader = temp.SelectSingleNode("/div[1]/div[2]/div[1]/span[2]/a[1]").InnerText.Trim();
                   //    blog.Review = temp.SelectSingleNode("/div[1]/div[2]/div[1]/span[1]/a[1]").InnerText.Trim();
                   //    blogs.Add(blog);
                   //}
                   #endregion
               });

           });
        }

        async Task GetNewsActcle(int pageIndex)
        {
            string url = until.RECENTNEWS.Replace("{PAGEINDEX}", pageIndex.ToString());
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

                                       //Author = new Author
                                       //{
                                       //    AuthorName = query.Element(d + "author").Element(d + "name").Value,
                                       //    AddressBlog = query.Element(d + "author").Element(d + "uri").Value,
                                       //    Avatar = query.Element(d + "author").Element(d + "avatar").Value
                                       //},
                                       Link = query.Element(d + "link").FirstAttribute.NextAttribute.Value.ToString(),
                                       Diggs = (string)query.Element(d + "diggs"),
                                       Views = (string)query.Element(d + "views"),
                                       Comments = (string)query.Element(d + "comments"),
                                       Topic = (string)query.Element(d + "topic"),
                                       TopicIcon =(string)query.Element(d + "topicIcon"),
                                       SourceName = (string)query.Element(d + "sourceName")
                                   };
                    news = newslist.ToList<News>();
                    Dispatcher.BeginInvoke(() =>
                    {
                        for (int i = 0; i < news.Count; i++)
                        {
                           // artsource.Add(blogs[i]);
                            newsArtSource.Add(news[i]);
                        }
                        progressbar.Visibility = System.Windows.Visibility.Collapsed;
                    });
                });

            });
        }

        #region listbox 滑动到底部自动加载
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

        async void queryBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
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
                    if (articleType.ToUpper() == "INDEX")
                    {
                        indexPage += 1;
                        await GetHomeArtcle(indexPage);
                    }
                    else
                    {
                        newsPage += 1;
                        await GetNewsActcle(newsPage);
                    }
                    #endregion
                }

                if (value <= min)
                {
                    #region Load New
                   
                    #endregion
                }
            }
        }
        #endregion

        #region
        //private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        //{
        //   // this.DoWork(backgroundWorker);
        //}

        //private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    if (e.Error == null)
        //    {
        //       // MessageBox.Show("OK");
        //        lbs[pageNum].ItemsSource = blogs;
              
        //    }
        //    else if (e.Error != null)
        //    {
        //        MessageBox.Show(e.Error.ToString());
        //    }
        //}

        //private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
            
        //}

        //private void DoWork(BackgroundWorker bk)
        //{
        //    FetchHelper fetch = new FetchHelper(url);
        //    blogs = new List<blog>();

        //    blogs = fetch.GetPageByHttpWebQuest();
        //}
        #endregion
        async void ShowArticlePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int pageIndex = ((Panorama)sender).SelectedIndex;
            articleType = ArticleTypeUrl.GetArticleTypeUrl(pageIndex);
            if (artsource.Count == 0 && articleType.ToUpper()=="INDEX")
            {
                progressbar.Visibility = System.Windows.Visibility.Visible;
                await GetHomeArtcle(indexPage);
            }
            if (newsArtSource.Count == 0 && articleType.ToUpper() == "NEWS")
            {
                progressbar.Visibility = System.Windows.Visibility.Visible;
                await GetNewsActcle(newsPage);
            }

            if (articleType == "OTHER")
            {
                ApplicationBar = ((ApplicationBar)this.Resources["secondAppBar"]);
            }
            else
            {
                ApplicationBar = ((ApplicationBar)this.Resources["defaultAppBar"]);
            }
            progressbar.Visibility = System.Windows.Visibility.Collapsed;
        }

        //private void ShowArticlePivot_LoadingPivotItem(object sender, PivotItemEventArgs e)
        //{
        //    //if (backgroundWorker.IsBusy)
        //    //{
        //    //    return;
        //    //}
        //    //backgroundWorker.RunWorkerAsync();
        //}

        private void Border_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Border border = sender as Border;
            string commentCount = "0";
            if (articleType.ToUpper() == "INDEX")
                commentCount = ((Blogs)(lbDFArticleLists.SelectedItem)).Comments;
            NavigationService.Navigate(new Uri(string.Format("/ArticleShow.xaml?ArticelType={0}&ArticleId={1}&CommentCount={2}",articleType,border.Tag,commentCount),UriKind.Relative));
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (!isExit)
            {
                isExit = true;
                var toast = new ToastPrompt
                {
                    Message = "提醒：再按一次返回键退出CnBlogs",
                    Background = (Brush)Application.Current.Resources["PromptColor"],
                    Foreground = (Brush)Application.Current.Resources["Fontground"]
                };
                toast.Completed += (o, ex) => { isExit = false; };
                toast.Show();

                #region test
                //_popUp = new Popup
                //{

                //    Child = new Border()
                //    {
                //        Child = new TextBlock()
                //        {
                //            Text = "提醒：再按一次返回键退出CnBlogs.",
                //            FontSize = 20,
                //            Foreground = new SolidColorBrush(Colors.Black),
                //            HorizontalAlignment = HorizontalAlignment.Center,
                //            VerticalAlignment = VerticalAlignment.Center

                //        },
                //        //Height = Application.Current.Host.Content.ActualHeight,
                //        //Width = Application.Current.Host.Content.ActualWidth,
                //        Background = new SolidColorBrush(Colors.LightGray),
                //        HorizontalAlignment = HorizontalAlignment.Center,
                //        VerticalAlignment = VerticalAlignment.Center
                //    },
                //};
                //_popUp.HorizontalAlignment = HorizontalAlignment.Center;
                //_popUp.VerticalAlignment = VerticalAlignment.Center;
                //_popUp.IsOpen = true;
                //_popUp.LayoutUpdated += _popUp_LayoutUpdated;
                #endregion

                e.Cancel = true;
            }
        }

        //void _popUp_LayoutUpdated(object sender, EventArgs e)
        //{
        //    //_popUp.Margin = new Thickness(
        //    //   (App.Current.Host.Content.ActualWidth - pborder.ActualWidth) / 2,
        //    //   (App.Current.Host.Content.ActualHeight - pborder.ActualHeight) / 2,
        //    //   0,
        //    //   0);
        //    System.Threading.Timer timer = new System.Threading.Timer(
        //           (state) =>
        //           {

        //               _popUp.Dispatcher.BeginInvoke(() =>
        //               {
        //                   _popUp.IsOpen = false;
        //                   isExit = false;
        //               });
        //           }, null, 1000, 1000);
        //}

        async void barRefreshIconBtn_Click(object sender, EventArgs e)
        {
            progressbar.Visibility = System.Windows.Visibility.Visible;
            if (articleType.ToUpper() == "INDEX")
            {
                artsource.Clear();
                indexPage = 1;
                await GetHomeArtcle(indexPage);
                
            }
            else if (articleType.ToUpper() == "NEWS")
            {
                newsArtSource.Clear();
                newsPage = 1;
                await GetHomeArtcle(newsPage);
            }
        }

        private void barTopIconBtn_Click(object sender, EventArgs e)
        {
            if (articleType.ToUpper() == "INDEX")
            {
                this.lbDFArticleLists.ScrollIntoView(this.lbDFArticleLists.Items[0]);
            }
            else if (articleType.ToUpper() == "NEWS")
            {
                this.lbNewsArticleLists.ScrollIntoView(this.lbNewsArticleLists.Items[0]);
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (artsource.Count == 0 && string.IsNullOrEmpty(articleType))
            {
                progressbar.Visibility = System.Windows.Visibility.Visible;
                await GetHomeArtcle(indexPage);
            }
            if (newsArtSource.Count == 0 && articleType.ToUpper() == "NEWS")
            {
                progressbar.Visibility = System.Windows.Visibility.Visible;
                await GetHomeArtcle(newsPage);
            }
            progressbar.Visibility = System.Windows.Visibility.Collapsed;
            base.OnNavigatedTo(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            NavigationService.Navigate(new Uri("/NewsPages.xaml?NewsType="+btn.Name, UriKind.Relative));
        }

        private void blog_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Name == "FourEight" || btn.Name == "TenDay")
            {
                NavigationService.Navigate(new Uri("/blogsPages.xaml?BlogsType=" + btn.Name, UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/BloggersPage.xaml", UriKind.Relative));
            }
        }

        private void aboutAppbar_Click(object sender, EventArgs e)
        {

        }

        private void btnIng_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ingPage.xaml", UriKind.Relative));
        }

        private void btnAccount_Click(object sender, RoutedEventArgs e)
        {
            if (aPPSetting.Count == 0)// aPPSetting["islogin"] != "true"
            {
                if (MessageBox.Show("请先登录", "温馨提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {

                }
            }
            else
            {

            }
        }
        
        // 用于生成本地化 ApplicationBar 的示例代码
        //private void BuildLocalizedApplicationBar()
        //{
        //    // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
        //    ApplicationBar = new ApplicationBar();

        //    // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // 使用 AppResources 中的本地化字符串创建新菜单项。
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}