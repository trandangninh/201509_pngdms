using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Domain.ScoreCards;
using Service.ScMeasures;
using Entities.Domain.ScMeasures;
using RepositoryPattern.Repositories;
using Entities.Domain.QualityAlerts;
using Entities.Domain.Suppliers;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.IO;
using System.Drawing;
using Service.QualityAlerts;
using Service.Suppliers;
using Utils;
using System.Globalization;

namespace Service.ScoreCards
{
    public class ScoreCardService : IScoreCardService
    {
        private IRepositoryAsync<ScMeasure> _scMeasureRepository;
        private IRepositoryAsync<MqsMeasure> _mqsMeasureRepository;
        private IScMeasureService _scMeasureService;
        private IRepositoryAsync<QualityAlert> _qualityAlertRepository;
        private IRepositoryAsync<Supplier> _supplierRepository;
        private ISupplierService _supplierService;
        private IQualityAlertService _qualityAlertService;
        private IMqsMeasureService _mqsMeasureService;
        private IScMeasureTargetService _scMeasureTargetService;
        private ISubColumnFormulaService _subColumnService;
        public ScoreCardService(
            IRepositoryAsync<ScMeasure> scMeasureRepository,
            IRepositoryAsync<MqsMeasure> mqsMeasureRepository,
            IRepositoryAsync<QualityAlert> qualityAlertRepository,
            IRepositoryAsync<Supplier> supplierRepository,
            IQualityAlertService qualityAlertService,
            IMqsMeasureService mqsMeasureService,
            IScMeasureService scMeasureService,
            ISupplierService supplierService,
            IScMeasureTargetService scMeasureTargetServices,
            ISubColumnFormulaService subColumnService)
        {
            _scMeasureRepository = scMeasureRepository;
            _mqsMeasureRepository = mqsMeasureRepository;
            _qualityAlertRepository = qualityAlertRepository;
            _supplierRepository = supplierRepository;
            _qualityAlertService = qualityAlertService;
            _mqsMeasureService = mqsMeasureService;
            _scMeasureService = scMeasureService;
            _supplierService = supplierService;
            _scMeasureTargetService = scMeasureTargetServices;
            _subColumnService = subColumnService;
        }
        
