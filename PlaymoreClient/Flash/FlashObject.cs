using PlaymoreClient.Util;
using NotMissing.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Flash
{
	[DebuggerDisplay("{Name}")]
	public class FlashObject
	{
		public List<FlashObject> Fields
		{
			get;
			set;
		}

		public bool HasFields
		{
			get
			{
				return this.Fields.Count > 0;
			}
		}

		public virtual FlashObject this[string key]
		{
			get
			{
				return this.Fields.FirstOrDefault<FlashObject>((FlashObject fo) => fo.Name == key);
			}
			set
			{
				int num = this.Fields.FindIndex((FlashObject fo) => fo.Name == key);
				if (num == -1)
				{
					this.Fields.Add(value);
					return;
				}
				this.Fields[num] = value;
			}
		}

		public string Name
		{
			get;
			set;
		}

		public FlashObject Parent
		{
			get;
			set;
		}

		public string Type
		{
			get;
			set;
		}

		public string Value
		{
			get;
			set;
		}

		public FlashObject(string name) : this(name, null)
		{
		}

		public FlashObject(string name, string value) : this(null, name, value)
		{
		}

		public FlashObject(string type, string name, string value)
		{
			this.Type = type;
			this.Name = name;
			this.Value = value;
			this.Fields = new List<FlashObject>();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is FlashObject))
			{
				return this.Equals(obj);
			}
			return ((FlashObject)obj).Name == this.Name;
		}

		public override int GetHashCode()
		{
			if (this.Name == null)
			{
				return this.GetHashCode();
			}
			return this.Name.GetHashCode();
		}

		public static void SetFields<T>(T obj, FlashObject flash)
		{
			object value;
			if (flash == null)
			{
				return;
			}
			PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < (int)properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				InternalNameAttribute internalNameAttribute = propertyInfo.GetCustomAttributes(typeof(InternalNameAttribute), false).FirstOrDefault<object>() as InternalNameAttribute;
				if (internalNameAttribute != null)
				{
					System.Type propertyType = propertyInfo.PropertyType;
					try
					{
						if (propertyType == typeof(string))
						{
							value = flash[internalNameAttribute.Name].Value;
						}
						else if (propertyType == typeof(int))
						{
							value = Parse.Int(flash[internalNameAttribute.Name].Value);
						}
						else if (propertyType == typeof(long))
						{
							value = Parse.Long(flash[internalNameAttribute.Name].Value);
						}
						else if (propertyType != typeof(bool))
						{
							try
							{
								object[] item = new object[] { flash[internalNameAttribute.Name] };
								value = Activator.CreateInstance(propertyType, item);
							}
							catch (Exception exception1)
							{
								Exception exception = exception1;
								throw new NotSupportedException(string.Format("Type {0} not supported by flash serializer", propertyType.FullName), exception);
							}
						}
						else
						{
							value = Parse.Bool(flash[internalNameAttribute.Name].Value);
						}
						propertyInfo.SetValue(obj, value, null);
					}
					catch (Exception exception3)
					{
						Exception exception2 = exception3;
						PlaymoreClient.Gui.MainForm.LOGGER.Error(string.Format("Error parsing {0}#{1}", typeof(T).FullName, propertyInfo.Name));
						PlaymoreClient.Gui.MainForm.LOGGER.Error(exception2);
					}
				}
			}
		}

		public override string ToString()
		{
			return this.Name;
		}
	}
}