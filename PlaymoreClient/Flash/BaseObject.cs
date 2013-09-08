using PlaymoreClient.Messages;
using FluorineFx;
using FluorineFx.AMF3;
using NotMissing;
using NotMissing.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace PlaymoreClient.Flash
{
	public class BaseObject
    {
		protected readonly ASObject Base;

		public BaseObject(ASObject obj)
		{
			this.Base = obj;
		}

		public static object GetObject(object obj)
		{
            PlaymoreClient.Gui.MainForm.LOGGER.Info("GetObject " + obj);
			if (obj == null)
			{
				return null;
			}
			Type type = obj.GetType();
			if (type != typeof(BaseObject))
			{
				if (type != typeof(BaseList<>))
				{
					return obj;
				}
				return new ArrayCollection((IList)obj);
			}
			MessageAttribute attribute = type.GetAttribute<MessageAttribute>();
			if (attribute.FullName == null)
			{
				throw new NotSupportedException(string.Format("Serialization for type {0} not supported", type.FullName));
			}
			ASObject aSObject = new ASObject(attribute.FullName);
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < (int)properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				InternalNameAttribute internalNameAttribute = propertyInfo.GetAttribute<InternalNameAttribute>();
				if (internalNameAttribute != null)
				{
					aSObject[internalNameAttribute.Name] = propertyInfo.GetValue(obj, null);
				}
			}
			return aSObject;
		}

		public static void SetFields<T>(T obj, ASObject flash)
		{
			object str;
			if (flash == null)
			{
				return;
			}
			PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < (int)properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				InternalNameAttribute attribute = propertyInfo.GetAttribute<InternalNameAttribute>();
				if (attribute != null)
				{
					Type propertyType = propertyInfo.PropertyType;
					if (flash.TryGetValue(attribute.Name, out str))
					{
						try
						{
							if (propertyType == typeof(string))
							{
								str = Convert.ToString(flash[attribute.Name]);
							}
							else if (propertyType == typeof(int))
							{
								str = Convert.ToInt32(flash[attribute.Name]);
							}
							else if (propertyType == typeof(long))
							{
								str = Convert.ToInt64(flash[attribute.Name]);
							}
							else if (propertyType == typeof(double))
							{
								str = Convert.ToInt64(flash[attribute.Name]);
							}
							else if (propertyType == typeof(bool))
							{
								str = Convert.ToBoolean(flash[attribute.Name]);
							}
							else if (propertyType == typeof(DateTime))
							{
								str = Convert.ToDateTime(flash[attribute.Name]);
							}
							else if (propertyType == typeof(ASObject))
							{
								str = flash[attribute.Name] as ASObject;
							}
							else if (propertyType == typeof(ArrayCollection))
							{
								str = flash[attribute.Name] as ArrayCollection;
							}
							else if (propertyType != typeof(object))
							{
								try
								{
									object[] item = new object[] { flash[attribute.Name] };
									str = Activator.CreateInstance(propertyType, item);
								}
								catch (Exception exception1)
								{
									Exception exception = exception1;
									throw new NotSupportedException(string.Format("Type {0} not supported by flash serializer", propertyType.FullName), exception);
								}
							}
							else
							{
								str = flash[attribute.Name];
							}
							propertyInfo.SetValue(obj, str, null);
						}
						catch (Exception exception3)
						{
							Exception exception2 = exception3;
							PlaymoreClient.Gui.MainForm.LOGGER.Error(new Exception(string.Format("Error parsing {0}#{1}", typeof(T).FullName, propertyInfo.Name), exception2));
						}
					}
					else
					{
						PlaymoreClient.Gui.MainForm.LOGGER.Warn(string.Format("{0} missing ASObject property {1}", typeof(T).FullName, attribute.Name));
					}
				}
			}
		}
	}
}