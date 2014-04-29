using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace 简易分级阅读器
{
    /// <summary>
    /// Double 类型数据转化为整型
    /// </summary>
    public class TruncationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,object parameter, CultureInfo culture)
        {
            if (value is double)
            {
                return Math.Round((double)value);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType,object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
