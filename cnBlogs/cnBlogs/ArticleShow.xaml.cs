using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using HtmlAgilityPack;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using Alexis.WindowsPhone.Social;
using Coding4Fun.Toolkit.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using System.Xml;
using cnBlogs.Model;

namespace cnBlogs
{
    public partial class ArticleShow : PhoneApplicationPage
    {
        IsolatedStorageSettings aPPSetting = IsolatedStorageSettings.ApplicationSettings;
        Popup _popUp;
        private string articleId = string.Empty;
        private NewsBody newsBody;
        private string blogApp = string.Empty;
        private int commcount = 0;
        Popup activePopup;
        public ArticleShow()
        {
            InitializeComponent();
            appbar = this.ApplicationBar as ApplicationBar;
            writeCommBarBtn = appbar.Buttons[0] as ApplicationBarIconButton;
            commentBarBtn = appbar.Buttons[1] as ApplicationBarIconButton;
            Loaded += ArticleShow_Loaded;
            appbar.IsVisible = false;
        }

        private void ArticleShow_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoginCnBlogs.Width = Application.Current.Host.Content.ActualWidth - 30;
            this.sendComment.Width = Application.Current.Host.Content.ActualWidth - 30;
        }

        private async Task GetBlogsBody()
        {
            string url = until.BLOGBODY + articleId;
            await Task.Run(() =>
            {
                FetchHelper.GetResponseStream(url, html =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        wbReadingPane.IsScriptEnabled = true;
                        using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            if (!file.DirectoryExists("temp"))
                                file.CreateDirectory("temp");
                            using (IsolatedStorageFileStream fs = new IsolatedStorageFileStream("temp\\review.html", FileMode.Create, file))
                            {
                                StringBuilder output = new StringBuilder();
                                using (XmlReader reader = XmlReader.Create(html))
                                {
                                    while (reader.Read())
                                    {
                                        switch (reader.NodeType)
                                        {
                                            case XmlNodeType.Element: // 类似startElement()在Android的SAXParser类中。
                                                break;
                                            case XmlNodeType.Text:  //解析节点内容
                                                output.Append(reader.Value);
                                                break;
                                            case XmlNodeType.XmlDeclaration:
                                            case XmlNodeType.ProcessingInstruction: //解析声明 
                                                break;
                                            case XmlNodeType.Comment:  //解析注释
                                                break;
                                            case XmlNodeType.EndElement: // 类似endElement()在SAXParser类中
                                                break;
                                        }
                                    }
                                }
                                string htmlbody = "<!DOCTYPE html><html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/></head><body>" + output.ToString() + "</body></html>";
                                byte[] bytes = Encoding.UTF8.GetBytes(htmlbody);
                                fs.Write(bytes, 0, bytes.Length);
                            }
                        }
                        appbar.IsVisible = true;
                        this.wbReadingPane.Navigate(new Uri("temp\\review.html", UriKind.Relative));
                        probar.Visibility = Visibility.Collapsed;
                    });
                });
            });
        }

        private async Task GetNewsBody()
        {
            string url = until.NEWSBODY + articleId;
            await Task.Run(() =>
            {
                FetchHelper.HttpGetAsync(url, html =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        wbReadingPane.IsScriptEnabled = true;
                        using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            if (!file.DirectoryExists("temp"))
                                file.CreateDirectory("temp");
                            using (IsolatedStorageFileStream fs = new IsolatedStorageFileStream("temp\\review.html", FileMode.Create, file))
                            {
                                XDocument doc = XDocument.Parse(html);
                                XNamespace d = @"http://www.w3.org/2001/XMLSchema-instance";
                                var newslist = from query in doc.Descendants("NewsBody")
                                               select new NewsBody
                                               {
                                                   Title = (string)query.Element("Title"),
                                                   SourcenName = (string)query.Element("SourceName"),
                                                   SubmitDate = (string)query.Element("SubmitDate"),
                                                   Content = (string)query.Element("Content"),
                                                   ImageUrl = (string)query.Element("ImageUrl"),
                                                   PrevNews = (string)query.Element("PrevNews"),
                                                   NextNews = (string)query.Element("NextNews"),
                                                   CommentCount = (string)query.Element("CommentCount")
                                               };
                                List<NewsBody> list = newslist.ToList<NewsBody>();
                                newsBody = list[0];
                                string htmlbody = "<!DOCTYPE html><html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/></head><body><h3><center>" + list[0].Title + "</center></h3>" + list[0].Content + "</body></html>";
                                byte[] bytes = Encoding.UTF8.GetBytes(htmlbody);
                                fs.Write(bytes, 0, bytes.Length);
                            }
                        }
                        appbar.IsVisible = true;
                        if (newsBody.CommentCount == "0")
                            EnableCommentAppBar(false);
                        commcount =int.Parse(newsBody.CommentCount);
                        commentBarBtn.Text = newsBody.CommentCount + " 评论";

                        this.wbReadingPane.Navigate(new Uri("temp\\review.html", UriKind.Relative));
                        probar.Visibility = Visibility.Collapsed;
                    });
                });
            });

        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                if (e.NavigationMode == NavigationMode.New)
                {
                    if (NavigationContext.QueryString.ContainsKey("ArticelType"))
                    {
                        if (NavigationContext.QueryString["ArticleId"].IndexOf("|") > 0)
                        {
                            articleId = NavigationContext.QueryString["ArticleId"].Split('|')[0];
                            blogApp = NavigationContext.QueryString["ArticleId"].Split('|')[1];
                        }
                        else
                            articleId = NavigationContext.QueryString["ArticleId"];

                        if (NavigationContext.QueryString["ArticelType"].ToUpper() == "NEWS")
                        {
                            newsBody = new NewsBody();
                            await GetNewsBody();
                        }
                        else
                        {
                            await GetBlogsBody();
                            Dispatcher.BeginInvoke(() =>
                            {
                                commcount = int.Parse(NavigationContext.QueryString["CommentCount"]);
                                commentBarBtn.Text = commcount.ToString() + " 评论";
                                if (NavigationContext.QueryString["CommentCount"] == "0")
                                    EnableCommentAppBar(false);
                            });
                        }
                      
                    }
                }
            }
            catch
            {
                MessageBox.Show("非常抱歉，出错了...");
            }
            
        }

        #region WebBrowser乱码解决方法
        /// <summary>
        /// 将字符串编码格式转换为unicode
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        private string ConvertEncode(string strSource)
        {
            MemoryStream mstream = new MemoryStream(Encoding.UTF8.GetBytes(strSource));
            StreamReader reader = new StreamReader(mstream, Encoding.Unicode);
            string strResult = reader.ReadToEnd();
            reader.Close();
            mstream.Dispose();
            reader.Dispose();
            return strResult;
        }

        public static string Unicode4HTML(string html)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in html)
            {
                if (Convert.ToInt32(c) > 127)
                {
                    sb.Append("&#" + Convert.ToInt32(c) + ";");
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
        #endregion

        //private void shareBarBtn_Click(object sender, EventArgs e)
        //{
        //    SnapShot();
        //    ShowShareControl();
        //}

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (_popUp != null && _popUp.IsOpen)
            {
                _popUp.IsOpen = false;
                e.Cancel = true;
                return;
            }
            if (activePopup!=null)
            {
                e.Cancel = true;
                activePopup.IsOpen = false;
                EnableViewerAndAppBar(true);
                return;
            }
            base.OnBackKeyPress(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (_popUp != null && _popUp.IsOpen)
            {
                _popUp.IsOpen = false;
            }
            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// 微博截图
        /// </summary>
        public static void SnapShot()
        {
            try
            {
                WriteableBitmap bitmap = new WriteableBitmap(480, 800);
                bitmap.Render(App.Current.RootVisual, null);
                bitmap.Invalidate();

                SocialSDk.ShareImage = bitmap;
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (var stream = store.OpenFile(Constants.SHARE_IMAGE, System.IO.FileMode.OpenOrCreate))
                    {
                        try
                        {
                            bitmap.SaveJpeg(stream, 480, 800, 0, 100);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        /// <summary>
        /// popup menu let user to choose
        /// </summary>
        private void ShowShareControl()
        {

            _popUp = new Popup
            {
                Height = 800,
                Width = 480,
            };


            ShareControl sc = new ShareControl();
            sc.Height = 800;
            sc.Width = 480;
            sc.TypeSelected = (p) =>
            {
                DoShare(p);
            };

            _popUp.Child = sc;
            _popUp.IsOpen = true;
        }

        /// <summary>
        /// do share matters
        /// </summary>
        /// <param name="type"></param>
        private void DoShare(SocialType type)
        {
            SocialSDk.CurrentSocialType = type;
            SocialSDk.Statues = "share text only for test usage";
            bool isLogin = true;
            switch (type)
            {
                case SocialType.Weibo:
                    if (!(SocialAPI.WeiboAccessToken == null || SocialAPI.WeiboAccessToken.IsExpired))
                    {
                        isLogin = false;
                    }
                    break;
                case SocialType.Tencent:
                    if (!(SocialAPI.TencentAccessToken == null || SocialAPI.TencentAccessToken.IsExpired))
                    {
                        isLogin = false;
                    }
                    break;
                case SocialType.Renren:
                    if (!(SocialAPI.RenrenAccessToken == null || SocialAPI.RenrenAccessToken.IsExpired))
                    {
                        isLogin = false;
                    }
                    break;
                case SocialType.Douban:
                    break;
                case SocialType.Net:
                    break;
                case SocialType.Sohu:
                    break;
                default:
                    break;
            }
            if (isLogin)
            {
                NavigationService.Navigate(new Uri("/SocialLoginPage.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/SocialSendPage.xaml", UriKind.Relative));
            }
        }

        private void commentBarBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(articleId))
            {
                NavigationService.Navigate(new Uri("/CommentPage.xaml?PostId=" + articleId + "&blogApp=" + blogApp + "&ArticleType=" + NavigationContext.QueryString["ArticelType"] + "&commCount=" + commcount.ToString(), UriKind.Relative));
            }
        }

        //private void BarBtn_Click(object sender, EventArgs e)
        //{

        //}

        private void writeCommBarBtn_Click(object sender, EventArgs e)
        {
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
                sendComment.postId = articleId;
                activePopup = SendCommentPopup;
                EnableViewerAndAppBar(false);
            }
        }

        void EnableCommentAppBar(bool isEnabled)
        {
            commentBarBtn.IsEnabled = isEnabled;
        }

        void EnableViewerAndAppBar(bool isEnabled)
        {
            writeCommBarBtn.IsEnabled = isEnabled;
        }

        private void SendComment_DialogDismissed(object sender, CancelEventArgs e)
        {
            SendCommentPopup.IsOpen = false;
            activePopup = null;
            if (sendComment.isSend)
            {
                commcount += 1;
                EnableCommentAppBar(true);
                commentBarBtn.Text = commcount.ToString() + " 评论";
            }
            EnableViewerAndAppBar(true);
        }

        private void cnBlogsLogin_DialogDismissed(object sender, CancelEventArgs e)
        {
            AccountLoginPopup.IsOpen = false;
            activePopup = null;
            if (aPPSetting.Count != 0 && (bool)aPPSetting["islogin"])
            {
                sendComment.blogApp = blogApp;
                sendComment.postId = articleId;
                activePopup = SendCommentPopup;
                EnableViewerAndAppBar(false);
                SendCommentPopup.IsOpen = true;
            }
            else
            {
                EnableViewerAndAppBar(true);
            }
        }
    }
}