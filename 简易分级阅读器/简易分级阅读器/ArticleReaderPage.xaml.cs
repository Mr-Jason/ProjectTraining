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
            _loadData = XDocument.Load(@"Resources\Articles.xml");
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
            var data = from query in _loadData.Descendants("Article")
                       where (string)query.Element("Title") == _articleTitle
                       select new Article
                       {
                           Title = (string)query.Element("Title"),
                           Content = (string)query.Element("Content")
                       };
            Article article = (Article)data.ToList()[0];

            //this.tbArticleTitle.Text = article.Title;
            //this.tbArticleContent.Text = article.Content;
        }

        private void ArticleContentWB_LoadCompleted(object sender, NavigationEventArgs e)
        {

        }

        private void ArticleContentWB_ScriptNotify(object sender, NotifyEventArgs e)
        {

        }
    }
}