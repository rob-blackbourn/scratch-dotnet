using System;
using System.ComponentModel;

namespace JetBlack.MessageBus.Common.ComponentModel
{
    /// <summary>
    /// A type convert for an enum, the members of which have been decorated with description attributes,
    /// </summary>
    public class EnumDescriptionTypeConverter : EnumConverter
    {
        /// <summary>
        /// The constructor of the enum type descriptor.
        /// </summary>
        /// <param name="type">The type of the enum to convert</param>
        public EnumDescriptionTypeConverter(Type type)
            : base(type)
        {
        }

        /// <summary>Converts the given value object to the specified destination type.</summary>
        /// <returns>An <see cref="T:System.Object" /> that represents the converted <paramref name="value" />.</returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context. </param>
        /// <param name="culture">An optional <see cref="T:System.Globalization.CultureInfo" />. If not supplied, the current culture is assumed. </param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert. </param>
        /// <param name="destinationType">The <see cref="T:System.Type" /> to convert the value to. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="destinationType" /> is null. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="value" /> is not a valid value for the enumeration. </exception>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string))
                return base.ConvertTo(context, culture, value, destinationType);

            var fi = value?.GetType().GetField(value.ToString());
            if (fi == null)
                return string.Empty;

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) && !string.IsNullOrEmpty(attributes[0].Description) ? attributes[0].Description : value.ToString();
        }
    }
}
