using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using testPhoneApp1.Resources;
using System.Windows.Media;
using Windows.Storage;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.IO;

namespace testPhoneApp1
{
    public partial class MainPage : PhoneApplicationPage
    {
        SmallTileTemplate smallTile;
        //@"\Shared\ShellContent\FlipCycleTileMedium.jpg"
        private string gFileName = "{0}.jpg";
        private string gRelatviePath = @"\Shared\ShellContent\";
        private string gRelativePath2 = @"isostore:/Shared/ShellContent/";  //isostore:/Shared/ShellContent/
        private string localPath = @"\Resources\kui.png";
        // 构造函数
        public MainPage()
        {
            InitializeComponent();

            // 用于本地化 ApplicationBar 的示例代码
            //BuildLocalizedApplicationBar();
            Loaded += MainPage_Loaded;
        }

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await BuildFolder();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Uri tile = new Uri("/", UriKind.Relative);
                ShellTile tileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(tile.ToString()));
                IconicTileData tileData = new IconicTileData
                {
                    Title = "锁屏记事",
                    Count = 0,
                    WideContent1 = "We came to explore the moon",
                    WideContent2 = "we discovered the earth...",
                    WideContent3 = "astronaut - Bill Sanders",
                };
                tileToFind.Update(tileData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
        //    smallTile = new SmallTileTemplate();
        ////    smallTile.tbContent.Text = this.tbShowText.Text;
        //    smallTile.Height = 159;
        //    smallTile.Width = 159;
        //    smallTile.Margin = new Thickness(0, -1000, 0, 0);
        //    smallTile.Measure(new Size(159, 159));
        //    smallTile.UpdateLayout();
        //    LayoutRoot.Children.Add(smallTile);
        //    LayoutRoot.UpdateLayout();
        //    gFileName = gFileName.Replace("{0}", Guid.NewGuid().ToString().Substring(0,6));
            createTile("",this.tbShowText.Text);
        }

        public void AddTile(UIElement ue, bool supportsWideTile)
        {
            string tFilePath = string.Format("{0}{1}", gRelatviePath, gFileName);
            WriteableBitmap wb = new WriteableBitmap(ue, null);

            var isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
            if (isolatedStorage.FileExists(tFilePath))
            {
                isolatedStorage.DeleteFile(tFilePath);
            }
            var fileStream = isolatedStorage.CreateFile(tFilePath);
            //將WriteableBitmap寫入至PNG
            //wb.WritePNG(fileStream);
            wb.SaveJpeg(fileStream, 159, 159, 0, 100);
            fileStream.Close();

            StandardTileData tTileData = new StandardTileData
            {
                BackgroundImage = new Uri(gRelativePath2 + gFileName, UriKind.Absolute),
            };
            ShellTile newTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(gFileName));
            Uri tile = new Uri("/MainPage.xaml?"+Guid.NewGuid(), UriKind.RelativeOrAbsolute);
            //newTile.Update(tTileData);
            ShellTile.Create(tile, tTileData);
           
        }

        public async Task<bool> BuildFolder()
        {
            StorageFolder tFolder = null;
            StorageFolder tsTileFolder = null;
            try
            {
                tFolder = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFolderAsync("Shared");
                tFolder = await tFolder.GetFolderAsync("ShellContent");
                tsTileFolder = await tFolder.GetFolderAsync("secondTiles");
            }
            catch (Exception ex)
            { }

            if (tsTileFolder == null)
            {
                try
                {
                    tsTileFolder = await tFolder.CreateFolderAsync("secondTiles");
                }
                catch (Exception ex) { }
            }
            return true;
        }

       

        /// <summary>
        /// 存圖片至Isolated Storage
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="fileName"></param>
        /// <param name="orientation"></param>
        /// <param name="quality"></param>
        public static void SaveImageToIsolatedStorage(WriteableBitmap wb, string fileName, int orientation = 0, int quality = 70)
        {
            var isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();

            if (isolatedStorage.FileExists(fileName))
                isolatedStorage.DeleteFile(fileName);
            var fileStream = isolatedStorage.CreateFile(fileName);
            wb.SaveJpeg(fileStream, wb.PixelWidth, wb.PixelHeight, orientation, quality);
            fileStream.Close();

        }

        /// <summary>
        /// 從IsolatedStorage 中讀取圖片
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static BitmapImage ReadImageFormIsolatedStorage(string fileName)
        {
            var bi = new BitmapImage();
            var isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
            if (!isolatedStorage.FileExists(fileName)) return bi;
            var fs = isolatedStorage.OpenFile(fileName, FileMode.Open, FileAccess.Read);
            bi.SetSource(fs);
            fs.Close();
            return bi;
        }

        public void createTile(string imgPath,string text)
        {
            gFileName = gFileName.Replace("{0}", Guid.NewGuid().ToString().Substring(0, 6));
            var lockBackgroundImage = new Image
            {
                Source = new BitmapImage(new Uri("/Resources/kui.jpg", UriKind.RelativeOrAbsolute)),
                Width = 100,
                Height = 100
            };
            var lockTextBlock = new TextBlock
            {
                Text = text,
                FontSize = 60,
                FontFamily = new FontFamily("/testPhoneApp1;component/Resources/禹卫书法行书简体.ttf#yuweij")
            };
            var lockImage = gRelatviePath + gFileName;
            var isoStoreLockImage = new Uri(gRelativePath2 + gFileName, UriKind.RelativeOrAbsolute);
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var stream = store.CreateFile(lockImage);

                var bitmap = new WriteableBitmap(100, 100);

                bitmap.Render(lockBackgroundImage, new TranslateTransform());

                bitmap.Render(lockTextBlock, new TranslateTransform()
                {
                    X = 19,
                    Y = 8
                });

                bitmap.Invalidate();
                bitmap.SaveJpeg(stream, 100, 100, 0, 100);
                stream.Close();
            }
            StandardTileData tTileData = new StandardTileData
            {
                BackgroundImage = new Uri(gRelativePath2 + gFileName, UriKind.Absolute),
            };
            ShellTile newTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(gFileName));
            Uri tile = new Uri("/MainPage.xaml?" + Guid.NewGuid(), UriKind.RelativeOrAbsolute);
            ShellTile.Create(tile, tTileData);
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