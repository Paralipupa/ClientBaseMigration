using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace ClientBaseMigration
{
    public class Sheet
    {
        public string Name { get; set; }
        public List<List<string>> Cells { get; set; }

        public Sheet()
        {
            Cells = new List<List<string>>();
        }
    }

    public class ExcelData
    {
        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);

        static Process GetExcelProcess(Excel.Application excelApp)
        {
            GetWindowThreadProcessId(excelApp.Hwnd, out int id);
            return Process.GetProcessById(id);
        }

        public Sheet[] Sheets { get; set; }

        public ExcelData()
        {
            Sheets = new Sheet[20];
        }

        public Sheet this[int index]
        {
            get { return Sheets[index]; }
            set { Sheets[index] = value; }
        }

        public Sheet this[string name]
        {
            get
            {
                Sheet sheet = null;
                foreach (var p in Sheets)
                {
                    if (p?.Name == name)
                    {
                        sheet = p;
                        break;
                    }
                }
                return sheet;
            }
        }


        public bool Read(string filename)
        {
            bool bDone = false;

            Excel.Application a = new Excel.Application();
            Excel.Workbook wb = null;
            Excel.Worksheet currentSheet = null;
            Process appProcess = GetExcelProcess(a);

            try
            {
                wb = a.Workbooks.Open(filename);
                int k = wb.Worksheets.Count < 20 ? wb.Worksheets.Count : 20;

                //k = 1;
                for (int i = 0; i < k; i++)
                {
                    currentSheet = wb.Worksheets[i + 1];
                    Sheet sheet = new Sheet
                    {
                        Name = currentSheet.Name
                    };

                    var lastCell = currentSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell);//последнюю ячейку
                                                                                                        // размеры базы
                    int lastColumn = (int)lastCell.Column;
                    int lastRow = (int)lastCell.Row;
                    lastCell = null;

                    for (int row = 1; row <= lastRow+1; row++)
                    {
                        List<string> cols = new List<string>();
                        for (int col = 1; col <= lastColumn; col++)
                        {
                            cols.Add(currentSheet.Cells[row, col].Text.ToString());
                        }
                        sheet.Cells.Add(cols);
                    }

                    Sheets[i] = sheet;
                }
                bDone = true;
            }

            catch (Exception ex)
            {
                ClientBase.DialogService.ShowMessage(ex.Message);
            }

            finally
            {
                currentSheet = null;
                wb?.Close(false,false,false);
                wb = null;
                a?.Quit();
                a = null;
                appProcess.Kill();
                GC.Collect();

            }

            return bDone;

        }

    }
}
