using System;
using System.Globalization;
using System.Windows.Shell;
using lib12.WPF.Converters;

namespace nex.Converters
{
    public sealed class TaskBarProgressStateConverter : StaticMultiConverter<TaskBarProgressStateConverter>
    {
        #region IValueConverter Members
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)values[0])
                return TaskbarItemProgressState.Error;
            else if ((bool)values[1])
                return TaskbarItemProgressState.Normal;
            else
                return TaskbarItemProgressState.None;
        }
        #endregion
    }
}