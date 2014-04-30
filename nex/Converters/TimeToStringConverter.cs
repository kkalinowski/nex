using System;
using System.Globalization;
using lib12.WPF.Converters;

namespace nex.Converters
{
    class TimeToStringConverter : StaticConverter<TimeToStringConverter>
    {
        #region IValueConverter Members
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var time = (TimeSpan)value;
            if (time.TotalSeconds == 0)
                return "Ukończono";
            else if (time.TotalSeconds < 1)
                return "Poniżej sekundy";
            else if (time.TotalSeconds < 5)
                return "Kilka sekund";
            else
                return string.Format("{0:00}:{1:00}:{2:00}", time.Hours, time.Minutes, time.Seconds);
        }
        #endregion
    }
}