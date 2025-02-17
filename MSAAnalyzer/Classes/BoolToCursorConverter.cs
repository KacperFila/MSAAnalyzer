using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using System;

public class BoolToCursorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isMouseOver && isMouseOver)
        {
            return Cursors.Help;
        }

        return Cursors.Arrow;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
