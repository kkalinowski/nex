using System;
using System.Globalization;
using lib12.WPF.Converters;

namespace nex.Converters
{
    /// <summary>
    /// Negates the bool
    /// </summary>
    public class DiskSizeConverter : StaticConverter<DiskSizeConverter>
    {
        #region IValueConverter Members
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value > 90;
        }
        #endregion
    }
}