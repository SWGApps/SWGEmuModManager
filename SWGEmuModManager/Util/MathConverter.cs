using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection.Metadata;
using System.Windows.Data;
using System.Windows.Forms;

namespace SWGEmuModManager.Util
{
    internal class MathConverter : IValueConverter
    {
        public enum Operation
        {
            Multiplication = 0,
            Division = 1,
            Addition = 2,
            Subtraction = 3
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
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
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
            }

            return value!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
