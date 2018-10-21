using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace JetBlack.MessageBus.Common.ComponentModel
{
    public class EnumTypeConverter<TEnum> : TypeConverter where TEnum : struct
    {
        private readonly Type _enumType;
        private const string Separator = " | ";
        private readonly bool _multipleValues;
        private readonly Dictionary<string, TEnum> _fieldNameMap = new Dictionary<string, TEnum>();
        private readonly Dictionary<string, TEnum> _nameMap = new Dictionary<string, TEnum>();
        private readonly Dictionary<string, TEnum> _descriptionMap = new Dictionary<string, TEnum>();
        private readonly Dictionary<TEnum, string> _reverseNameMap = new Dictionary<TEnum, string>();
        private readonly Dictionary<TEnum, string> _reverseDescriptionMap = new Dictionary<TEnum, string>();

        public EnumTypeConverter()
        {
            _enumType = typeof(TEnum);

            _multipleValues = Attribute.IsDefined(_enumType, typeof(FlagsAttribute));

            var descriptionAttributeType = typeof(DescriptionAttribute);
            var nameAndDescriptionAttributeType = typeof(NameAndDescriptionAttribute);
            foreach (FieldInfo fieldInfo in _enumType.GetFields())
            {
                if (fieldInfo.IsLiteral)
                {
                    var value = (TEnum)fieldInfo.GetRawConstantValue();

                    DescriptionAttribute[] attrs;
                    attrs = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(nameAndDescriptionAttributeType, true);
                    if (attrs == null || attrs.Length == 0)
                        attrs = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(descriptionAttributeType, true);
                    var attr = (attrs != null && attrs.Length == 1 ? attrs[0] : null);

                    var name = (attr != null && attr is NameAndDescriptionAttribute ? ((NameAndDescriptionAttribute)attr).Name : string.Empty);
                    var description = (attrs != null && attrs.Length == 1 ? attrs[0].Description : string.Empty);

                    _fieldNameMap[fieldInfo.Name] = value;
                    _nameMap[name] = value;
                    _descriptionMap[description] = value;

                    _reverseNameMap[value] = name;
                    _reverseDescriptionMap[value] = description;
                }
            }
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(value, attributes);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || destinationType == typeof(KeyValuePair<string, string>[]);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value.GetType() == _enumType)
            {
                if (destinationType == typeof(string))
                {
                    List<string> itemList = new List<string>();
                    TEnum intValue = (TEnum)value;

                    foreach (KeyValuePair<TEnum, string> item in _reverseDescriptionMap)
                    {
                        if (
                            (!_multipleValues && item.Key.Equals(intValue))
                            || (_multipleValues && ((TEnum)(object)(Convert.ToInt32(item.Key) & Convert.ToInt32(intValue))).Equals(item.Key)))
                        {
                            itemList.Add(item.Value);
                        }
                    }
                    return String.Join(Separator, itemList.ToArray());
                }

                if (destinationType == typeof(KeyValuePair<string, string>))
                {
                    TEnum enumValue = (TEnum)value;
                    string description = _reverseDescriptionMap[enumValue];
                    string name = _reverseNameMap[enumValue];
                    return new KeyValuePair<string, string>(name, description);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                if ((string)value == string.Empty)
                {
                    DefaultValueAttribute[] defaultValueAttributes = (DefaultValueAttribute[])_enumType.GetCustomAttributes(typeof(DefaultValueAttribute), true);
                    if (defaultValueAttributes != null && defaultValueAttributes.Length > 0)
                    {
                        if (defaultValueAttributes[0].Value.GetType() == _enumType)
                            return Convert.ToInt32(defaultValueAttributes[0].Value);

                        if (defaultValueAttributes[0].Value.GetType() == typeof(string))
                            return ConvertFrom(context, culture, defaultValueAttributes[0].Value);
                    }

                    return Convert.ToInt32(default(TEnum));
                }

                string[] itemArray = ((string)value).Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
                int result = 0;
                foreach (string item in itemArray)
                {
                    TEnum itemValue;

                    if (_descriptionMap.ContainsKey(item))
                        itemValue = _descriptionMap[item];
                    else if (_nameMap.ContainsKey(item))
                        itemValue = _nameMap[item];
                    else if (_fieldNameMap.ContainsKey(item))
                        itemValue = _fieldNameMap[item];
                    else
                        continue;

                    if (_multipleValues)
                        result |= Convert.ToInt32(itemValue);
                    else
                    {
                        result = Convert.ToInt32(itemValue);
                        break;
                    }
                }

                return (TEnum)(object)result;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

}
