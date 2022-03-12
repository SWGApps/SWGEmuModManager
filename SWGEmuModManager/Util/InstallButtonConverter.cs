using System;
using System.Globalization;
using System.Windows.Data;
using SWGEmuModManager.Models;

namespace SWGEmuModManager.Util
{
    internal class InstallButtonConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (MainWindowModel.ModIsInstalled((int)values[1])) return "Uninstall";

            // Return original value (Install)
            return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
