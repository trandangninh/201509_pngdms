using System;
using System.Data;
using System.IO;
using System.Linq;
using OfficeOpenXml;

namespace Service.Common
{
    public class ExcellService : IExcellService
    {
        public DataTable ReadExcellToDataTable(string fileName, Boolean hasHeader)
        {
            try
            {
                var file = new FileInfo(fileName);
                using (var excel = new ExcelPackage(file))
                {
                    var tbl = new DataTable();
                    var ws = excel.Workbook.Worksheets.First();
                   // var hasHeader = true;  // adjust accordingly
                    // add DataColumns to DataTable
                    foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                        tbl.Columns.Add(hasHeader ? firstRowCell.Text
                            : String.Format("Column {0}", firstRowCell.Start.Column));

                    // add DataRows to DataTable
                    int startRow = hasHeader ? 2 : 1;
                    for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                        var row = tbl.NewRow();
                        foreach (var cell in wsRow)
                        {
                             row[cell.Start.Column - 1] = cell.Text;
                            
                        }
                           tbl.Rows.Add(row.ItemArray);
                    }
                    return tbl;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
