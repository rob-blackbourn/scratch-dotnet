using System;
using System.ComponentModel;
using System.Reflection;

namespace JetBlack.MessageBus.Common.ComponentModel
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class NameAndDescriptionAttribute : DescriptionAttribute
    {
        public NameAndDescriptionAttribute(string name, string description)
            : base(description)
        {
            Name = name;
        }

        public string Name { get; }

        public static NameAndDescriptionAttribute GetAttributeFromEnum(Type enumType, int enumValue)
        {
            foreach (var fieldInfo in enumType.GetFields())
            {
                if (fieldInfo.IsLiteral && (int)fieldInfo.GetRawConstantValue() == enumValue)
                {
                    var attrs = (NameAndDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(NameAndDescriptionAttribute), true);
                    if (attrs.Length == 1)
                        return attrs[0];
                }
            }
            return null;
        }

        public static NameAndDescriptionAttribute GetAttributeFromEnum<T>(T enumValue)
        {
            foreach (var fieldInfo in typeof(T).GetFields())
            {
                if (fieldInfo.IsLiteral && ((T)fieldInfo.GetRawConstantValue()).Equals(enumValue))
                {
                    var attrs = (NameAndDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(NameAndDescriptionAttribute), true);
                    if (attrs.Length == 1)
                        return attrs[0];
                }
            }
            return null;
        }

        public static string GetDescriptionFromEnum<T>(T enumValue) where T : struct
        {
            return GetDescriptionFromEnum(enumValue, "#undef");
        }

        public static string GetDescriptionFromEnum<T>(T enumValue, string undefName) where T : struct
        {
            var attr = GetAttributeFromEnum(enumValue);
            return attr == null ? undefName : attr.Description;
        }

        public static FieldInfo GetFieldInfoFromDescription(Type enumType, string description)
        {
            foreach (var fieldInfo in enumType.GetFields())
            {
                var attrs = (NameAndDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(NameAndDescriptionAttribute), true);
                if (attrs.Length == 1)
                    if (description == attrs[0].Description)
                        return fieldInfo;
            }

            return null;
        }

        public static int GetEnumFromDescription(Type enumType, string enumText)
        {
            var fieldInfo = GetFieldInfoFromDescription(enumType, enumText);
            if (fieldInfo != null)
                return (int)fieldInfo.GetRawConstantValue();

            throw new ArgumentException("invalid enum");
        }

        public static FieldInfo GetFieldInfoFromDescription<T>(string description)
        {
            foreach (var fieldInfo in typeof(T).GetFields())
            {
                NameAndDescriptionAttribute[] attrs = (NameAndDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(NameAndDescriptionAttribute), true);
                if (attrs.Length == 1)
                    if (String.Equals(description, attrs[0].Description, StringComparison.CurrentCultureIgnoreCase))
                        return fieldInfo;
            }

            return null;
        }

        public static T GetEnumFromDescription<T>(string description)
        {
            var fieldInfo = GetFieldInfoFromDescription<T>(description);
            if (fieldInfo != null)
                return (T)fieldInfo.GetRawConstantValue();

            var error = $"Could not parse type {typeof(T).FullName} from string \"{description}\": Type is invalid.";
            throw new ArgumentException(error, nameof(description));
        }
    }
}
