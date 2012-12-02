using System;

namespace CompareJsonSerializers.SimpleJson
{
    public class SimpleJsonSerializerStrategy : PocoJsonSerializerStrategy
    {
        protected override object SerializeEnum(System.Enum p)
        {
            return p.ToString();
        }

        public override object DeserializeObject(object value, System.Type type)
        {
            if (type.IsEnum)
                return Enum.Parse(type, value.ToString());

            return base.DeserializeObject(value, type);
        }
    }
}