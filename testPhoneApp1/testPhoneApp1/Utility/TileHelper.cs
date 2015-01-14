using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace testPhoneApp1.Utility
{
   public class TileHelper
    {
        private string gFileName = "{0}.jpg";
        private string gRelatviePath = @"\Shared\ShellContent\";
        private string gRelativePath2 = @"isostore:/Shared/ShellContent/";  //isostore:/Shared/ShellContent/
        public static readonly TileHelper _tileHelper = new TileHelper();

        public TileHelper()
        {
            gFileName = gFileName.Replace("{0}", Guid.NewGuid().ToString().Substring(0, 6));            
        }

       public void createTile(string imgPath, string text,string fontfamily)
       {
           var lockBackgroundImage = new Image
           {
               Source = new BitmapImage(new Uri(imgPath, UriKind.RelativeOrAbsolute)),
               Width = 159,
               Height = 159
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

               var bitmap = new WriteableBitmap(159, 159);

               bitmap.Render(lockBackgroundImage, new TranslateTransform());

               bitmap.Render(lockTextBlock, new TranslateTransform()
               {
                   X = 19,
                   Y = 8
               });

               bitmap.Invalidate();
               bitmap.SaveJpeg(stream, 159, 159, 0, 100);
               stream.Close();
           }
           StandardTileData tTileData = new StandardTileData
           {
               BackgroundImage = new Uri(gRelativePath2 + gFileName, UriKind.Absolute),
           };
           ShellTile newTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(gFileName));
           Uri tile = new Uri("/CoupletPage.xaml?" + Guid.NewGuid(), UriKind.RelativeOrAbsolute);
           ShellTile.Create(tile, tTileData);
       }

       public void createTile(string imgPath)
       {
           var lockBackgroundImage = new Image
           {
               Source = new BitmapImage(new Uri(imgPath, UriKind.RelativeOrAbsolute)),
               Width = 336,
               Height = 336
           };

            var lockImage = gRelatviePath + gFileName;
           var isoStoreLockImage = new Uri(gRelativePath2 + gFileName, UriKind.RelativeOrAbsolute);
           using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
           {
               var stream = store.CreateFile(lockImage);

               var bitmap = new WriteableBitmap(336, 336);

               bitmap.Render(lockBackgroundImage, new TranslateTransform());
               bitmap.Invalidate();
               bitmap.SaveJpeg(stream,336,336,0,100);
           }
           StandardTileData tTileData = new StandardTileData
           {
               BackgroundImage = new Uri(gRelativePath2 + gFileName, UriKind.Absolute),
           };
           ShellTile newTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(gFileName));
           Uri tile = new Uri("/CoupletPage.xaml?" + Guid.NewGuid(), UriKind.RelativeOrAbsolute);
           ShellTile.Create(tile, tTileData);
       }
    }
}
