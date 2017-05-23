using Entities.Domain.ScMeasures;
using RepositoryPattern.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Utils.Caching;
using System.Data.Entity;
using System.IO;
using OfficeOpenXml;
using Service.Suppliers;
using OfficeOpenXml.Style;
using System.Drawing;
using Service.QualityAlerts;
using Entities.Domain.QualityAlerts;

namespace Service.ScMeasures
{
    public class MqsMeasureService : BaseService<MqsMeasure>, IMqsMeasureService
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        /// </remarks>
        private const string MQSMEASURE_BY_SUPPLIERID_SCMEASUREID_TIME = "PG.MqsMeasure.MqsMeasureBySupplierIdAndScMeasureIdAndTime-{0}-{1}-{2}";
        protected override string PatternKey
        {
            get { return "PG.MqsMeasure."; }
        }

        private readonly ICacheManager _cacheManager;
        private readonly ISupplierService _supplierService;
        private readonly IScMeasureService _scMeasureService;
        private readonly IRepositoryAsync<ScMeasure> _scMeasureRepositoryAsync;
        private readonly IRepositoryAsync<MqsMeasure> _mqsMeasureRepositoryAsync;
        private readonly ISubColumnFormulaService _subColumnFormulaService;

        public MqsMeasureService(ICacheManager cacheManager,
            IRepositoryAsync<MqsMeasure> mqsMeasureRepositoryAsync,
            ISupplierService supplierService,
            IScMeasureService scMeasureService,
            IRepositoryAsync<ScMeasure> scMeasureRepositoryAsync,
            ISubColumnFormulaService subColumnFormulaService)
            : base(mqsMeasureRepositoryAsync, cacheManager)
        {
            _mqsMeasureRepositoryAsync = mqsMeasureRepositoryAsync;
            _cacheManager = cacheManager;
            _supplierService = supplierService;
            _scMeasureService = scMeasureService;
            _scMeasureRepositoryAsync = scMeasureRepositoryAsync;
            _subColumnFormulaService = subColumnFormulaService;
        }

