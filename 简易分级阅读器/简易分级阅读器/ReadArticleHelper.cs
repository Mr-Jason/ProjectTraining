using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace 简易分级阅读器
{
    public class ReadArticleHelper
    {
        public const string _htmlTemplateFilePath = "/简易分级阅读器;component/WebBrowserArtivle/html/content_template.html";
        public const string _htmlCssFilePath = "/简易分级阅读器;component/WebBrowserArtivle/html/css/content.css";
        public const string _htmlJs_ContentFilePath = "/简易分级阅读器;component/WebBrowserArtivle/html/js/content.js";
        public const string _htmlJs_JQFilePath = "/简易分级阅读器;component/WebBrowserArtivle/html/js/jquery-1.11.0.js";
        public const string _htmlFilePath = "/Article/index.html";
        public const string _cssFilePath = "/Article/css/content.css";
        public const string _jsFilePath = "/Article/js/content.js";

        /// <summary>
        /// 数据初始化方法
        /// </summary>
       public void initializationData()
        {
            XDocument Articles = getArticleResource();
            var data = from query in Articles.Descendants("Article")
                       select new Article
                       {
                           Title = (string)query.Element("Title").Value,
                           Content = (string)query.Element("Content").Value
                       };

            foreach (Article elem in data)
            {
                //将资源文件写入独立存储中
               IsolatedStorageHelper.SaveFile(elem.Title, "<div class='title'>" + elem.Title + "</div>" + "<div class='content'>"+elem.Content+"</div>");
            }

            //将模板文件写入独立存储中
            IsolatedStorageHelper.CopyContentToIsolatedStorage(_htmlTemplateFilePath, _htmlFilePath);
           //将css文件写入独立存储中
            IsolatedStorageHelper.CopyContentToIsolatedStorage(_htmlCssFilePath, _cssFilePath);
           //将js文件写入独立存储中
            IsolatedStorageHelper.CopyContentToIsolatedStorage(_htmlJs_ContentFilePath, _jsFilePath);
            IsolatedStorageHelper.CopyContentToIsolatedStorage(_htmlJs_JQFilePath, _jsFilePath);
        }

       public static XDocument getArticleResource()
       {
           XDocument Articles = XDocument.Load("简易分级阅读器;component/Resources/Articles.xml");
           return Articles;
       }
    }
}
