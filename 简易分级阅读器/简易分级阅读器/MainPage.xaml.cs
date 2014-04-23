using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace 简易分级阅读器
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 构造函数
        public MainPage()
        {
            InitializeComponent();
            //加载文章列表
            this.DataBind();
        }

        //private void lbArticlesList_Tap(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        //{
        //    Article selectedItem = (Article)lbArticlesList.SelectedItem;
        //    if (selectedItem != null)
        //    {
        //        NavigationService.Navigate(new Uri("/ArticleReaderPage.xaml?Title=" + selectedItem.Title, UriKind.Relative));
        //    }
        //}


        private void DataBind()
        {
            XDocument loadData = XDocument.Load(@"Resources\Articles.xml");

            var data = from query in loadData.Descendants("Article")

                       select new Article
                       {
                           Title = (string)query.Element("Title")
                       };

            this.lbArticlesList.ItemsSource = data;
        }

        private void lbArticlesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Article selectedItem = (Article)lbArticlesList.SelectedItem;
            if (selectedItem != null)
            {
                NavigationService.Navigate(new Uri("/ArticleReaderPage.xaml?Title=" + selectedItem.Title, UriKind.Relative));
            }
        }
    }
}