using Grasshopper;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using System.Collections.Generic;

namespace BabyPanda.GHReflectionComponents
{
    public static class ComponentHelpers
    {
        public static DataTree<T> CastToDataTree<T, Goo>(GH_Structure<Goo> structure) where Goo : IGH_Goo
        {
            var tree = new DataTree<T>();
            foreach (GH_Path path in structure.Paths)
            {
                var items = new List<T>();
                foreach (IGH_Goo item in structure[path])
                {
                    T obj;
                    var sucess = item.CastTo<T>(out obj);
                    if (sucess) items.Add(obj);
                    else items.Add(default(T));
                }
                tree.AddRange(items, path);
            }
            return tree;
        }

        public static DataTree<Goo> ConvertToDataTree<Goo>(GH_Structure<Goo> structure) where Goo : IGH_Goo
        {
            var tree = new DataTree<Goo>();
            foreach (GH_Path path in structure.Paths)
            {
                tree.AddRange(structure[path], path);
            }
            return tree;
        }
    }
}