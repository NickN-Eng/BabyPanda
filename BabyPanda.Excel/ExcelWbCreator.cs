using ExcelBrowser.Interop;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyPanda.Excel
{
    public class ExcelWbCreator
    {
        /// <summary>
        /// The path of the new file.
        /// If this is left blank, the new excel file created,but will not be saved.
        /// Can be a filename, in which case the working directory will be used.
        /// </summary>
        public string NewFilepath { get; set; }

        /// <summary>
        /// The path of a template excel file which will be copied to create the new file.
        /// Can be a filename, in which case the working directory will be used.
        /// </summary>
        public string TemplateFilepath { get; set; }

        private string workingDirectory = null;
        /// <summary>
        /// If the NewFilepath/TemplateFilepath is instead a fileNAME, this working directory will be used.
        /// </summary>
        public string WorkingDirectory 
        { 
            get => workingDirectory != null ? workingDirectory : System.IO.Directory.GetCurrentDirectory(); 
            set => workingDirectory = value; 
        }

        /// <summary>
        /// Sets the application to visible if true.
        /// Hiding the window is not recommended.
        /// </summary>
        public bool ShowWindow { get; set; } = true;

        public bool Execute(out Workbook workbook)
        {
            //Get an excel app
            Application app;
            try
            {
                app = (Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");
            }
            catch
            {
                app = new Application();
            }
            app.Visible = ShowWindow;

            if (TemplateFilepath == null)
            {
                workbook = app.Workbooks.Add();
            }
            else
            {
                var templatepath = GetFilepath(NewFilepath);
                workbook = app.Workbooks.Add(templatepath);
            }

            var fpath = GetFilepath(NewFilepath);
            if (fpath != null)
            {
                workbook.SaveAs(fpath);
            }

            return true;
        }

        private string GetFilepath(string potentialFilename)
        {
            if (potentialFilename == null) return null;
            var name = Path.GetFileName(potentialFilename);
            if(name != potentialFilename)
                return potentialFilename;

            return Path.Combine(WorkingDirectory, potentialFilename);
        }
    }


}
