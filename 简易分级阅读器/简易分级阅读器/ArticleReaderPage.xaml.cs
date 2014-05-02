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
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Microsoft.Phone.Shell;

namespace 简易分级阅读器
{
    public partial class ArticleReaderPage : PhoneApplicationPage
    {
        private string _articleTitle;
        private XDocument _loadData;
        private Article _article;
        private string _articleContent;

        public ArticleReaderPage()
        {
            InitializeComponent();
            this.FontSizeSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(FontSizeSlider_ValueChanged);//FontSizeSlider_ValueChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
              _articleTitle = NavigationContext.QueryString["Title"];
            //加载文档
            _loadData = ReadArticleHelper.getArticleResource();
            var data = from query in _loadData.Descendants("Article")
                       where (string)query.Element("Title") == _articleTitle
                       select new Article
                       {
                           Translation = (string)query.Element("Translation"),
                           NewWord = (string)query.Element("NewWord")
                       };
            _article = (Article)data.ToList()[0];
        }

        /// <summary>
        /// 全屏
        /// </summary>
        void FullScreenAction()
        {
            if (this.ApplicationBar.IsVisible == false)
                return;
            this.ShowArticlePivot.Margin = new Thickness(0, -10, 0, 0);
            TitleDispear.Begin();  
            this.ApplicationBar.IsVisible = false;
            this.ShowArticlePivot.IsLocked = true;
            this.BackKeyPress += OnFullScreenBackKeyPress;
        }

        /// <summary>
        /// 取消全屏
        /// </summary>
        void StopFullScreenAction()
        {
            if (this.ApplicationBar.IsVisible == true)
                return;
            TitleAppear.Begin();  
            this.ApplicationBar.IsVisible = true;
            this.ShowArticlePivot.Margin = new Thickness(0);
            this.ShowArticlePivot.IsLocked = false;
            this.BackKeyPress -= OnFullScreenBackKeyPress;
        }

        /// <summary>
        /// 返回键退出全屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFullScreenBackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopFullScreenAction();
            this.WordLeavelDialopPopup.IsOpen = false;
            this.FontSizeDialogPopup.IsOpen = false;
            e.Cancel = true;
        }

        /// <summary>
        /// 判断手势是否进入全屏状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GestureListener_Flick(object sender, FlickGestureEventArgs e)
        {
            if (e.Direction == System.Windows.Controls.Orientation.Horizontal) //判断手势方向  
                return;
            if (e.VerticalVelocity < 0) //垂直速度判断  
            {
                //全屏展示
                if (!this.FontSizeDialogPopup.IsOpen)
                    this.WordLeavelDialopPopup.IsOpen = true;
                FullScreenAction();
            }
            else
            {
                //调用取消全屏方法
               // StopFullScreenAction();
            }
        }

        /// <summary>
        /// 第一次预览时加载数据
        /// </summary>
        private void readDataToTemplate()
        {
              // 程序初始化处理    
            new ReadArticleHelper().initializationData();// initializationData();
            //从独立存储中读取资源文件
            string content = IsolatedStorageHelper.OpenFile(_articleTitle);
            //从独立存储中读取模板文件
            string template = IsolatedStorageHelper.OpenFile(ReadArticleHelper._htmlFilePath);//读取模板文件
            template = template.Replace("{body}", content);//替换模板内容
<<<<<<< HEAD

=======
            _articleContent = template;
>>>>>>> 9e0b29ce3f281fdb84c684209401c281f1a7dd49
            IsolatedStorageHelper.SaveFile(ReadArticleHelper._htmlFilePath, template);
            //这样加载数据
            this.ArticleContentWB.Navigate(new Uri(ReadArticleHelper._htmlFilePath, UriKind.Relative));
           //也可以这样 this.ArticleContentWB.NavigateToString(template);
        }

        private void TackOverCrossSlip_Flick(object sender, FlickGestureEventArgs e)
        {
            if (this.ShowArticlePivot.IsLocked)
                return;
            if (e.Direction == System.Windows.Controls.Orientation.Horizontal)
            {
                this.ShowArticlePivot.SelectedIndex = 1;
            }
        }

        #region Pivot Event
        private void ShowArticlePivot_LoadingPivotItem(object sender, PivotItemEventArgs e)
        {
            //防止因为Pivot切换时重复加载数据
            if (this.ArticleContentWB.Source == null)
                readDataToTemplate();
        }

        private void ShowArticlePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int itemIndex = 0;
            itemIndex = ((Microsoft.Phone.Controls.Pivot)(sender)).SelectedIndex;

