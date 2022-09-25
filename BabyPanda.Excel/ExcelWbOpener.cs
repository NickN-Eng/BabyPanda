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
    public class ExcelWbOpener
    {
        /// <summary>
        /// File path. 
        /// Optional if Filename is already provided.
        /// </summary>
        public string Filepath { get; set; }

        /// <summary>
        /// Opens a workbook with a given filename.
        /// Optional if Filepath is already provided(Filepath takes priority).
        /// </summary>
        public string Filename { get; set; }

        private string workingDirectory = null;
        /// <summary>
        /// If the filepath is empty (i.e. using filename), 
        /// files are opened from the WorkingDirectory with the Filename specified.
        /// </summary>
        public string WorkingDirectory 
        { 
            get => workingDirectory != null ? workingDirectory : System.IO.Directory.GetCurrentDirectory(); 
            set => workingDirectory = value; 
        }

        /// <summary>
        /// Tries to grab a window which is already open according to the filepath/filename. 
        /// This setting takes priority, so if the existing window is ready only, it shall remain read only & TryWrite will be ignored.
        /// </summary>
        public bool TryGrabExisting { get; set; } = true;


        /// <summary>
        /// If a new window is opened, shall attempt to open in Write mode.
        /// </summary>
        public bool TryWrite { get; set; } = true;

        /// <summary>
        /// Sets the application to visible if true.
        /// Hiding the window is not recommended.
        /// </summary>
        public bool ShowWindow { get; set; } = true;

        public bool Execute(out Workbook workbook, out bool isNewWindow)
        {
            if (TryGrabExisting)
            {
                bool result = GrabExistingWorkbook(out workbook);
                if (result)
                {
                    isNewWindow = false;
                    workbook.Application.Visible = ShowWindow; //this can fail if the excel workbook has a modal dialog. E.g. save as etc...
                    return true;
                }
            }

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
            
            //First try to open the Filepath, or failing that, open the WorkingDirectory + Filename
            if(Filepath != null && OpenWorkbook(Filepath, app, out workbook))
            {
                isNewWindow = true;
                return true;
            }

            if (Filename != null && OpenWorkbook(Path.Combine(WorkingDirectory,Filename), app, out workbook))
            {
                isNewWindow = true;
                return true;
            }

            workbook = null;
            isNewWindow = false;
            return false;
        }

        private bool GrabExistingWorkbook(out Workbook workbook)
        {
            //fileNameFromFilepathWoExtension tells this method to search for this filename without extension if all other searches fail.
            //This may be required if the workbook has not yet been saved (i.e. excel file does not yet have extension).
            //If this is set to null, this search is not required.
            string fileNameFromFilepathWoExtension = null;

            if (Filepath != null)
            {
                fileNameFromFilepathWoExtension = Path.GetFileNameWithoutExtension(Filepath);
                if (fileNameFromFilepathWoExtension == Filename || fileNameFromFilepathWoExtension == Filepath) fileNameFromFilepathWoExtension = null;

                foreach (var wb in WorkbookHelpers.GetOpenWorkbooks())
                {
                    if (wb.FullName == Filepath)
                    {
                        workbook = wb;
                        return true;
                    }
                }
            }

            if (Filename != null)
            {
                //
                if (fileNameFromFilepathWoExtension == Filename || fileNameFromFilepathWoExtension == Filepath) fileNameFromFilepathWoExtension = null;

                foreach (var wb in WorkbookHelpers.GetOpenWorkbooks())
                {
                    if (wb.Name == Filename)
                    {
                        workbook = wb;
                        return true;
                    }
                }
            }

            if (fileNameFromFilepathWoExtension != null)
            {
                foreach (var wb in WorkbookHelpers.GetOpenWorkbooks())
                {
                    if (wb.Name == fileNameFromFilepathWoExtension)
                    {
                        workbook = wb;
                        return true;
                    }
                }
            }

            workbook = null;
            return false;
        }

        private bool OpenWorkbook(string path, Application app, out Workbook workbook)
        {
            //ReadOnly: Optional Object. True to open the workbook in read-only mode.
            //Notify: Optional Object. If the file cannot be opened in read/write mode, this argument is True to add the file to the file notification list. Microsoft Excel will open the file as read-only, poll the file notification list, and then notify the user when the file becomes available. If this argument is False or omitted, no notification is requested, and any attempts to open an unavailable file will fail.

            if (TryWrite)
            {
                try
                {
                    workbook = app.Workbooks.Open(Filepath, ReadOnly: false, Notify: true);
                    return true;
                }
                catch (Exception e) { }
            }

            //Open read only if failed to open with write mode.
            try
            {
                workbook = app.Workbooks.Open(Filepath, ReadOnly: true, Notify: true);
                return true;
            }
            catch (Exception e) { }

            workbook = null;
            return false;
        }
    }


}
