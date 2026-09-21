using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace Marketplace_Group_Project.Converters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            // Проверяем, что значение является перечислением
            if (!value.GetType().IsEnum)
                return value.ToString();

            // Получаем поле перечисления
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null)
                return value.ToString();

            // Пытаемся получить атрибут Description
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo
                .GetCustomAttributes(typeof(DescriptionAttribute), false);

            // Если атрибут найден, возвращаем его описание
            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;

            // Иначе возвращаем строковое представление значения
            return value.ToString();
        }

        // Обратное преобразование (не реализовано).
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Обратное преобразование не поддерживается.");
        }
    }
}
