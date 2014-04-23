using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace 简易分级阅读器.WebBrowserActivle
{
    public class WebBrowserArticle : Control
    {
        private const string _htmlTemplateFilePath = "/html/content_template.html";
        private const string _htmlFilePath = "/html/index.html";

        private WebBrowser _webBrowser;
        public bool ScrollDisabled { get; set; }//是否禁用滚动
        public WebBrowserArticle()
        {
            this.DefaultStyleKey = typeof(WebBrowserArticle);
            this.Loaded += WebBrowserArticle_Loaded;
        }

        #region 属性
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                "Title",
                typeof(string),
                typeof(WebBrowserArticle),
                new PropertyMetadata(null, OnTextPropertyChanged));
        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public static readonly DependencyProperty BodyProperty =
            DependencyProperty.Register(
                "Body",
                typeof(string),
                typeof(WebBrowserArticle),
                new PropertyMetadata(null, OnTextPropertyChanged));
        public string Body
        {
            get
            {
                return (string)GetValue(BodyProperty);
            }
            set
            {
                SetValue(BodyProperty, value);
            }
        }
        #endregion

        void WebBrowserArticle_Loaded(object sender, RoutedEventArgs e)
        {
            if (this._webBrowser != null)
            {
                this.SaveHtml();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this._webBrowser = this.GetTemplateChild("WebBrowser") as WebBrowser;
            this._webBrowser.LoadCompleted += _webBrowser_LoadCompleted;
            this._webBrowser.ScriptNotify += _webBrowser_ScriptNotify;

        }

        /// <summary>
        /// 在LoadCompleted事件中编写调用js初始化方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _webBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            this._webBrowser.InvokeScript("init");//调用js初始化方法
        }

        /// <summary>
        /// js 可以通过window.external.notify(param) 触发这个事件--ScriptNotify
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">参数</param>
        void _webBrowser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            string keyValue = e.Value;
            if (string.IsNullOrWhiteSpace(keyValue)) { return; }
            JObject jObject = JObject.Parse(keyValue);//将参数（字符串类型的）转换为json对象
        }

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WebBrowserArticle source = (WebBrowserArticle)d;
            //string html = (string)e.NewValue;
            source.SaveHtml();
        }

        private void SaveHtml()
        {
            if (this._webBrowser == null || this.Title == null || this.Body == null) { return; }
            string template = IsolatedStorageHelper.OpenFile(_htmlTemplateFilePath);
            template = template.Replace("{title}", this.Title);
            template = template.Replace("{body}", this.Body);

            IsolatedStorageHelper.SaveFile(_htmlFilePath, template);

            this._webBrowser.Navigate(new Uri(_htmlFilePath, UriKind.RelativeOrAbsolute));
        }

        #region 阻止拖动
        private void Border_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (e.FinalVelocities.ExpansionVelocity.X != 0.0 ||
                e.FinalVelocities.ExpansionVelocity.Y != 0.0)
            {
                e.Handled = true;
            }
        }

        private void Border_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (e.DeltaManipulation.Scale.X != 0.0 ||
                e.DeltaManipulation.Scale.Y != 0.0)
            {
                e.Handled = true;
            }
            if (ScrollDisabled)
            {
                if (e.DeltaManipulation.Translation.X != 0.0 ||
                  e.DeltaManipulation.Translation.Y != 0.0)
                {
                    e.Handled = true;
                }
            }
        }
        #endregion
    }
}
