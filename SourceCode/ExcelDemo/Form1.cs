using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ExcelDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void writeExcelButton_Click(object sender, EventArgs e)
        {
            string currentPath = Directory.GetCurrentDirectory();
            WriteExcel(currentPath + "\\results.xlsx");

            MessageBox.Show("Success!");
        }


        public void WriteExcel(string filename)
        {
            //new an excel object
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            if (excelApp == null)
            {
                // if equal null means EXCEL is not installed.
                MessageBox.Show("Excel is not properly installed!");
                return;
            }

            // open a workbook,if not exist, create a new one
            Microsoft.Office.Interop.Excel.Workbook workBook;
            if (File.Exists(filename))
            {
                workBook = excelApp.Workbooks.Open(filename, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            }
            else
            {
                workBook = excelApp.Workbooks.Add(true);
            }

            //new a worksheet
            var workSheet = workBook.ActiveSheet as Microsoft.Office.Interop.Excel.Worksheet;

            //write data
            workSheet = (Microsoft.Office.Interop.Excel.Worksheet)workBook.Worksheets.get_Item(1);//获得第i个sheet，准备写入

            workSheet.Cells[1, 1] = "1,1";
            workSheet.Cells[1, 2] = "1,2";
            workSheet.Cells[1, 3] = "1,3";
            workSheet.Cells[1, 4] = 4;
            workSheet.Cells[1, 5] = false;
            workSheet.Cells[1, 6] = 1.01;

            //set visible the Excel will run in background
            excelApp.Visible = false;
            //set false the alerts will not display
            excelApp.DisplayAlerts = false;

            //workBook.SaveAs(filename, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
            workBook.SaveAs(filename);
            workBook.Close(false, Missing.Value, Missing.Value);

            //quit and clean up objects
            excelApp.Quit();
            workSheet = null;
            workBook = null;
            excelApp = null;
            GC.Collect();
        }

    }
}