        public List<ScoreCardObject> GetScoreCardData(int year, int supplierId, DateTime? fromDate, DateTime? toDate)
        {
            if (toDate.HasValue && toDate.Value > DateTime.Now.AddDays(1))
                toDate = DateTime.Now.AddDays(1);

            var scMeasures = _scMeasureRepository.Table.Where(m => m.IsDisplay).ToList();

            var result = scMeasures.Select(scm => new ScoreCardObject
            {
                MsqMeasure = scm.Name,
                ScMeasureId = scm.Id,
                IsBold = scm.IsBold
            }).ToList();
            var result2 = scMeasures.Select(scm => new ScoreCardObject
            {
                MsqMeasure = scm.Name,
                ScMeasureId = scm.Id
            }).ToList();
            //Get supplier
            var supplier = _supplierService.GetByIdAsync(supplierId).Result;
            //Get all quality alert in year from 1st July to 30th Jun of supplier
            var quanlityAlerts = _qualityAlertService.SearchQualityAlertObjectAsync(startDate: new DateTime(year - 1, 7, 1), endDate: new DateTime(year, 7, 1), supplierIds: new List<int>() { supplierId }).Result;
            //Get all MQS measure
            var mqsMeasures = _scMeasureRepository.Table.Where(m => m.IsImported).OrderBy(m => m.DisplayOrder).ToList();
            //Get all sub-Column to add to Quality alert table in excel sheeet
            var subColumns = _subColumnService.GetAllAsync().Result;
            //get all scoreCardItems of supplier
            var scoreCardItems = _mqsMeasureRepository.Table.Where(x => x.SupplierId == supplierId).ToList();
            //Get all score card target of supplier
            var scMeasureTargets = _scMeasureTargetService.GetAllScMeasureTargetBySupplierId(supplier.Id).Result;
            var listTask = new List<Task>();

            DateTime fStartDate = new DateTime(year - 1, 7, 1);
            DateTime fEndDate = DateTime.Now.AddDays(1);

            do
            {
                DateTime tDate;
                if (fStartDate.Year == fEndDate.Year && fStartDate.Month == fEndDate.Month)
                    tDate = fEndDate;
                else
                    tDate = fStartDate.AddMonths(1).AddDays(1 - fStartDate.Day);

                listTask.Add(CreateScoreCardAsync(fStartDate, tDate, result, quanlityAlerts, scMeasures, mqsMeasures, scoreCardItems, scMeasureTargets, subColumns));
                fStartDate = tDate;
            } while (fStartDate < fEndDate);


            if(fromDate.HasValue && toDate.HasValue && fromDate.Value.Year == toDate.Value.Year && fromDate.Value.Month == toDate.Value.Month)
            {
                listTask.Add(CreateScoreCardAsync(fromDate.Value, toDate.Value, result2, quanlityAlerts, scMeasures, mqsMeasures, scoreCardItems, scMeasureTargets, subColumns));

                Task.WaitAll(listTask.ToArray());

                YtdCalculate(result, scMeasures);

                for (int j = 0; j < scMeasures.Count; j++)
                {
                    result[j].Data[(fromDate.Value.Month + 5) % 12] = result2[j].Data[(fromDate.Value.Month + 5) % 12];
                }
                return result;
            }

            if(fromDate.HasValue && fromDate.Value != new DateTime(year - 1, 7, 1))
            {
                var tDate = fromDate.Value.AddMonths(1).AddDays(1 - fromDate.Value.Day);
                listTask.Add(CreateScoreCardAsync(fromDate.Value, tDate, result2, quanlityAlerts, scMeasures, mqsMeasures, scoreCardItems, scMeasureTargets, subColumns));
            }
            if (toDate.HasValue && toDate.Value != DateTime.Now.Date)
            {
                var sDate = toDate.Value.AddDays(1 - toDate.Value.Day);
                listTask.Add(CreateScoreCardAsync(sDate, toDate.Value, result2, quanlityAlerts, scMeasures, mqsMeasures, scoreCardItems, scMeasureTargets, subColumns));
            }
            //Calculate for every month from July to Jun

            Task.WaitAll(listTask.ToArray());

            YtdCalculate(result, scMeasures);

            if (fromDate.HasValue && fromDate.Value != new DateTime(year - 1, 7, 1))
            {
                for (int j = 0; j < scMeasures.Count; j++)
                {
                    result[j].Data[(fromDate.Value.Month + 5) % 12] = result2[j].Data[(fromDate.Value.Month + 5) % 12];
                }
            }
            if (toDate.HasValue && toDate.Value != DateTime.Now.Date)
            {
                for (int j = 0; j < scMeasures.Count; j++)
                {
                    result[j].Data[(toDate.Value.Month + 5) % 12] = result2[j].Data[(toDate.Value.Month + 5) % 12];
                }
            }
            return result;
        }

        private void YtdCalculate(List<ScoreCardObject> result, List<ScMeasure> scMeasures)
        {
            var i = 0;
            foreach (var scm in scMeasures)
            {
                if (scm.YtdCalculatedType == YtdCalculatedType.LatestData)
                {
                    var latest = result[i].Data.LastOrDefault(d => d != null);
                    if (latest != null)
                        result[i].Ytd = latest?.ToString("0.##");
                }
                else
                if (scm.YtdCalculatedType == YtdCalculatedType.Sum)
                {
                    result[i].Ytd = result[i].Data.Sum()?.ToString("0.##");
                }
                else
                if (scm.YtdCalculatedType == YtdCalculatedType.Average)
                {
                    result[i].Ytd = result[i].Data.Where(d => d != null && d != 0).Average()?.ToString("0.##");
                }
                i++;
            }
        }

