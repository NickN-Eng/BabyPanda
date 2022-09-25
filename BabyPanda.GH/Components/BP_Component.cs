using Grasshopper.Kernel;
using System.Collections;
using System.Collections.Generic;

namespace BabyPanda.GH
{
    /// <summary>
    /// A base class for components with useful (but non-essential) methods such as DA_GetData.
    /// </summary>
    public abstract class BP_Component : GH_Component
    {
        public BP_Component(string name, string nickname, string description, string category, string subCategory) 
            : base(name, nickname, description, category, subCategory)
        {

        }

        /// <summary>
        /// Gets data from the Data Access object. Does not check for null values.
        /// </summary>
        public bool DA_GetData<T>(IGH_DataAccess DA, int index, out T value)
        {
            value = default(T);
            var success = DA.GetData(index, ref value);
            if (!success)
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Could not get data from parameter no " + index.ToString() + ".");
            return success;
        }

        /// <summary>
        /// Gets data from the Data Access object. Checks for null values.
        /// </summary>
        public bool DA_GetDataAndCheckNull<T>(IGH_DataAccess DA, int index, out T value)
        {
            value = default(T);
            var success = DA.GetData(index, ref value);
            if (!success)
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Could not get data from parameter no " + index.ToString() + ".");
            if (value == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Null value input at parameter no " + index.ToString() + ".");
                return false;
            }

            return success;
        }

        /// <summary>
        /// Gets data list from the Data Access object. Does not check for null values.
        /// </summary>
        public bool DA_GetDataList<T>(IGH_DataAccess DA, int index, out List<T> value)
        {
            value = new List<T>();
            var success = DA.GetDataList(index, value);
            if (!success)
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Could not get data from parameter no " + index.ToString() + ".");
            return success;
        }

        /// <summary>
        /// Checks that all the supplied lists are of equal length.
        /// Does not check the supplied list for null, and will throw error if lists are null.
        /// </summary>
        public bool CheckListsAreSameLength(string listNames, params ICollection[] lists)
        {
            if (lists.Length <= 1) return true;

            int firstListLen = lists[0].Count;
            for (int i = 1; i < lists.Length; i++)
            {
                if(firstListLen != lists[i].Count)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The " + listNames + " are not of the same length.");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks that all the supplied lists are of equal length.
        /// Does not check the supplied list for null, and will throw error if lists are null.
        /// </summary>
        public bool CheckListsAreSameLength(params ICollection[] lists)
        {
            return CheckListsAreSameLength("input lists", lists);
        }
    }
}
