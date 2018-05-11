using System;
using System.Windows.Data;
using System.Windows.Media;

namespace TOEC_Dist
{
    public class StringConverterColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush color = new SolidColorBrush(Color.FromArgb(255, 158, 150, 110));
            string z = (string)value;
            if (z.Equals("成功"))
            {
                color = new SolidColorBrush(Color.FromArgb(255, 106, 172, 106));
            }
            if (z.Equals("异常") || z.Equals("失败"))
            {
                color = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            }
            return color;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
