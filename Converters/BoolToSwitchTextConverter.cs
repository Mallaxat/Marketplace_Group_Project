using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Marketplace_Group_Project.Converters
{
    public class BoolToSwitchTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isRegistration)
            {
                return isRegistration
                    ? "Есть аккаунт? Войти"
                    : "Нет аккаунта? Зарегистрироваться";
            }
            return "Переключить режим";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
