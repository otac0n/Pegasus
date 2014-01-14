namespace Pegasus.Workbench
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Windows.Data;
    using System.Windows.Markup;

    public class PathToFileNameConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                throw new NotSupportedException();
            }

            return Path.GetFileName(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
