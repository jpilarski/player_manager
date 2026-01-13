using System;
using System.Globalization;
using System.Windows.Data;

namespace Pilarski.PlayerManager.UI.Converters
{
    public class ObjectToTypeConverter : IValueConverter
    {
        public static readonly ObjectToTypeConverter Instance = new ObjectToTypeConverter();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.GetType();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
