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
using Newtonsoft.Json;
using HtmlAgilityPack;
using cnBlogs.Model;
using System.Xml.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using System.IO.IsolatedStorage;
using System.Windows.Controls.Primitives;
using System.ComponentModel;

namespace cnBlogs
{
    public partial class CommentPage : PhoneApplicationPage
    {
        IsolatedStorageSettings aPPSetting = IsolatedStorageSettings.ApplicationSettings;
        Popup activePopup;
        private int pageIndex;
        private string blogApp = string.Empty;
        private string postId = string.Empty;
        private int commentCount = 0;
        private int loadComment = 0;
        CommentCollection commSource;
        public CommentPage()
        {
            InitializeComponent();
            commSource = new CommentCollection();
            this.lbComments.ItemsSource = commSource;
            appbar = this.ApplicationBar as ApplicationBar;
            writeCommBarBtn = appbar.Buttons[0] as ApplicationBarIconButton;
            this.Loaded += CommentPage_Loaded;
        }

        protected async void CommentPage_Loaded(object sender, RoutedEventArgs e)
        {
            pageIndex = 1;
            RegisterScrollListBoxEvent(lbComments);
            if (commentCount > 0)
                await GetComments(pageIndex);
            else
                this.progressbar.Visibility = System.Windows.Visibility.Collapsed;
        }

        public async Task GetComments(int pageIndex)
        {
            if (NavigationContext.QueryString.ContainsKey("PostId"))//PostId
            {

                if (!string.IsNullOrEmpty(NavigationContext.QueryString["PostId"].ToString()))
                {
                     postId = NavigationContext.QueryString["PostId"].ToString();
                     string url =string.Empty;

                     loadComment = commentCount - 15 < 0 ? commentCount : 15;
                     commentCount = commentCount - loadComment;

                     if (NavigationContext.QueryString["ArticleType"].ToUpper() == "NEWS")
                         url = until.NEWSCOMMENT.Replace("{CONTENTID}", postId);
                     else
                         url = until.BLOGCOMMENT.Replace("{POSTID}", postId);

                     url = url.Replace("{PAGEINDEX}", pageIndex.ToString()) + loadComment;
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
                            List<Comment> liscomments = new List<Comment>();
                            XDocument doc = XDocument.Parse(html);
                            XNamespace d = @"http://www.w3.org/2005/Atom";

                            var comments = from query in doc.Descendants(d + "entry")
                                           select new Comment
                                           {
                                               Id = (string)query.Element(d + "id"),
                                               Author = new CommentAuthor
                                               {
                                                   Name = query.Element(d + "author").Element(d + "name").Value,
                                                   Uri = query.Element(d + "author").Element(d + "uri").Value
                                               },
                                               Content = (string)query.Element(d + "content"),
                                               Published = (string)query.Element(d + "published")

                                           };
                            Dispatcher.BeginInvoke(() =>
                            {
                                liscomments = comments.ToList<Comment>();
                                for (int i = 0; i < liscomments.Count; i++)
                                {
                                    commSource.Add(liscomments[i]);
                                }
                                progressbar.Visibility = System.Windows.Visibility.Collapsed;
                            });
                        });
                    });
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.New)
            {
                if (NavigationContext.QueryString.ContainsKey("blogApp"))
                {
                    blogApp = NavigationContext.QueryString["blogApp"];
                    commentCount = int.Parse(NavigationContext.QueryString["commCount"]);
                }
            }
        }

        private void writeCommBarBtn_Click(object sender, EventArgs e)
        {
           // IsolatedStorageSettings aPPSetting = IsolatedStorageSettings.ApplicationSettings;
            if (aPPSetting.Count == 0)
            {
                AccountLoginPopup.IsOpen = true;
                activePopup = AccountLoginPopup;
                EnableViewerAndAppBar(false);
            }
            else
            {
                SendCommentPopup.IsOpen = true;
                sendComment.blogApp = blogApp;
                sendComment.postId = postId;
                activePopup = SendCommentPopup;
                EnableViewerAndAppBar(false);
            }
        }

        void EnableViewerAndAppBar(bool isEnabled)
        {
            writeCommBarBtn.IsEnabled = isEnabled;
        }

        private void cnBlogsLogin_DialogDismissed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AccountLoginPopup.IsOpen = false;
            activePopup = null;
            if (aPPSetting.Count != 0 && (bool)aPPSetting["islogin"])
            {
                sendComment.blogApp = blogApp;
                sendComment.postId = postId;
                activePopup = SendCommentPopup;
                EnableViewerAndAppBar(false);
                SendCommentPopup.IsOpen = true;
            }
            else
            {
                EnableViewerAndAppBar(true);
            }
        }

        protected override void OnBackKeyPress(CancelEventArgs args)
        {
            if (activePopup != null)
            {
                activePopup.IsOpen = false;
                activePopup = null;
                EnableViewerAndAppBar(true);
                args.Cancel = true;
            }

            base.OnBackKeyPress(args);
        }

        private async void SendComment_DialogDismissed(object sender, CancelEventArgs e)
        {
            SendCommentPopup.IsOpen = false;
            
            activePopup = null;
            commentCount += 1;
            progressbar.Visibility = System.Windows.Visibility.Visible;
            await GetComments(pageIndex);
            EnableViewerAndAppBar(true);
        }

        private async void barRefreshIconBtn_Click(object sender, EventArgs e)
        {
            pageIndex = 1;
            progressbar.Visibility = System.Windows.Visibility.Visible;
            commSource.Clear();
            await GetComments(pageIndex);
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
                    if (commentCount != 0)
                        pageIndex += 1;
                        await GetComments(pageIndex);
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
    }
}