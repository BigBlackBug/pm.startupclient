using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace PlaymoreClient.Assets
{
	public class DescriptionEnumTypeConverter<T> : EnumConverter
	where T : struct
	{
		private readonly Dictionary<T, string> s_toString;

		private readonly Dictionary<string, T> s_toValue;

		private bool s_isInitialized;

		static DescriptionEnumTypeConverter()
		{
		}

		public DescriptionEnumTypeConverter() : base(typeof(T))
		{
			if (!this.s_isInitialized)
			{
				this.Initialize();
				this.s_isInitialized = true;
			}
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string str = value as string;
			if (!string.IsNullOrEmpty(str) && this.s_toValue.ContainsKey(str))
			{
				return this.s_toValue[str];
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			T t = (T)value;
			if (destinationType == typeof(string) && this.s_toString.ContainsKey(t))
			{
				return this.s_toString[t];
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		private static string GetDescription(T optionValue)
		{
			string str = optionValue.ToString();
			FieldInfo field = typeof(T).GetField(str);
			if (!Attribute.IsDefined(field, typeof(DescriptionAttribute)))
			{
				return str;
			}
			return ((DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))).Description;
		}

		protected void Initialize()
		{
			foreach (T value in Enum.GetValues(typeof(T)))
			{
				string description = DescriptionEnumTypeConverter<T>.GetDescription(value);
				this.s_toString[value] = description;
				this.s_toValue[description] = value;
			}
		}
	}
}