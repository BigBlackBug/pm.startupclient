using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Util
{
	public static class AssemblyAttributes
	{
		public static string Configuration
		{
			get
			{
				AssemblyConfigurationAttribute attribute = AssemblyAttributes.GetAttribute<AssemblyConfigurationAttribute>();
				if (attribute == null)
				{
					return null;
				}
				return attribute.Configuration;
			}
		}

		public static string FileVersion
		{
			get
			{
				AssemblyFileVersionAttribute attribute = AssemblyAttributes.GetAttribute<AssemblyFileVersionAttribute>();
				if (attribute == null)
				{
					return null;
				}
				return attribute.Version;
			}
		}

		public static T GetAttribute<T>()
		where T : Attribute
		{
			return (T)(typeof(AssemblyAttributes).Assembly.GetCustomAttributes(typeof(T), false).FirstOrDefault<object>((object o) => o is T) as T);
		}
	}
}