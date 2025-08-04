using System.Globalization;
using System.Windows.Data;

namespace PhotoTools.Application.Converters
{
    /// <summary>
    /// 将一个列表转为其长度
    /// </summary>
    public class CollectionLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICollection<object> collection)
            {
                return collection.Count;
            }
            else if (value is IEnumerable<object> enumerable)
            {
                return enumerable.Count();
            }
            return 0; // 如果不是集合类型，返回0
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
