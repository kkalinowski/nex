using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using lib12.WPF.Converters;
using lib12.WPF.Extensions;

namespace nex.Converters
{
    internal class ActiveColorConverter : StaticConverter<ActiveColorConverter>
    {
        private Brush activeBrush;
        public Brush ActiveBrush
        {
            get
            {
                return activeBrush ?? (activeBrush = Application.Current.FindBrush("ActiveColor"));
            }
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? ActiveBrush : Brushes.Transparent;
        }
    }
}
