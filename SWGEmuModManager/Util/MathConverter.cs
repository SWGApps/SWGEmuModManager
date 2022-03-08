using System;
using System.Globalization;
using System.Windows.Data;

namespace SWGEmuModManager.Util
{
    internal class MathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not null && parameter is not null)
            {
                string param = (string) parameter;

                if (param.Contains('/'))
                    return (System.Convert.ToDouble(value) / System.Convert.ToDouble(param.Split('/')[1])).ToString(culture);
                if (param.Contains('*'))
                    return (System.Convert.ToDouble(value) * System.Convert.ToDouble(param.Split('*')[1])).ToString(culture);
                if (param.Contains('+'))
                    return (System.Convert.ToDouble(value) + System.Convert.ToDouble(param.Split('+')[1])).ToString(culture);
                if (param.Contains('-'))
                    return (System.Convert.ToDouble(value) - System.Convert.ToDouble(param.Split('-')[1])).ToString(culture);
            }

            return value!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
