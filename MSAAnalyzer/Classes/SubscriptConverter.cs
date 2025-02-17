using System;
using System.Globalization;
using System.Windows.Data;

public class SubscriptConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue && int.TryParse(stringValue, out int intValue))
        {
            return intValue > 0 ? intValue : "-";
        }

        return "-";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
