using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace WorkClocker.ViewModel.Converters
{
	public class BooleanConverter<T> : IValueConverter
	{
		public BooleanConverter()
		{
		}

		public BooleanConverter(T trueValue, T falseValue)
		{
			True = trueValue;
			False = falseValue;
		}

		public T True { get; set; }
		public T False { get; set; }

		public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var s = parameter as string;
			if (s != null && s == @"!")
				return value is bool && ((bool)value) ? False : True;
			return value is bool && ((bool)value) ? True : False;
		}

		public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is T && EqualityComparer<T>.Default.Equals((T)value, True);
		}
	}
}