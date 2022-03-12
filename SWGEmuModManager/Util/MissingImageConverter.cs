using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace SWGEmuModManager.Util;

internal class MissingImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (string.IsNullOrWhiteSpace((string)value))
        {
            return "/no-image-available.png";
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
