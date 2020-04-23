using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace NielsenPDFv2.Tools
{
    public class BuildStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var statusColor = Brushes.Gray;
            if(value == null)
            {
                return statusColor;
            }
            var status = value.ToString().ToLower();

            if (status.Contains("success"))
            {
                statusColor = Brushes.Green;
            }
            else if (status.Contains("fail"))
            {
                statusColor = Brushes.Red;
            }
            return statusColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //not needed for this
            throw new NotImplementedException();
        }
    }
}
