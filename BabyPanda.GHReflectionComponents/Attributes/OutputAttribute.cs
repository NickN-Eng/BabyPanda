using System;

namespace BabyPanda.GHReflectionComponents
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OutputAttribute : IOAttributeBase
    {
        public OutputAttribute(string shortName, string description, bool isAngle = false, bool isRadians = false, Type optionalParameterType = null) : base(shortName, description, isAngle, isRadians, optionalParameterType)
        {
        }
    }
}
