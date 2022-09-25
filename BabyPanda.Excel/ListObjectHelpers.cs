using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyPanda.Excel
{
    public static class ListObjectHelpers
    {
        //Test for single value
        public static Range GetColumnRange(this ListObject table, string columnName)
        {
            ListColumns cols = table.ListColumns;
            foreach (ListColumn col in table.ListColumns)
            {
                if (col.Name != columnName) continue;
                return col.DataBodyRange;
            }
            return null;

        }

        public static ListColumn GetColumn(this ListObject table, string columnName)
        {
            ListColumns cols = table.ListColumns;
            foreach (ListColumn col in table.ListColumns)
            {
                if (col.Name != columnName) continue;
                return col;
            }
            return null;

        }


        //public static void WriteToColumn(this ListObject table, string columnName, 
    }

}
