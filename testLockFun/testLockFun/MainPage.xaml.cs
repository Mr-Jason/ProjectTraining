using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using testLockFun.Resources;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Windows.Phone.System.UserProfile;

namespace testLockFun
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 构造函数
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
            // 用于本地化 ApplicationBar 的示例代码
            //BuildLocalizedApplicationBar();
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {
            createButtonToScreen();
            var toast = new ShellToast
            {
                Title = "testLockFunb",
                Content = "The lock screen was updated...",
                NavigationUri = new Uri("/MainPage.xaml?agentLockscreen=1", UriKind.RelativeOrAbsolute)
            };

            toast.Show();
        }

        private async void createButtonToScreen()
        {
            var lockBackgroundImage = new Image
            {
                Source = new BitmapImage(new Uri("/Assets/Lock/Background.jpg", UriKind.RelativeOrAbsolute)),
                Width = 480,
                Height = 800
            };

            //var lockWeatherImage = new Image
            //{
            //    Source = new BitmapImage(new Uri("/Assets/Lock/CloudSun.png", UriKind.RelativeOrAbsolute)),
            //    Width = 88,
            //    Height = 88
            //};

            //var lockTextBlock = new TextBlock
            //{
            //    Text = "san francisco" + Environment.NewLine + "72 degrees" + Environment.NewLine +
            //           "partially sunny",
            //    FontSize = 24,
            //    Foreground = new SolidColorBrush(Colors.Orange),
            //    FontFamily = new FontFamily("Segoe WP SemiLight")
            //};

            var button = new Button
            {
                Content="点击我",
                Foreground = new SolidColorBrush(Colors.Orange),
                FontSize = 35,
                Width=100,
                Height=40,
               ClickMode=ClickMode.Hover,
              //  Background = new SolidColorBrush(Colors.Red),
                FontFamily = new FontFamily("Segoe WP SemiLight")
            };
            button.Click += button_Click;

            // The LockScreen.SetImageUri requires that the Uri of the new image is different than the current one.
            // Determine the name to use, doing an A-B toggle to have always a maximum 
            // of 2 images (current and previous), and no need to implement a cache purging mechanism.
            string fileName;
            Uri currentImage;

            try
            {
                currentImage = LockScreen.GetImageUri();
            }
            catch (Exception)
            {
                currentImage = new Uri("ms-appdata:///local/LiveLockBackground_A.jpg", UriKind.Absolute);
            }

            if (currentImage.ToString().EndsWith("_A.jpg"))
            {
                fileName = "LiveLockBackground_B.jpg";
            }
            else
            {
                fileName = "LiveLockBackground_A.jpg";
            }

            var lockImage = string.Format("{0}", fileName);
            var isoStoreLockImage = new Uri(string.Format("ms-appdata:///local/{0}", fileName), UriKind.Absolute);

            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var stream = store.CreateFile(lockImage);

                var bitmap = new WriteableBitmap(480,800);
               
                bitmap.Render(lockBackgroundImage, new TranslateTransform());
                
                bitmap.Render(this.lockbtn, new TranslateTransform()
                {
                    X = 24,
                    Y = 118
                });

                //bitmap.Render(lockTextBlock, new TranslateTransform()
                //{
                //    X = 24,
                //    Y = 118
                //});

                bitmap.Invalidate();
                bitmap.SaveJpeg(stream, 480, 800, 0, 100);

                stream.Close();

            }


            await LockScreenManager.RequestAccessAsync();

            bool isProvider = LockScreenManager.IsProvidedByCurrentApplication;
            if (isProvider)
            {
                LockScreen.SetImageUri(isoStoreLockImage);
          
                System.Diagnostics.Debug.WriteLine("New current image set to {0}", isoStoreLockImage);
            }
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            //var toast = new ShellToast
            //{
            //    Title = "testLockFunb",
            //    Content = "Click Button",
            //    NavigationUri = new Uri("/MainPage.xaml?agentLockscreen=1", UriKind.RelativeOrAbsolute)
            //};
            MessageBox.Show("点击了");
            //toast.Show();
        }

        private void lockbtn_Click(object sender, RoutedEventArgs e)
        {
            var toast = new ShellToast
            {
                Title = "Lockbtn",
                Content = "Click Button",
                NavigationUri = new Uri("/MainPage.xaml?agentLockscreen=1", UriKind.RelativeOrAbsolute)
            };

            toast.Show();
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