        public void ImportMqsMeasure(MemoryStream stream, out string message)
        {
            message = "";
            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    if (package.Workbook.Worksheets.Count <= 0)
                        message = "Your Excel file does not contain any work sheets";
                    else
                    {
                        var worksheet = package.Workbook.Worksheets[1];
                        var year = 0;
                        int.TryParse(CellValue(worksheet, "B1"), out year);
                        var month = 0;
                        int.TryParse(CellValue(worksheet, "D1"), out month);

                        if (year == 0 || month == 0)
                        {
                            message = "Year or Month is not valid. Please check excel file again.";
                            return;
                        }

                        var colIndex = 3;
                        var rowIndex = 3;
                        var wsCol = worksheet.Cells[2, colIndex, 2, worksheet.Dimension.End.Column];
                        var wsRow = worksheet.Cells[rowIndex, 1, worksheet.Dimension.End.Row, 1];

                        //var mqsInserted = new List<MqsMeasure>();
                        foreach (var col in wsCol)
                        {
                            var supplierName = col.Value == null ? "" : col.Value.ToString();
                            var supplier = _supplierService.GetSupplierByNameAsync(supplierName).Result;
                            if (supplier == null)
                                continue;
                            rowIndex = 3;
                            foreach (var row in wsRow)
                            {
                                var measureName = row.Value == null ? "" : row.Value.ToString();
                                var measure = _scMeasureService.GetScMeasureByNameAsync(measureName).Result;
                                if (measure == null)
                                    continue;

                                //Update style
                                measure.IsBold = row.Style.Font.Bold;
                                _scMeasureService.UpdateAsync(measure).Wait();

                                var value = CellValue(worksheet, rowIndex, colIndex);

                                var mqsExisting = _mqsMeasureRepositoryAsync.Table.FirstOrDefault(m => m.ScMeasureId == measure.Id && m.SupplierId == supplier.Id && m.Date.Year == year && m.Date.Month == month);

                                if (mqsExisting != null)
                                {
                                    mqsExisting.Value = value;
                                    _mqsMeasureRepositoryAsync.UpdateAsync(mqsExisting).Wait();
                                }
                                else
                                {
                                    _mqsMeasureRepositoryAsync.InsertAsync(new MqsMeasure
                                    {
                                        SupplierId = supplier.Id,
                                        ScMeasureId = measure.Id,
                                        Value = value,
                                        Date = new DateTime(year, month, 1)
                                    }).Wait();
                                }
                                //Todo here
                                rowIndex++;
                            }
                            colIndex++;
                        }
                        //if (mqsInserted.Count != 0)
                        //    _mqsMeasureRepositoryAsync.InsertAsync(mqsInserted);
                    }
                }
            }
            catch(Exception ex)
            {
                message = ex.Message;
            }
            
        }

        public void ImportScoreCardMeasure(MemoryStream stream, out string message)
        {
            message = "";
            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    if (package.Workbook.Worksheets.Count <= 0)
                        message = "Your Excel file does not contain any work sheets";
                    else
                    {
                        var worksheet = package.Workbook.Worksheets["Score Card"];
                        //Todo: Check worksheet
                        var rowIndex = 3;
                        var rows = worksheet.Cells[rowIndex, 1, worksheet.Dimension.End.Row, 1];

                        foreach (var row in rows)
                        {
                            var measure = _scMeasureService.GetScMeasureByNameAsync(row.Value == null ? "" : row.Value.ToString()).Result;
                            if (measure == null)
                                continue;
                            measure.Formula = CellFormula(worksheet, rowIndex, 3);

                            measure.YtdCalculatedTypeId = 0;
                            if (CellFormula(worksheet, rowIndex, 2).ToLower() == YtdCalculatedType.Average.ToString().ToLower())
                                measure.YtdCalculatedType = YtdCalculatedType.Average;
                            if (CellFormula(worksheet, rowIndex, 2).ToLower() == YtdCalculatedType.LatestData.ToString().ToLower())
                                measure.YtdCalculatedType = YtdCalculatedType.LatestData;
                            if (CellFormula(worksheet, rowIndex, 2).ToLower() == YtdCalculatedType.Sum.ToString().ToLower())
                                measure.YtdCalculatedType = YtdCalculatedType.Sum;

                            _scMeasureService.UpdateAsync(measure).Wait();
                            rowIndex++;
                        }

                        //sub column
                        var subcolumnws = package.Workbook.Worksheets["Sub-Column"];
                        //Todo: Check worksheet
                        rowIndex = 3;
                        rows = subcolumnws.Cells[rowIndex, 1, subcolumnws.Dimension.End.Row, 1];

                        var subColumns = _subColumnFormulaService.GetAllAsync().Result;

                        if(rows.Count()==0&&subColumns.Count()!=0)
                        {
                            foreach(var subC in subColumns)
                                _subColumnFormulaService.DeleteAsync(subC);
                        }else
                        {
                            if (rows.Count() > 0)
                            {
                                
                                var updates = subColumns.Take(rows.Count());
                                var count = updates.Count();
                                foreach (var subC in updates)
                                {
                                    subC.Name = CellValue(subcolumnws, rowIndex, 1);
                                    subC.Formula = CellFormula(subcolumnws, rowIndex, 2);
                                    _subColumnFormulaService.UpdateAsync(subC).Wait();
                                    rowIndex++;
                                }
                                var deletes = subColumns.Skip(count);
                                foreach (var subC in deletes)
                                    _subColumnFormulaService.DeleteAsync(subC);

                                var adds = rows.Skip(count);
                                foreach (var subC in adds)
                                {
                                    var aSubC = new SubColumnFormula();
                                    aSubC.Name = CellValue(subcolumnws, rowIndex, 1);
                                    aSubC.Formula = CellFormula(subcolumnws, rowIndex, 2);
                                    _subColumnFormulaService.InsertAsync(aSubC).Wait();
                                    rowIndex++;
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }            
        }

        public void ExportMqsTemplate(Stream stream, string path)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            using (var xlPackage = new ExcelPackage(stream))
            {
                var msWorksheet = xlPackage.Workbook.Worksheets.Add("MQS measure");
                //Quality Alert Sheet
                var scMeasures = _scMeasureRepositoryAsync.Table.Where(m => m.IsImported);
                var suppliers = _supplierService.GetAllAsync().Result;

                msWorksheet.Column(1).Width = 35;

                msWorksheet.Cells[1, 1].Value = "Year";
                msWorksheet.Cells[1, 2].Value = DateTime.Now.Year;
                msWorksheet.Cells[1, 3].Value = "Month";
                msWorksheet.Cells[1, 4].Value = DateTime.Now.Month;

                HeaderStyle(msWorksheet.Cells[2, 1], "MQS measure");
                HeaderStyle(msWorksheet.Cells[2, 2], "Target");
                var col = 3;
                foreach(var supplier in suppliers)
                {
                    HeaderStyle(msWorksheet.Cells[2,col], supplier.Name);
                    col++;
                }
                var row = 3;
                foreach(var scm in scMeasures)
                {
                    msWorksheet.Cells[row, 1].Value = scm.Name;
                    if (scm.IsBold)
                        msWorksheet.Cells[row, 1].Style.Font.Bold = true;
                    row++;
                }

                if (String.IsNullOrEmpty(path))
                {
                    xlPackage.Save(); // save to excell
                }
                else
                {
                    xlPackage.SaveAs(new FileInfo(path));

                    stream.Close();
                }
            }
        }

        public Task<List<MqsMeasure>> GetMqsMeasures(DateTime date, int supplierId, int scMeasureId = 0)
        {
            var query = _mqsMeasureRepositoryAsync.Table.Where(mm => mm.Date == date && mm.SupplierId == supplierId);
            if (scMeasureId != 0)
                query = query.Where(mm => mm.ScMeasureId == scMeasureId);
            return query.ToListAsync();
        }
        public Task<List<MqsMeasure>> GetMqsMeasures(DateTime fromDate, DateTime toDate, int supplierId = 0, int scMeasureId = 0)
        {
            var query = _mqsMeasureRepositoryAsync.Table.Where(mm => mm.Date > fromDate && mm.Date < toDate);
            if(supplierId != 0)
                query = query.Where(mm => mm.SupplierId == supplierId);
            if (scMeasureId != 0)
                query = query.Where(mm => mm.ScMeasureId == scMeasureId);
            return query.ToListAsync();
        }

        private string CellValue(ExcelWorksheet worksheet,string address)
        {
            var value = worksheet.Cells[address].Value;
            return value == null ? "" : value.ToString();
        }
        private string CellValue(ExcelWorksheet worksheet, int rowIndex, int colIndex)
        {
            var value = worksheet.Cells[rowIndex,colIndex].Value;
            return value == null ? "" : value.ToString();
        }
        private string CellFormula(ExcelWorksheet worksheet, string address)
        {
            var formula = worksheet.Cells[address].Formula;
            return formula == null ? "" : formula.ToString();
        }
        private string CellFormula(ExcelWorksheet worksheet, int rowIndex, int colIndex)
        {
            var formula = worksheet.Cells[rowIndex, colIndex].Formula;
            if (string.IsNullOrEmpty(formula) && worksheet.Cells[rowIndex, colIndex].Value != null)
                formula = worksheet.Cells[rowIndex, colIndex].Value.ToString();

            return formula == null ? "" : formula.ToString();
        }
        private void HeaderStyle(ExcelRangeBase cell, string text)
        {
            cell.Value = text;
            cell.Style.Font.Size = 12;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
            cell.Style.Font.Color.SetColor(Color.White); // set color font
        }

        /// <summary>
        /// get mqsMeasure by suppplierId, scMeasureId and time
        /// </summary>
        /// <param name="supplierId"></param>
        /// <param name="scMeasureId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public Task<MqsMeasure> GetMqsMeasureBySupplierIdAndScMeasureIdAndTime(int supplierId, int scMeasureId, DateTime time)
        {
            var key = string.Format(MQSMEASURE_BY_SUPPLIERID_SCMEASUREID_TIME, supplierId, scMeasureId, time);
            return _cacheManager.Get(key, () => _mqsMeasureRepositoryAsync.Table.FirstOrDefaultAsync(x => x.SupplierId == supplierId && x.ScMeasureId == scMeasureId
                                    && x.Date.Year == time.Year && x.Date.Month == time.Month));
        }
    }
}
