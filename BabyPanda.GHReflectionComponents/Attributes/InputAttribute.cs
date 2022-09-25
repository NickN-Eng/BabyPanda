using System;

namespace BabyPanda.GHReflectionComponents
{
    [AttributeUsage(AttributeTargets.Property)]
    public class InputAttribute : IOAttributeBase
    {
        /// <summary>
        /// If true, the initial value of the property will be used as the default value during AddInputParameter.
        /// </summary>
        public bool HasDefaultValue { get; set; } = false;

        public InputAttribute(string shortName, string description, bool hasDefault = false, bool isAngle = false, bool isRadians = false, Type optionalParameterType = null) : base(shortName, description, isAngle, isRadians, optionalParameterType)
        {
            HasDefaultValue = hasDefault;
        }
    }
}
