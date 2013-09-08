using PlaymoreClient.Messages;
using FluorineFx;
using FluorineFx.AMF3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PlaymoreClient.Messages.Translators
{
	public class MessageTranslator : IObjectTranslator
	{
		public static MessageTranslator Instance
		{
			get;
			private set;
		}

		protected Dictionary<string, Type> Types
		{
			get;
			set;
		}

		static MessageTranslator()
		{
			MessageTranslator.Instance = new MessageTranslator(typeof(MessageTranslator).Assembly.GetTypes());
		}

		public MessageTranslator(params Type[] types)
		{
			this.Types = new Dictionary<string, Type>();
			Type[] typeArray = types;
			for (int i = 0; i < (int)typeArray.Length; i++)
			{
				Type type = typeArray[i];
				MessageAttribute messageAttribute = type.GetCustomAttributes(typeof(MessageAttribute), false).FirstOrDefault<object>() as MessageAttribute;
				if (messageAttribute != null)
				{
					this.Types.Add(messageAttribute.Name, type);
				}
			}
		}

		public virtual object GetArray(ArrayCollection array)
		{
			if (array.Count < 1)
			{
				return array;
			}
			if (!(array[0] is ASObject))
			{
				return array;
			}
			object obj = this.GetObject((ASObject)array[0]);
			if (obj == null)
			{
				return null;
			}
			Type type = typeof(List<>);
			Type[] typeArray = new Type[] { obj.GetType() };
			IList lists = (IList)Activator.CreateInstance(type.MakeGenericType(typeArray));
			foreach (object obj1 in array)
			{
				if (!(obj1 is ASObject))
				{
					continue;
				}
				object obj2 = this.GetObject((ASObject)obj1);
				if (obj2 == null)
				{
					continue;
				}
				lists.Add(obj2);
			}
			return lists;
		}

		public virtual object GetObject(ASObject flashobj)
		{
			if (flashobj == null)
			{
				return null;
			}
			KeyValuePair<string, Type> keyValuePair = this.Types.FirstOrDefault<KeyValuePair<string, Type>>((KeyValuePair<string, Type> kv) => flashobj.TypeName.EndsWith(kv.Key));
			if (keyValuePair.Value == null)
			{
				return null;
			}
			return Activator.CreateInstance(keyValuePair.Value, new object[] { flashobj });
		}

		public virtual T GetObject<T>(ASObject flashobj)
		where T : class
		{
			return (T)(this.GetObject(flashobj) as T);
		}
	}
}