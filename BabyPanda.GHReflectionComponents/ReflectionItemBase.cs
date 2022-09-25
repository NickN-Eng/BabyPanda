using System;
using System.Reflection;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace BabyPanda.GHReflectionComponents
{

    /// <summary>
    /// WORK IN PROGRESS
    /// </summary>
    public abstract class ReflectionItemBase
    {
        /// <summary>
        /// Long name of the attribute
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of a single item of this parameter. 
        /// Should be written to handle primitive type (e.g. int, double, GeometryBase) and also Goo
        /// </summary>
        public Type UnitType { get; set; }

        public bool UnitIsGoo => typeof(IGH_Goo).IsAssignableFrom(UnitType);

        /// <summary>
        /// The type of the property. 
        /// For GH_ParamAccess.item, will be the UnitType. If a Goo type is used, 
        /// For GH_ParamAccess.list, will be the List<UnitType>. ATM, only LIST is supported. For input, List type is preferred
        /// For GH_ParamAccess.tree, can be DataTree or GH_Structure. For input, GH_Structure is preferred.
        /// </summary>
        public Type OriginalType { get; set; }

        public GenericType GenericType {get; set;}

        public PropertyInfo Property { get; set; }

        public int Index { get; set; }

        /// <summary>
        /// Is the datatype an item, list or tree?
        /// </summary>
        public GH_ParamAccess ParamAccess { get; set; }

        public abstract IOAttributeBase IOAttribute { get; }
    }

    public class InputReflectionItem : ReflectionItemBase
    {
        public InputAttribute InputAttribute { get; set; }

        public override IOAttributeBase IOAttribute => InputAttribute;

        public object DefaultValue;
    }

    public class OutputReflectionItem : ReflectionItemBase
    {
        public OutputAttribute OutputAttribute { get; set; }

        public override IOAttributeBase IOAttribute => OutputAttribute;
    }

}
