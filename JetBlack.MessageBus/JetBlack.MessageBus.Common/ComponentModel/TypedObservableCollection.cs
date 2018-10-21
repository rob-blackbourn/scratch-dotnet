using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace JetBlack.MessageBus.Common.ComponentModel
{
    /// <summary>
    /// An observable collection which supports ITypedList
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    public class TypedObservableCollection<T> : ObservableCollection<T>, ITypedList
    {
        private readonly PropertyDescriptorCollection _properties;

        /// <summary>
        /// Construct the collection with a given collection of properties.
        /// </summary>
        /// <param name="properties">The properties in the collection.</param>
        public TypedObservableCollection(PropertyDescriptorCollection properties)
        {
            _properties = properties;
        }

        /// <summary>
        /// Construct the collection with a given collection of properties and source values.
        /// </summary>
        /// <param name="properties">The properties in the collection.</param>
        /// <param name="source">The values in the collection.</param>
        public TypedObservableCollection(PropertyDescriptorCollection properties, List<T> source)
            : base(source)
        {
            _properties = properties;
        }

        /// <summary>
        /// Construct the collection with a given collection of properties and source values.
        /// </summary>
        /// <param name="properties">The properties in the collection.</param>
        /// <param name="source">The values in the collection.</param>
        public TypedObservableCollection(PropertyDescriptorCollection properties, IEnumerable<T> source)
            : base(source)
        {
            _properties = properties;
        }

        /// <summary>Returns the name of the list.</summary>
        /// <returns>The name of the list.</returns>
        /// <param name="listAccessors">An array of <see cref="T:System.ComponentModel.PropertyDescriptor" /> objects, for which the list name is returned. This can be null. </param>
        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return typeof(T).Name;
        }

        /// <summary>Returns the <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> that represents the properties on each item used to bind data.</summary>
        /// <returns>The <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> that represents the properties on each item used to bind data.</returns>
        /// <param name="listAccessors">An array of <see cref="T:System.ComponentModel.PropertyDescriptor" /> objects to find in the collection as bindable. This can be null. </param>
        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return _properties;
        }
    }
}
