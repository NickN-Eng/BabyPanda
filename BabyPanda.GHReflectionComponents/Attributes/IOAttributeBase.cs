using System;

namespace BabyPanda.GHReflectionComponents
{
    /// <summary>
    /// WORK IN PROGRESS
    /// </summary>
    public class IOAttributeBase : Attribute
    {
        public string ShortName { get; set; } = null;

        public string Description { get; set; } = null;

        /// <summary>
        /// Set to true if a double value is an angle. Not applicable for other types.
        /// </summary>
        public bool IsAngle { get; set; } = false;

        /// <summary>
        /// Set to true if an angle value is in radians. Not applicable for other types.
        /// </summary>
        public bool IsRadians { get; set; } = false;

        public Type OptionalParameterType { get; set; }

        public IOAttributeBase(string shortName, string description, bool isAngle = false, bool isRadians = false, Type optionalParameterType = null )
        {
            ShortName = shortName;
            Description = description;
            IsAngle = isAngle;
            IsRadians = isRadians;
        }
    }
}
