using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Windows.Phone.System.UserProfile;
using Windows.Storage;
using ToolStackCRCLib;
using ToolStackPNGWriterLib;
using System.Windows.Media.Imaging;

namespace LockTextScreen.Utility
{
   public class TileHelper
    {
        //@"\Shared\ShellContent\FlipCycleTileMedium.jpg"
        private string gFileName = "DyncTile.png";
        private string gRelatviePath = @"\Shared\ShellContent\secondTiles\";
        private string gRelativePath2 = @"isostore:/Shared/ShellContent/secondTiles/";

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

       //根据内容的长度生成不同大小的磁贴
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
           wb.WritePNG(fileStream);
           fileStream.Close();

           ShellTile newTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(gFileName));
           FlipTileData tFlipTileData;
           StandardTileData tSTileData;
           if (supportsWideTile)
           {
               tFlipTileData = new FlipTileData
               {
                   BackgroundImage = new Uri(gRelativePath2 + gFileName, UriKind.Absolute),
                   WideBackgroundImage = new Uri(gRelativePath2 + gFileName, UriKind.Absolute)
               };
               if (newTile == null)
               {
                   ShellTile.Create(new Uri("/LockScreenPage.xaml?" + gFileName, UriKind.Relative), tFlipTileData, supportsWideTile);
               }
               else
               {
                   newTile.Delete();
                   ShellTile.Create(new Uri("/LockScreenPage.xaml?" + gFileName, UriKind.Relative), tFlipTileData, supportsWideTile);
               }
           }
           else
           {
               tSTileData = new StandardTileData
               {
                   BackgroundImage = new Uri(gRelativePath2 + gFileName, UriKind.Absolute)
               };
               if (newTile == null)
               {
                   ShellTile.Create(new Uri("/LockScreenPage.xaml?" + gFileName, UriKind.Relative), tSTileData, supportsWideTile);
               }
               else
               {
                   newTile.Delete();
                   ShellTile.Create(new Uri("/LockScreenPage.xaml?" + gFileName, UriKind.Relative), tSTileData, supportsWideTile);
               }
           }
       }

       public void SetLockScreenText(string lockText)
       {
           string temp = string.Empty;
           try
           {
               IconicTileData tileData = new IconicTileData();
               if (lockText.Length > 17)
               {
                   tileData.WideContent1 = lockText.Substring(0, 17);
                   temp = lockText.Substring(17, lockText.Length - 17);
                   if (temp.Length > 17)
                   {
                       tileData.WideContent2 = lockText.Substring(17, 17);
                       tileData.WideContent3 = lockText.Substring(34, lockText.Length - 34);
                   }
                   else
                   {
                       tileData.WideContent2 = temp;
                   }
               }
               else
               {
                   tileData.WideContent1 = lockText;
               }

               Uri tile = new Uri("/", UriKind.Relative);
               ShellTile tileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(tile.ToString()));
               tileToFind.Update(tileData);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

       public void DeleteLockScreenText()
       {
           Uri tile = new Uri("/", UriKind.Relative);
           ShellTile tileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(tile.ToString()));
           IconicTileData tileData = new IconicTileData
           {
               WideContent1 = "",
               WideContent2 = "",
               WideContent3 = "",
           };
           tileToFind.Update(tileData);
       }
    }
}
