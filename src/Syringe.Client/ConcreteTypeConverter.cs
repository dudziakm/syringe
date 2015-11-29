using System;
using Newtonsoft.Json;

namespace Syringe.Client
{
	public class ConcreteTypeConverter<TInterface, TConcrete> : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType.IsInterface && objectType == typeof(TInterface);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return serializer.Deserialize<TConcrete>(reader);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value);
		}
	}
}