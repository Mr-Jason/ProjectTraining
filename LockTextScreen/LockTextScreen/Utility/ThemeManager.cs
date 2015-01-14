using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LockTextScreen.Utility
{
    /// <summary>
    /// 主题管理器
    /// </summary>
    public class ThemeManager
    {
        /// <summary>
        /// 加载主题资源
        /// </summary>
        /// <param name="path">例如：/Assets/ThemeResources/DayResource.xaml</param>
        public static void Load(string path)
        {
            var resourceDictionary = new ResourceDictionary();

            //从程序集读取资源（这里的Uri格式：/解决方案;component/资源文件路径）
            Application.LoadComponent(resourceDictionary,
                new Uri(string.Format("/ThemeDemo;component{0}", path), UriKind.Relative));

            //应用样式（只有颜色和Color和图片BitmapImage）
            foreach (DictionaryEntry kv in resourceDictionary)
            {
                if (kv.Value is Color)
                {
                    ((SolidColorBrush)Application.Current.Resources[kv.Key]).Color = (Color)kv.Value;
                }
                else if (kv.Value is BitmapImage)
                {
                    if (Application.Current.Resources.Contains(kv.Key))
                    {
                        ((ImageBrush)Application.Current.Resources[kv.Key]).ImageSource = ((BitmapImage)kv.Value);
                    }
                    else
                    {
                        Application.Current.Resources.Add(kv.Key, new ImageBrush { ImageSource = (BitmapImage)kv.Value });
                    }
                }
            }
        }
    }
}
