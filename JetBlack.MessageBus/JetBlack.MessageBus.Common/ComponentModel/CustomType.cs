using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace JetBlack.MessageBus.Common.ComponentModel
{
    /// <summary>
    /// A simple implementation of a custom type descriptor.
    /// </summary>
    public class CustomType : CustomTypeDescriptor
    {
        /// <summary>
        /// The underlying dictionary of values.
        /// </summary>
        public readonly IDictionary<string, object> Values = new Dictionary<string, object>();

        /// <summary>
        /// Construct a custom type.
        /// </summary>
        /// <param name="className">The class name.</param>
        /// <param name="properties">The class properties.</param>
        /// <param name="attributes">The class attributes.</param>
        /// <param name="events">The class events.</param>
        public CustomType(string className = "CustomClass", PropertyDescriptorCollection properties = null, IEnumerable<Attribute> attributes = null, EventDescriptorCollection events = null)
        {
            ClassName = className;
            Attributes = attributes != null ? new AttributeCollection(attributes.ToArray()) : new AttributeCollection();
            Properties = properties ?? new PropertyDescriptorCollection(new PropertyDescriptor[0]);
            Events = events ?? new EventDescriptorCollection(new EventDescriptor[0]);
        }

        /// <summary>
        /// The name of the class.
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        /// The properties of the class.
        /// </summary>
        public PropertyDescriptorCollection Properties { get; }

        /// <summary>
        /// The attributes of the class.
        /// </summary>
        public AttributeCollection Attributes { get; }

        /// <summary>
        /// The events of the class.
        /// </summary>
        public EventDescriptorCollection Events { get; }

        /// <summary>Returns the fully qualified name of the class represented by this type descriptor.</summary>
        /// <returns>A <see cref="T:System.String" /> containing the fully qualified class name of the type this type descriptor is describing. The default is null.</returns>
        public override string GetClassName()
        {
            return ClassName;
        }

        /// <summary>Returns a collection of custom attributes for the type represented by this type descriptor.</summary>
        /// <returns>An <see cref="T:System.ComponentModel.AttributeCollection" /> containing the attributes for the type. The default is <see cref="F:System.ComponentModel.AttributeCollection.Empty" />.</returns>
        public override AttributeCollection GetAttributes() => Attributes;

        /// <summary>
        /// Add a property to the custom type.
        /// </summary>
        /// <param name="name">The property name</param>
        /// <param name="type">The property type</param>
        /// <param name="attrs">The property attributes.</param>
        public void AddProperty(string name, Type type, Attribute[] attrs = null)
        {
            Properties.Add(new CustomPropertyDescriptor(name, type, attrs));
        }

        /// <summary>
        /// Remove a property from the custom type.
        /// </summary>
        /// <param name="name">The name of the property to remove.</param>
        /// <returns>If the property was deleted <c>true</c>, otherwise <c>false</c>.</returns>
        public bool RemoveProperty(string name)
        {
            var propertyDescriptor = Properties.Find(name, true);
            if (propertyDescriptor == null)
                return false;
            Properties.Remove(propertyDescriptor);
            return true;
        }

        /// <summary>
        /// Find if the property exists.
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <returns>If the property exists true, otherwise false.</returns>
        public bool ContainsProperty(string name)
        {
            return Properties.Find(name, false) != null;
        }

        /// <summary>
        /// Clear all the properties.
        /// </summary>
        public void ClearProperties()
        {
            Properties.Clear();
        }

        /// <summary>Returns a collection of property descriptors for the object represented by this type descriptor.</summary>
        /// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> containing the property descriptions for the object represented by this type descriptor. The default is <see cref="F:System.ComponentModel.PropertyDescriptorCollection.Empty" />.</returns>
        public override PropertyDescriptorCollection GetProperties()
        {
            return Properties;
        }

        /// <summary>Returns a filtered collection of property descriptors for the object represented by this type descriptor.</summary>
        /// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> containing the property descriptions for the object represented by this type descriptor. The default is <see cref="F:System.ComponentModel.PropertyDescriptorCollection.Empty" />.</returns>
        /// <param name="attributes">An array of attributes to use as a filter. This can be null.</param>
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return new PropertyDescriptorCollection(
                GetProperties()
                    .Cast<PropertyDescriptor>()
                    .Where(x => x.Attributes.Matches(attributes))
                    .ToArray());
        }

        /// <summary>Returns a collection of event descriptors for the object represented by this type descriptor.</summary>
        /// <returns>An <see cref="T:System.ComponentModel.EventDescriptorCollection" /> containing the event descriptors for the object represented by this type descriptor. The default is <see cref="F:System.ComponentModel.EventDescriptorCollection.Empty" />.</returns>
        public override EventDescriptorCollection GetEvents()
        {
            return Events;
        }

        /// <summary>Returns a filtered collection of event descriptors for the object represented by this type descriptor.</summary>
        /// <returns>An <see cref="T:System.ComponentModel.EventDescriptorCollection" /> containing the event descriptions for the object represented by this type descriptor. The default is <see cref="F:System.ComponentModel.EventDescriptorCollection.Empty" />.</returns>
        /// <param name="attributes">An array of attributes to use as a filter. This can be null.</param>
        public override EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return new EventDescriptorCollection(Events.Cast<EventDescriptor>().Where(x => x.Attributes.Matches(attributes)).ToArray());
        }

        /// <summary>
        /// The value of the named property.
        /// </summary>
        /// <param name="name">The property name</param>
        /// <returns>The property value</returns>
        public object this[string name]
        {
            get { return GetValue(name); }
            set { SetValue(name, value); }
        }

        /// <summary>
        /// Set the value of a property
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        public void SetValue(string name, object value)
        {
            var propertyDescriptor = Properties.Find(name, false);
            if (propertyDescriptor == null)
                throw new MissingMemberException(name);

            propertyDescriptor.SetValue(this, value);
        }

        /// <summary>
        /// Get the value of a property.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        public object GetValue(string name)
        {
            var propertyDescriptor = Properties.Find(name, false);
            if (propertyDescriptor == null)
                throw new MissingMemberException(name);
            return propertyDescriptor.GetValue(this);
        }

        /// <summary>
        /// Add an event handler for value changes.
        /// </summary>
        /// <param name="handler"></param>
        public void AddValueChanged(EventHandler handler)
        {
            foreach (var property in Properties.Cast<PropertyDescriptor>())
                property.AddValueChanged(this, handler);
        }

        /// <summary>
        /// Remove an event handler for value changes.
        /// </summary>
        /// <param name="handler">The handler to remove</param>
        public void RemoveValueChanged(EventHandler handler)
        {
            foreach (var property in Properties.Cast<PropertyDescriptor>())
                property.RemoveValueChanged(this, handler);
        }
    }
}
