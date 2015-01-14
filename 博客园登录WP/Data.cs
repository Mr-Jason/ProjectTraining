using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace 博客园登录
{
    public class Data:ModelBase
    {
        public Page page { get; set; }      

        static BitmapImage bm = new BitmapImage();
        private ImageSource _img = bm;
        public ImageSource img
        {
            get { return _img; }
            set { this.SetProperty(ref _img, value); }
        }

        public Data()
        {

        }

        string x = null;

        public async Task GetImage()
        {
            this.x = await HttpService.Current.LoginPageGet();
            MemoryStream ImageStream = null;
            ImageStream = await HttpService.Current.DownloadImage(x);
            //ImageStream = await HttpService.Current.DownloadImageUseCookie();
            if (ImageStream != null)
            {
                await page.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    bm.SetSource(ImageStream.AsRandomAccessStream());
                });

            } 
            
        }

        public async void Login(string UserName,string UserPassword,string checkCode)
        {
            string x1 = await HttpService.Current.LoginBlog(UserName,UserPassword,checkCode, x);
            await new MessageDialog(x1).ShowAsync();
        }

        public async void GetMessage()
        {
            string x2 = await HttpService.Current.MsgGet();
            await new MessageDialog(x2).ShowAsync();
        }
    }
}