        public void ExportScoreCardTemplate(Stream stream, int year, string path)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            using (var xlPackage = new ExcelPackage(stream))
            {
                var qaWorksheet = xlPackage.Workbook.Worksheets.Add("Quality Alert");
                var smWorksheet = xlPackage.Workbook.Worksheets.Add("Supplier Measure");
                var scWorksheet = xlPackage.Workbook.Worksheets.Add("Score Card");
                var subCWorksheet = xlPackage.Workbook.Worksheets.Add("Sub-Column");
                var supplier = _supplierRepository.Table.FirstOrDefault();
                //Quality Alert Sheet
                var quanlityAlerts = _qualityAlertService.SearchQualityAlertObjectAsync(pageSize: 10).Result.ToList();
                var subColumns = _subColumnService.GetAllAsync().Result;
                CreateQualityAlertWorkSheet(qaWorksheet, quanlityAlerts.ToList(), subColumns.ToList());

                //MQS Measure
                //var mqsMeasures = _mqsMeasureService.GetMqsMeasures(new DateTime(year, 9, 1), supplier.Id).Result;
                var mqsMeasure = _mqsMeasureRepository.Table.Where(m => m.SupplierId == supplier.Id).OrderByDescending(m => m.Date).FirstOrDefault();
                var mqsMeasures = _scMeasureRepository.Table.Where(m => m.IsImported).OrderBy(m => m.DisplayOrder).ToList();
                var scoreCardItems = _mqsMeasureRepository.Table.Where(x => x.SupplierId == supplier.Id).ToList();
                var scMeasureTargets = _scMeasureTargetService.GetAllScMeasureTargetBySupplierId(supplier.Id).Result;
                CreateMqsMeasureWorksheet(smWorksheet, mqsMeasures, scoreCardItems, scMeasureTargets, new DateTime(mqsMeasure != null ? mqsMeasure.Date.Year : year, mqsMeasure != null ? mqsMeasure.Date.Month : 1,1));
                
                //Score Card
                var scMeasures = _scMeasureRepository.Table.Where(m => m.IsDisplay).ToList();
                CreateScoreCardWorksheet(scWorksheet, scMeasures, null, DateTime.Now);
                //Sub Columns
                CreateSubColumnWorksheet(subCWorksheet, subColumns);
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

        /// <summary>
        /// Export multi Score Card template
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="year"></param>
        /// <param name="listScoreCardPerYear"></param>
        /// <param name="listMonthId"></param>
        /// <param name="path"></param>
        public void ExportMultiScoreCardTemplate(MemoryStream stream, int year, List<ScoreCardsPerYear> listScoreCardPerYear, List<int> listMonthId, string path)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            using (var xlPackage = new ExcelPackage(stream))
            {
                foreach (var scoreCardPerYear in listScoreCardPerYear)
                {
                    var newWorkSheet = xlPackage.Workbook.Worksheets.Add(scoreCardPerYear.SupplierName);
                    CreateScoreCardPerYearWordSheet(newWorkSheet, year, scoreCardPerYear, listMonthId);
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

        private void CreateScoreCardPerYearWordSheet(ExcelWorksheet worksheet, int year, ScoreCardsPerYear scoreCardPerYear, List<int> listMonthId)
        {
            //new list monthId when it null
            if (listMonthId == null || listMonthId.Count == 0)
            {
                listMonthId = new List<int>();
                for (int i = 0; i <= 11; i++)
                {
                    listMonthId.Add(i);
                }
            }
            else
            {
                //order accending
                listMonthId = listMonthId.OrderBy(x => x).ToList();
            }

            var monthHeader = new List<string> { "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", "Jan", "Feb", "Mar", "Apr", "May", "Jun" };

            var colHeader = new List<string> { "C2", "D2", "E2", "F2", "G2", "H2", "I2", "J2", "K2", "L2", "M2", "N2" };

            var header = new Dictionary<string, string>
                {
                    {"A2", "MSQ Measure"},
                    {"B2", "YTD"}
                };

            //Add header
            int count = 0;
            foreach (var monthId in listMonthId)
            {
                var month = monthHeader[monthId];
                header.Add(colHeader[count], monthHeader[monthId]);
                count++;
            }

            //create header
            foreach (var cell in header)
            {
                HeaderStyle(worksheet.Cells[cell.Key], cell.Value);
            }

            worksheet.Cells[1, 1].Value = "Year";
            worksheet.Cells[1, 2].Value = year;

            #region format header width

            count = 3;
            worksheet.Column(1).Width = 40;
            worksheet.Column(2).Width = 20;
            foreach (var monthId in listMonthId)
            {
                worksheet.Column(count).Width = 10;
                count++;
            }


            #endregion


            #region fill data
            for (int i = 0; i < scoreCardPerYear.ScoreCards.Count; i++)
            {
                worksheet.Cells[i + 3, 1].Value = scoreCardPerYear.ScoreCards[i].MsqMeasure;
                worksheet.Cells[i + 3, 2].Value = scoreCardPerYear.ScoreCards[i].Ytd;
                int index = 3;
                var valueFormat = scoreCardPerYear.ScoreCards[i].MsqMeasure.Contains("%") ? "{0}%" : "{0}";
                foreach (var monthId in listMonthId)
                {
                    worksheet.Cells[i + 3, index].Value = String.Format(valueFormat,scoreCardPerYear.ScoreCards[i].Data[monthId]);
                    index++;
                }
            }
            #endregion
        }

        #region private methods
        private Task CreateScoreCardAsync(DateTime startDate, DateTime endDate, List<ScoreCardObject> result, IPagedList<QualityAlertFullObject> quanlityAlerts, List<ScMeasure> scMeasures, IList<ScMeasure> mqsMeasures, List<MqsMeasure> scoreCardItems, IList<ScMeasureTarget> scMeasureTargets, IList<SubColumnFormula> subColumns)
        {
            return Task.Run(() =>
            {
                using (var xlPackage = new ExcelPackage())
                {
                    var qaWorksheet = xlPackage.Workbook.Worksheets.Add("Quality Alert");
                    var smWorksheet = xlPackage.Workbook.Worksheets.Add("Supplier Measure");
                    var scWorksheet = xlPackage.Workbook.Worksheets.Add("Score Card");

                    var qas = quanlityAlerts.Where(x => x.AlertDateTime >= startDate && x.AlertDateTime < endDate).ToList();

                    //Quality alert sheet
                    CreateQualityAlertWorkSheet(qaWorksheet, qas, subColumns);

                    //MQS Measure sheet
                    CreateMqsMeasureWorksheet(smWorksheet, mqsMeasures, scoreCardItems, scMeasureTargets, startDate);

                    //Score Card sheet
                    CreateScoreCardWorksheet(scWorksheet, scMeasures, scoreCardItems, startDate);
                    for (int j = 0; j < scMeasures.Count; j++)
                    {
                        float? value = null;
                        var cellValue = scWorksheet.Cells[j + 3, 3].Value;

                        //get formula of cell has "enter" or "formular" or null 
                        var scMeasureFormula = scMeasures[j].Formula;
                        result[j].AvailableEdit = false;
                        if (scMeasureFormula != null && scMeasureFormula.ToString().ToLower() == "manualedit")
                        {
                            result[j].AvailableEdit = true;
                        }

                        if(cellValue != null && !string.IsNullOrEmpty(cellValue.ToString()))
                        {
                            value = 0.0f;
                            var v = 0.0f;
                            float.TryParse(cellValue.ToString(), out v);
                            value = v;
                        }
                        result[j].Data[(startDate.Month + 5) % 12] = value;
                    }
                }
            });
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

        private void CreateQualityAlertWorkSheet(ExcelWorksheet worksheet, List<QualityAlertFullObject> qualityAlerts, IList<SubColumnFormula> subColumns)
        {
            #region create headers
            var header = new Dictionary<string, string>
                {
                    { "A2", "STT Alert"},
                    { "B2", "Alert Date"},
                    { "C2", "Found by"},
                    { "D2", "Line"},
                    { "E2", "Machine"},
                    { "F2", "Description"},
                    { "G2", "Immediate Action"},
                    { "H2", "GCAS"},
                    { "I2", "SAP Lot"},
                    { "J2", "Supplier Lot"},
                    { "K2", "Supplier name"},
                    { "L2", "Supplier location"},
                    { "M2", "Material name"},
                    { "N2", "Blocked Qty"},
                    { "O2", "Unit"},
                    { "P2", "Owner"},
                    { "Q2", "Follow up action"},
                    { "R2", "When"},
                    { "S2", "Status"},
                    { "T2", "Classification"},
                    { "U2", "Category"},
                    { "V2", "Complaint Type"},
                    { "W2", "Classification Defect"},
                    { "X2", "Quantity Return"},
                    { "Y2", "Defect Repeat"},
                    { "Z2", "Supplier Reply Date"},
                    //{ "Z2", "Cost Impacted"},
                    { "AA2", "Defected Qty"},
                    { "AB2", "%PR Loss"},
                    { "AC2", "#Stop"},
                    { "AD2", "Down Time"},
                    { "AE2", "Date Informed To Supplier"},
                    { "AF2", "Effort loss (PGer)"},
                    { "AG2", "Effort loss (Contractor)"}
                };

            foreach (var cell in header)
            {
                HeaderStyle(worksheet.Cells[cell.Key], cell.Value);
            }


            worksheet.Column(1).Width = 10;
            worksheet.Column(2).Width = 20;
            worksheet.Column(3).Width = 10;
            worksheet.Column(4).Width = 10;
            worksheet.Column(5).Width = 15;
            worksheet.Column(6).Width = 30;
            worksheet.Column(7).Width = 30;
            worksheet.Column(8).Width = 20;
            worksheet.Column(9).Width = 15;
            worksheet.Column(10).Width = 15;
            worksheet.Column(11).Width = 15;
            worksheet.Column(12).Width = 20;
            worksheet.Column(13).Width = 20;
            worksheet.Column(14).Width = 15;
            worksheet.Column(15).Width = 15;
            worksheet.Column(16).Width = 15;
            worksheet.Column(17).Width = 20;
            worksheet.Column(18).Width = 15;
            worksheet.Column(19).Width = 15;
            worksheet.Column(20).Width = 15;
            worksheet.Column(21).Width = 15;
            worksheet.Column(22).Width = 20;
            worksheet.Column(23).Width = 25;
            worksheet.Column(24).Width = 25;
            worksheet.Column(25).Width = 20;
            worksheet.Column(26).Width = 25;
            worksheet.Column(27).Width = 15;
            worksheet.Column(28).Width = 15;
            worksheet.Column(29).Width = 15;
            worksheet.Column(30).Width = 20;
            worksheet.Column(31).Width = 25;
            worksheet.Column(32).Width = 25;
            worksheet.Column(33).Width = 25;

            //worksheet.Cells[2, 1, qualityAlerts.Count + 1, 28].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

            #endregion

            #region create  content for excell           

            var newDate = new DateTime();

            for (int i = 0; i < qualityAlerts.Count; i++)
            {
                worksheet.Cells[i + 3, 1].Value = i + 1;
                if (qualityAlerts[i].AlertDateTime != new DateTime())
                {
                    worksheet.Cells[i + 3, 2].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    worksheet.Cells[i + 3, 2].Value = qualityAlerts[i].AlertDateTime;
                }
                worksheet.Cells[i + 3, 3].Value = qualityAlerts[i].UserNameCreated;
                worksheet.Cells[i + 3, 4].Value = qualityAlerts[i].LineName;
                worksheet.Cells[i + 3, 5].Value = qualityAlerts[i].Machine;
                worksheet.Cells[i + 3, 6].Value = qualityAlerts[i].Detail;
                worksheet.Cells[i + 3, 7].Value = qualityAlerts[i].Action;
                worksheet.Cells[i + 3, 8].Value = qualityAlerts[i].GCAS;
                worksheet.Cells[i + 3, 9].Value = qualityAlerts[i].SAPLot;
                worksheet.Cells[i + 3, 10].Value = qualityAlerts[i].SupplierLot;
                worksheet.Cells[i + 3, 11].Value = qualityAlerts[i].SupplierName;
                worksheet.Cells[i + 3, 12].Value = qualityAlerts[i].SupplierLocation == 1 ? "Local" : "External";
                worksheet.Cells[i + 3, 13].Value = qualityAlerts[i].Material;
                worksheet.Cells[i + 3, 14].Value = qualityAlerts[i].NumBlock;
                worksheet.Cells[i + 3, 15].Value = qualityAlerts[i].Unit;
                worksheet.Cells[i + 3, 16].Value = qualityAlerts[i].OwnerName;
                worksheet.Cells[i + 3, 17].Value = qualityAlerts[i].FollowUpAction;
                if (qualityAlerts[i].When != new DateTime())
                {
                    worksheet.Cells[i+3,18].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    worksheet.Cells[i + 3, 18].Value = qualityAlerts[i].When;
                }
                worksheet.Cells[i + 3, 19].Value = qualityAlerts[i].QualityAlertStatus.ToString();
                worksheet.Cells[i + 3, 20].Value = qualityAlerts[i].ClassificationName;
                worksheet.Cells[i + 3, 21].Value = qualityAlerts[i].CategoryName;
                if (qualityAlerts[i].ComplaintTypeId != 0)
                    worksheet.Cells[i + 3, 22].Value = qualityAlerts[i].ComplaintType.ToString();
                worksheet.Cells[i + 3, 23].Value = qualityAlerts[i].ClassificationDefectName;
                worksheet.Cells[i + 3, 24].Value = qualityAlerts[i].QuantityReturn;
                if (qualityAlerts[i].DefectRepeatId != 0)
                    worksheet.Cells[i + 3, 25].Value = qualityAlerts[i].DefectRepeat.ToString();
                if (qualityAlerts[i].SupplierReplyDate != newDate)
                {
                    worksheet.Cells[i + 3, 26].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    worksheet.Cells[i + 3, 26].Value = qualityAlerts[i].SupplierReplyDate;
                }
                worksheet.Cells[i + 3, 27].Value = qualityAlerts[i].DefectedQty;
                worksheet.Cells[i + 3, 28].Value = qualityAlerts[i].PRLossPercent;
                worksheet.Cells[i + 3, 29].Value = qualityAlerts[i].NumStop;
                worksheet.Cells[i + 3, 30].Value = qualityAlerts[i].DownTime;
                if (qualityAlerts[i].InformedToSupplierDate != newDate)
                {
                    worksheet.Cells[i + 3, 31].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    worksheet.Cells[i + 3, 31].Value = qualityAlerts[i].InformedToSupplierDate;
                }
                worksheet.Cells[i + 3, 32].Value = qualityAlerts[i].PGerEffortLoss;
                worksheet.Cells[i + 3, 33].Value = qualityAlerts[i].ContractorEffortLoss;
            }

            var nextColumnIndex = 34;
            if (qualityAlerts.Count() != 0)
                foreach (var subC in subColumns)
                {
                    HeaderStyle(worksheet.Cells[2, nextColumnIndex], subC.Name);
                    worksheet.Cells[3, nextColumnIndex, qualityAlerts.Count() + 2, nextColumnIndex].Formula = subC.Formula;
                    try
                    {
                        worksheet.Cells[3, nextColumnIndex, qualityAlerts.Count() + 2, nextColumnIndex].Calculate();
                    }catch
                    {

                    }
                    nextColumnIndex++;
                }
            #endregion
        }

        private void CreateMqsMeasureWorksheet(ExcelWorksheet worksheet, IList<ScMeasure> scMeasures, IList<MqsMeasure> scoreCardItems, IList<ScMeasureTarget> scMeasureTargets, DateTime date)
        {
            var header = new Dictionary<string, string>
                {
                    { "A2", "MQS measure"},
                    { "B2", "Target"},
                    { "C2", "Supplier"}
                };

            foreach (var cell in header)
            {
                HeaderStyle(worksheet.Cells[cell.Key], cell.Value);
            }
            worksheet.Column(1).Width = 50;
            worksheet.Column(2).Width = 15;
            worksheet.Column(3).Width = 15;
            
            var i = 0;
            foreach (var scm in scMeasures)
            {
                var mqsMeasure = scoreCardItems.FirstOrDefault(m => m.ScMeasureId == scm.Id && m.Date.Year == date.Year && m.Date.Month == date.Month);

                worksheet.Cells[i + 3, 1].Value = scm.Name;

                //get target from scmeasure target table
                var target = scMeasureTargets.FirstOrDefault(x => x.ScMeasureId == scm.Id);
                if (target != null)
                    worksheet.Cells[i + 3, 2].Value = target.Target;
                else
                    worksheet.Cells[i + 3, 2].Value = string.Empty;

                if (mqsMeasure != null)
                {
                    worksheet.Cells[i + 3, 3].Style.Numberformat.Format = "@";
                    worksheet.Cells[i + 3, 3].Value = mqsMeasure.Value.Replace(',', '.');
                }
                i++;
            }
        }

        private void CreateScoreCardWorksheet(ExcelWorksheet worksheet, IList<ScMeasure> scMeasures, List<MqsMeasure> scoreCardItems, DateTime startDate)
        {
            worksheet.Cells["A1:A2"].Merge = true;
            var header = new Dictionary<string, string>
                {
                    { "A1", "MQS measure"},
                    { "B1", "FYTD"},
                    { "B2", "YTD"},
                    { "C1", "Actuals"},
                    { "C2", "Month"}
                };

            foreach (var cell in header)
            {
                HeaderStyle(worksheet.Cells[cell.Key], cell.Value);
            }
            worksheet.Column(1).Width = 50;
            worksheet.Column(2).Width = 15;
            worksheet.Column(3).Width = 15;
            for (int i = 0; i < scMeasures.Count; i++)
            {
                worksheet.Cells[i + 3, 1].Value = scMeasures[i].Name;
                worksheet.Cells[i + 3, 2].Value = scMeasures[i].YtdCalculatedType.ToString();
                worksheet.Cells[i + 3, 3].Value = 0;
                if (scMeasures[i].Formula != null && scMeasures[i].Formula.ToLower() == "manualedit")
                {
                    if (scoreCardItems == null) // export for template
                        worksheet.Cells[i + 3, 3].Value = "ManualEdit";
                    else
                    {
                        var valueTemp = (scoreCardItems != null && scoreCardItems.Count() > 0) ? scoreCardItems.FirstOrDefault(x => x.ScMeasureId == scMeasures[i].Id && x.Date.Year == startDate.Year && x.Date.Month == startDate.Month) : null;
                        if (valueTemp != null && !String.IsNullOrEmpty(valueTemp.Value))
                        {
                            var v = 0.0d;
                            double.TryParse(valueTemp.Value, out v);
                            worksheet.Cells[i + 3, 3].Value = v;
                        }
                        else
                            worksheet.Cells[i + 3, 3].Value = null;
                    }
                }
                else
                    worksheet.Cells[i + 3, 3].CreateArrayFormula(scMeasures[i].Formula);
            }
            worksheet.Calculate();
        }

        private void CreateSubColumnWorksheet(ExcelWorksheet worksheet, IList<SubColumnFormula> subColumns)
        {
            var header = new Dictionary<string, string>
                {
                    { "A2", "Column name"},
                    { "B2", "Formula"}
                };

            foreach (var cell in header)
            {
                HeaderStyle(worksheet.Cells[cell.Key], cell.Value);
            }

            worksheet.Column(1).Width = 20;
            worksheet.Column(2).Width = 20;

            var i = 0;
            foreach (var subC in subColumns)
            {

                worksheet.Cells[i + 3, 1].Value = subC.Name;
                worksheet.Cells[i + 3, 2].Formula = subC.Formula;
                i++;
            }
        }
        #endregion
    }
}
