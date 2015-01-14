using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace LockTextScreen.Utility
{
    public class KeyValueItem
    {
        [XmlAttribute]
        public string Key { get; set; }
        [XmlAttribute]
        public string Value { get; set; }
    }

    public class ThemeColor : KeyValueItem
    {
        [XmlIgnore]
        public Color Color
        {
            get { return ConvertFromString(Value); }
        }

        [XmlIgnore]
        public SolidColorBrush SolidColorBrush
        {
            get { return new SolidColorBrush(Color); }
        }

        private Color ConvertFromString(string argb)
        {
            uint result;
            if (uint.TryParse(argb.TrimStart('#', '0'), NumberStyles.HexNumber, null, out result))
            {
                uint a = argb.Length > 8 ? result >> 24 : 0xFF;
                uint r = result >> 16;
                uint g = (result << 8) >> 16;
                uint b = (result << 16) >> 16;

                return Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
            }
            return Colors.Black;
        }
    }

    public class ImageBrushItem : KeyValueItem
    {
        public Uri ImageUri
        {
            get { return new Uri(Value, UriKind.Relative); }
        }

        public ImageSource ImageSource
        {
            get { return new BitmapImage(new Uri(Value, UriKind.Relative)); }
        }

        public ImageBrush ImageBrush
        {
            get { return new ImageBrush { ImageSource = ImageSource }; }
        }
    }

    public class ThemeItem
    {
        public string Name;

        /// <summary>
        /// 图片
        /// </summary>
        public List<ImageBrushItem> ImageBrushs { get; set; }


        /// <summary>
        /// 颜色
        /// </summary>
        public List<ThemeColor> Colors { get; set; }
    }
}
