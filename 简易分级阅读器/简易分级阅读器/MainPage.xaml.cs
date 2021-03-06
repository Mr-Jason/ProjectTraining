﻿using System;
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
using System.IO;
using System.Threading;

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
            //异步加载单词等级表
            this.loadLocalDatabase();
        }

        /// <summary>
        /// 加载文章列表
        /// </summary>
        private void DataBind()
        {
            #region 写法1
            //Stream stream = Application.GetResourceStream(new Uri("简易分级阅读器;component/Resources/Articles.xml", UriKind.Relative)).Stream;
            //XDocument xdoc = XDocument.Load(stream);
            #endregion

            #region 写法2
            //Uri uri = new Uri("Resources/Articles.xml", UriKind.RelativeOrAbsolute);
            //StreamResourceInfo sri = Application.GetResourceStream(uri);
            //Stream stream = sri.Stream;
            //XDocument xdoc = XDocument.Load(stream);
            #endregion

            XDocument loadData = ReadArticleHelper.getArticleResource();

             var data = from query in loadData.Descendants("Article")

                       select new Article
                       {
                           Title = (string)query.Element("Title")
                       };

            this.lbArticlesList.ItemsSource = data;
        }

        private void lbArticlesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = this.lbArticlesList.SelectedIndex;
            if (selectedIndex == -1)
            {
                return;
            }
            
            Article selectedItem = (Article)lbArticlesList.SelectedItem;
            if (selectedItem != null)
            {
                string strTitle = selectedItem.Title.ToString().TrimStart().TrimEnd();
                NavigationService.Navigate(new Uri("/ArticleReaderPage.xaml?Title=" + strTitle, UriKind.Relative));
                this.lbArticlesList.SelectedIndex = -1;
            }
        }

        private void loadLocalDatabase()
        {
            Thread threa = new Thread(new localDataBaseHelper().InitializeData);
            threa.Start();
        }
    }
}