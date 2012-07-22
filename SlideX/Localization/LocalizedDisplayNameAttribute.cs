using System;
using System.ComponentModel;
using System.Reflection;

namespace SlideX.Localization
{
    /// <summary>
    /// Provides localization for Display attribute
    /// </summary>
    public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        private PropertyInfo nameProperty;
        private Type resourceType;

        public LocalizedDisplayNameAttribute(string displayNameKey)
            : base(displayNameKey)
        {
            //Do nothing
        }

        /// <summary>
        /// Gets or sets the type of the name resource.
        /// </summary>
        /// <value>
        /// The type of the name resource.
        /// </value>
        public Type NameResourceType
        {
            get
            {
                return resourceType;
            }
            set
            {
                resourceType = value;
                nameProperty = resourceType.GetProperty(base.DisplayName, BindingFlags.Static | BindingFlags.Public);
            }
        }

        /// <summary>
        /// Gets the display name for a property, event, or public void method that takes no arguments stored in this attribute.
        /// </summary>
        /// <returns>The display name.</returns>
        public override string DisplayName
        {
            get
            {
                if (nameProperty == null)
                {
                    return base.DisplayName;
                }
                return (string)nameProperty.GetValue(nameProperty.DeclaringType, null);
            }
        }
    }
}