using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace JetBlack.MessageBus.Common.ComponentModel
{
    /// <summary>
    /// An implementation of a property descriptor to be used with custom type.
    /// </summary>
    public class CustomPropertyDescriptor : PropertyDescriptor
    {
        private IDictionary<Type, Type> _editors;

        /// <summary>
        /// Construct the property descriptor.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="type">The type of the property.</param>
        /// <param name="attrs">The properties attributes.</param>
        public CustomPropertyDescriptor(string name, Type type, Attribute[] attrs)
            : base(name, attrs)
        {
            PropertyType = type;
        }

        /// <inheritdoc />
        public override TypeConverter Converter
        {
            get
            {
                var typeConverterAttribute = GetAttribute<TypeConverterAttribute>(AttributeArray);
                var typeConverterType = typeConverterAttribute == null ? null : Type.GetType(typeConverterAttribute.ConverterTypeName);
                return typeConverterType == null ? null : TypeDescriptor.GetConverter(typeConverterType);
            }
        }

        /// <inheritdoc />
        public override object GetEditor(Type editorBaseType)
        {
            if (_editors == null)
                _editors = GetAttributes<EditorAttribute>(AttributeArray).ToDictionary(x => Type.GetType(x.EditorBaseTypeName), x => Type.GetType(x.EditorTypeName));

            Type editorType;
            if (!_editors.TryGetValue(editorBaseType, out editorType))
                return null;
            return Activator.CreateInstance(editorType);
        }

        /// <inheritdoc />
        public override bool IsLocalizable => GetAttribute<LocalizableAttribute>(AttributeArray).IsLocalizable;

        /// <inheritdoc />
        public override bool IsBrowsable => GetAttribute<BrowsableAttribute>(AttributeArray).Browsable;

        /// <inheritdoc />
        public override bool DesignTimeOnly => GetAttribute<DesignOnlyAttribute>(AttributeArray).IsDesignOnly;

        /// <inheritdoc />
        public override string Category => GetAttribute<CategoryAttribute>(AttributeArray).Category;

        /// <inheritdoc />
        public override string Description => GetAttribute<DescriptionAttribute>(AttributeArray).Description;

        /// <inheritdoc />
        public override string DisplayName => GetAttribute<DisplayNameAttribute>(AttributeArray)?.DisplayName ?? Name;

        /// <inheritdoc />
        public override bool CanResetValue(object component)
        {
            return GetAttribute<DefaultValueAttribute>(AttributeArray) != null;
        }

        /// <inheritdoc />
        public override Type ComponentType
        {
            get { throw new NotImplementedException(); }
        }

        /// <inheritdoc />
        public override object GetValue(object component)
        {
            var obj = (CustomType)component;
            object value;
            obj.Values.TryGetValue(Name, out value);
            return value;
        }

        /// <inheritdoc />
        public override bool IsReadOnly => GetAttribute<ReadOnlyAttribute>(AttributeArray)?.IsReadOnly ?? false;

        /// <inheritdoc />
        public override Type PropertyType { get; }

        /// <inheritdoc />
        public override void ResetValue(object component)
        {
            SetValue(component, GetAttribute<DefaultValueAttribute>(AttributeArray).Value);
        }

        /// <inheritdoc />
        public override void SetValue(object component, object value)
        {
            var oldValue = GetValue(component);

            if (oldValue != value)
            {
                var obj = (CustomType)component;
                obj.Values[Name] = value;
                OnValueChanged(component, new PropertyChangedEventArgs(Name));
            }
        }

        /// <inheritdoc />
        public override bool ShouldSerializeValue(object component)
        {
            return !IsReadOnly;
        }

#if DEBUG
        /// <inheritdoc />
        public override void AddValueChanged(object component, EventHandler handler)
        {
            // set a breakpoint here to see WPF attaching a value changed handler
            base.AddValueChanged(component, handler);
        }
#endif

        private static T GetAttribute<T>(IReadOnlyCollection<Attribute> attrs) where T : Attribute
        {
            if (attrs == null || attrs.Count == 0)
                return null;
            return attrs.OfType<T>().FirstOrDefault();
        }

        private static IEnumerable<T> GetAttributes<T>(IReadOnlyCollection<Attribute> attrs) where T : Attribute
        {
            if (attrs == null || attrs.Count == 0)
                return null;
            return attrs.OfType<T>();
        }
    }
}
