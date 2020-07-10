using Newtonsoft.Json.Linq;
using RestEase.Implementation;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RestEase.Serialization.Extensions
{
	/// <summary>
	/// Serializes object parameters in a format compatible with asp.net core mvc model binding for complex types
	/// https://docs.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-3.1#complex-types
	/// </summary>
	public class ComplexTypeRequestQueryParamSerializer : RequestQueryParamSerializer
	{
		public override IEnumerable<KeyValuePair<string, string>> SerializeQueryCollectionParam<T>(string name, IEnumerable<T> values, RequestQueryParamSerializerInfo info)
		{
			var dictionary = new Dictionary<string, string>();
			if (values == null)
				return dictionary;

			var i = 0;
			foreach (var value in values)
			{
				string itemName = $"{name}[{i}]";
				var paramDictionary = SerializeQueryParam(itemName, value, info);
				foreach (var kvp in paramDictionary)
				{
					dictionary.Add(kvp.Key, kvp.Value);
				}
			}
			return dictionary;
		}

		public override IEnumerable<KeyValuePair<string, string>> SerializeQueryParam<T>(string name, T value, RequestQueryParamSerializerInfo info)
		{
			var dictionary = new Dictionary<string, string>();
			if (value == null)
				return dictionary;

			var type = typeof(T);

			if (type.IsValueType || type == typeof(string))
			{
				dictionary.Add(name, ToStringHelper.ToString(value, info.Format, info.FormatProvider));
				return dictionary;
			}

			return ToKeyValue(value, name);
		}

		private static IDictionary<string, string> ToKeyValue(object obj, string name)
		{
			if (obj == null)
			{
				return null;
			}

			var token = obj as JToken;
			if (token == null)
			{
				return ToKeyValue(JObject.FromObject(obj), name);
			}

			if (token.HasValues)
			{
				var contentData = new Dictionary<string, string>();
				foreach (var child in token.Children().ToList())
				{
					var childContent = ToKeyValue(child, name);
					if (childContent != null)
					{
						contentData = contentData.Concat(childContent).ToDictionary(k => k.Key, v => v.Value);
					}
				}

				return contentData;
			}

			var data = token as JValue;
			if (data?.Value == null)
			{
				return null;
			}

			var value = data.Type == JTokenType.Date ?
				data.ToString("o", CultureInfo.InvariantCulture) :
				data.ToString(CultureInfo.InvariantCulture);

			return new Dictionary<string, string> { { name + "." + token.Path, value } };
		}
	}
}
