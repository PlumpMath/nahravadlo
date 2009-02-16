using System;
using System.Reflection;

namespace Nahravadlo.Schedule.VLC
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    class NameValueAttribute : Attribute
    {
        public NameValueAttribute(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }
        public string Value { get; private set; }

        public static NameValueAttribute FromEnum(Enum en)
        {
            var field = en.GetType().GetField(en.ToString());

            return GetCustomAttribute(field, typeof (NameValueAttribute)) as NameValueAttribute;
        }

        public static Enum FromValue(Type en, string value)
        {
            foreach(Enum item in Enum.GetValues(en))
            {
                var attribute = FromEnum(item);
                if (attribute != null && attribute.Value.Equals(value,StringComparison.InvariantCultureIgnoreCase))
                    return item;
            }
            return null;
        }
    }
}