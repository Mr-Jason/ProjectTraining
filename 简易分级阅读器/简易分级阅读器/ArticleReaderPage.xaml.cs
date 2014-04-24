using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using System.Xml.Linq;

namespace 简易分级阅读器
{
    public partial class ArticleReaderPage : PhoneApplicationPage
    {
        private string _articleTitle;
        private XDocument _loadData;

        public ArticleReaderPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _articleTitle = NavigationContext.QueryString["Title"];
            _loadData = XDocument.Load("简易分级阅读器;component/Resources/Articles.xml");
           
        }

        private void barIconBtnWordLight_Click(object sender, EventArgs e)
        {

        }

        private void barIconBtnFontSize_Click(object sender, EventArgs e)
        {

        }

        void FullScreenAction()
        {
            if (this.ApplicationBar.IsVisible == false)
                return;
            this.ShowArticlePivot.Margin = new Thickness(0, -10, 0, 0);
            //this.contentScrollView.Height = System.Windows.Application.Current.Host.Content.ActualHeight-300;
            //this.tbArticleTitle.Margin = new Thickness(0, 12, 0, 12);
            TitleDispear.Begin();  
            this.ApplicationBar.IsVisible = false;
            this.ShowArticlePivot.IsLocked = true;
            this.BackKeyPress += OnFullScreenBackKeyPress;
        }

        void StopFullScreenAction()
        {
            if (this.ApplicationBar.IsVisible == true)
                return;
            TitleAppear.Begin();  
            this.ApplicationBar.IsVisible = true;
            this.ShowArticlePivot.Margin = new Thickness(0);
            //this.tbArticleTitle.Margin = new Thickness(0);
            //this.contentScrollView.Height = System.Windows.Application.Current.Host.Content.ActualHeight - 350;
            this.ShowArticlePivot.IsLocked = false;
            this.BackKeyPress -= OnFullScreenBackKeyPress;
        }

        private void OnFullScreenBackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopFullScreenAction();
            e.Cancel = true;
        }

        private void GestureListener_Flick(object sender, FlickGestureEventArgs e)
        {
            if (e.Direction == System.Windows.Controls.Orientation.Horizontal) //判断手势方向  
                return;
            if (e.VerticalVelocity < 0) //垂直速度判断  
            {
                FullScreenAction();
            }
            else
            {
                StopFullScreenAction();
            }  
        }

        private void ShowArticlePivot_LoadingPivotItem(object sender, PivotItemEventArgs e)
        {
            readDataToTemplate();
        }

        private void ArticleContentWB_LoadCompleted(object sender, NavigationEventArgs e)
        {
            //防止WebBrowser控件加载过程中的闪烁
            this.ArticleContentWB.Opacity = 1;
        }

        private void ArticleContentWB_ScriptNotify(object sender, NotifyEventArgs e)
        {

        }

        private void readDataToTemplate()
        {
            new ReadArticleHelper().initializationData();// initializationData();
            //从独立存储中读取资源文件
            string content = IsolatedStorageHelper.OpenFile(_articleTitle);
            //从独立存储中读取模板文件
            string template = IsolatedStorageHelper.OpenFile(ReadArticleHelper._htmlFilePath);//读取模板文件
            template = template.Replace("{body}", content);//替换模板内容
            IsolatedStorageHelper.SaveFile(ReadArticleHelper._htmlFilePath, template);
            this.ArticleContentWB.Navigate(new Uri(ReadArticleHelper._htmlFilePath, UriKind.RelativeOrAbsolute));
           // this.ArticleContentWB.NavigateToString(template);
        }

        private void TackOverCrossSlip_Flick(object sender, FlickGestureEventArgs e)
        {
            //if (this.ShowArticlePivot.IsLocked)
            //    return;
            //if (e.Direction == System.Windows.Controls.Orientation.Vertical)
            //{
            //    this.ShowArticlePivot.SelectedIndex = 1;
            //}
        }

        private void ShowArticlePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}