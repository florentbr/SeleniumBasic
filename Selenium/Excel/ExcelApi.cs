using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace Selenium.Excel {

    class ExcelApi {

        static CultureInfo CULTURE_US = new CultureInfo("en-US");

        /// <summary>
        /// Gets a range object from an existing instance or new one
        ///  if source is null, then adds a new worksheet and return the used range
        ///  if source is Range, then return the range
        ///  if source is Worksheet, then return the used range
        ///  if source is string, then parse the source and return the range
        /// </summary>
        /// <param name="source">Reference, range or worksheet</param>
        /// <returns>Range</returns>
        public static IRange GetRange(object source) {
            //Get a running instance of Excel or create one
            IExcel excel = ExcelApi.GetInstance();

            //Handle null source
            IWorksheets worksheets = excel.Worksheets;
            if (source == null) {
                //Add a new sheet to the current workbook
                IWorksheet lastWorksheet = worksheets[worksheets.Count];
                IWorksheet newWorksheet = worksheets.Add(Type.Missing, lastWorksheet, Type.Missing, Type.Missing);
                return newWorksheet.UsedRange;
            }

            //Handle source as address
            if (source is string) {
                //try parse worksheet name
                var srctxt = source as string;
                if (srctxt != null && srctxt.IndexOf('!') == -1 && srctxt.IndexOf(':') == -1) {
                    try {
                        return ((IWorksheet)worksheets[source]).UsedRange;
                    } catch { }
                }
                //try parse range name
                try {
                    return excel[source, Type.Missing];
                } catch {
                    throw new ExcelSourceError("Failed to resolve the source:\n{0}", source);
                }
            }

            //Handle source as Worksheet
            IWorksheet worksheet = source as IWorksheet;
            if (worksheet != null)
                return worksheet.UsedRange;

            //Handle source as Range
            IRange range = source as IRange;
            if (range != null)
                return range;

            throw new ExcelSourceError("Invalid type of source: {0}", source.GetType().Name);
        }

        /// <summary>
        /// Returns an active instance of Excel or a new one.
        /// </summary>
        /// <returns></returns>
        public static IExcel GetInstance() {

            //Set a compatible culture
            Thread.CurrentThread.CurrentCulture = CULTURE_US;

            //Get the running instance of Excel or create one if there isn't.
            bool isCreatedApp = false;
            Object app = null;
            Guid excelguid = typeof(Excel).GUID;
            NativeMethods.GetActiveObject(ref excelguid, IntPtr.Zero, out app);
            if (app == null) {
                try {
                    app = new Excel();
                    isCreatedApp = true;
                } catch {
                    throw new ExcelError("Failed to launch Excel.");
                }
            }

            //Cast to interface
            IExcel excel = app as IExcel;
            if (excel == null)
                throw new ExcelError("Failed to interface with Excel.");

            //Adds a worksheet if it's an empty instance of Excel
            if (isCreatedApp) {
                IWorkbook workbook = excel.Workbooks.Add();
                workbook.Worksheets.Add();
                excel.Visible = true;
            }

            return excel;
        }


        class NativeMethods {

            const String OLE32 = "oleaut32.dll";

            [DllImport(OLE32, PreserveSig = true)]
            public static extern void GetActiveObject(ref Guid rclsid, IntPtr reserved
                , [MarshalAs(UnmanagedType.Interface)] out Object ppunk);

        }


        #region Errors

        /// <summary>
        /// Excel error
        /// </summary>
        public class ExcelError : SeleniumError {
            internal ExcelError(string message, params object[] args)
                : base(message, args) { }
        }

        /// <summary>
        /// Error occuring when a source is incorrect.
        /// </summary>
        public class ExcelSourceError : ExcelError {
            internal ExcelSourceError(string message, params object[] args)
                : base(message, args) { }
        }

        #endregion

    }

}
