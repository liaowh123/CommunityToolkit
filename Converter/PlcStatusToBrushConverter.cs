using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace CommunityToolkit示例.Converter
{
    public class PlcStatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 假设“连接成功”为"连接成功"字符串
            if (value is string status && status == "连接成功")
                return new SolidColorBrush(Color.FromRgb(46, 204, 113)); // 绿色 #FF2ECC71
            return new SolidColorBrush(Color.FromRgb(231, 76, 60)); // 红色 #FFE74C3C
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