            if (itemIndex == 0)
            {
                this.ApplicationBar.IsVisible = true;
                return;
            }
            this.ApplicationBar.IsVisible = false;

            if (itemIndex == 1)
            {
                this.tbTranslation.Text = this._article.Translation;
            }
            if (itemIndex == 2)
            {
                this.tbNewWords.Text = this._article.NewWord;
            }
        }
        #endregion

        #region WebBrowser Event
        /// <summary>
        /// 在LoadCompleted事件中编写调用js初始化方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArticleContentWB_LoadCompleted(object sender, NavigationEventArgs e)
        {
            //防止WebBrowser控件加载过程中的闪烁
            this.ArticleContentWB.Opacity = 1;
            //调用js方法（在这里是调用js初始化方法）
            // this.ArticleContentWB.InvokeScript("Js_Method_Name");
        }

        /// <summary>
        /// 在ScriptNotify事件中可监听javascript，脚本中通过window.external.notify触发ScriptNotify事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArticleContentWB_ScriptNotify(object sender, NotifyEventArgs e)
        {
            //页面传递的参数
            string param = e.Value;
            //判断参数是否为空
            if (string.IsNullOrWhiteSpace(param))
                return;
            //将字符串转化为json对象
            JObject jObject = JObject.Parse(param);

        }

        //private void ArticleContentWB_Loaded(object sender, RoutedEventArgs e)
        //{
        //    var border = this.ArticleContentWB.Descendants<Border>().Last() as Border;
        //   // border.ManipulationDelta += Border_ManipulationDelta;
        //   // border.ManipulationCompleted += Border_ManipulationCompleted;
        //}
        #endregion

        #region ApplicationBar Button Event
        private void barIconBtnWordLight_Click(object sender, EventArgs e)
        {
            try
            {
                ApplicationBarIconButton btnLight = sender as ApplicationBarIconButton;
                if (btnLight.IconUri.ToString().IndexOf("light") > 0)
                {
                    btnLight.IconUri = new Uri("Assets/images/appbar.dark.rest.png", UriKind.Relative);
                }
                else
                {
                    btnLight.IconUri = new Uri("Assets/images/appbar.light.rest.png", UriKind.Relative);
                }
                this.ArticleContentWB.InvokeScript("highLightWord", new string[]{this._article.NewWord});
            }
            catch (Exception se)
            {
                MessageBox.Show("Excute JavaScript Have Exception:" + se.Message);
            }
        }

        private void barIconBtnFontSize_Click(object sender, EventArgs e)
        {
            FontSizeDialogPopup.IsOpen = true;
            FullScreenAction();
        }
        #endregion


        private void Levelslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            object levelValue = Math.Round(e.NewValue);
            try
            {
                using (localDataBaseHelper db = new localDataBaseHelper())
                {
                    var result = from query in db.WordLevel
                                 where query.Level <= Convert.ToInt32(levelValue)
                                 select query.Word;
                                 
                    var wordList = new List<string>();
                    for (int i = 0; i < result.ToArray().Length; i++)
                    {
<<<<<<< HEAD
                        wordList.Add(result.ToArray()[i] + "|");
                    }

                   this.ArticleContentWB.InvokeScript("highLightLevelWord", wordList.ToArray());
=======
                        wordList.Add(result.ToArray()[i]);
                    }
                    
                    var pattern = string.Empty;
    	            for (int i = 0; i < wordList.Count; i++)
    		    {
    			var value = wordList[i];
    			pattern = @"\b" + value + @"\b";
			_articleContent = Regex.Replace(_articleContent, pattern, "<span class='highlight'>" + value + "</span>", RegexOptions.IgnoreCase);
    		    }
    				
                   this.ArticleContentWB.NavigateToString(_articleContent);
>>>>>>> 9e0b29ce3f281fdb84c684209401c281f1a7dd49
                }
            }
            catch (Exception se)
            {
                MessageBox.Show("Excute JavaScript Have Exception:" + se.Message);
            }
        }

        private void FontSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            object fontSize = Math.Round(e.NewValue);
            string[] str = {fontSize.ToString()};
            try
            {
                this.ArticleContentWB.InvokeScript("changeFontSize", str);
            }
            catch (Exception se)
            {
                MessageBox.Show("Excute JavaScript Have Exception:" + se.Message);
            }
        }

        private void ArticleContentWB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string selectedWord = (string)this.ArticleContentWB.InvokeScript("eval", new string[]
			{
				"document.selection.createRange().text;"
			});
            if (!string.IsNullOrEmpty(selectedWord))
            {

            }

        }
    }
}
