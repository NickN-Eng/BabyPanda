using ExcelBrowser.Interop;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyPanda.Excel
{
    public static class WorkbookHelpers
    {

        /// <summary>
        /// Finds the table which matches the name, otherwise returns null
        /// </summary>
        /// <param name="tableName">Excel range name of the table</param>
        /// <returns></returns>
        public static ListObject GetTable(this Workbook wb, string tableName)
        {
            foreach (Worksheet ws in wb.Worksheets)
            {
                foreach (ListObject table in ws.ListObjects)
                {
                    if (table.Name == tableName) return table;
                }
            }

            return null;
        }

        public static IEnumerable<Workbook> GetOpenWorkbooks()
        {
            //First try Marshal.GetActiveObject first since this can find an instance of
            //the excel app, even if it is an invisible instance.
            Application app = null;
            try
            {
                app = (Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");
            }
            catch (Exception) { }
            if(app == null) yield break;

            foreach (Workbook wb in app.Workbooks)
            {
                yield return wb;
            }

            //Use ExcelBrowser C# library to find apps which are open, but not the last used Excel.Application object
            //Application objects which are hidden will not be found.
            var sesh = Session.Current;
            var apps = sesh.Apps;

            foreach (Application seshApp in apps)
            {
                if (seshApp == app) continue;

                foreach (Workbook wb in seshApp.Workbooks)
                {
                    yield return wb;
                }
            }
        }
    }

    public static class ApplicationHelpers
    {
        public static IEnumerable<Workbook> GetWorkbooks(this Application app)
        {
            foreach (Workbook wb in app.Workbooks)
            {
                yield return wb;
            }
        }
    }
}
