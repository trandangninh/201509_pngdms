using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Entities.Domain;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Entities.Domain.QualityAlerts;

namespace Service.Common
{
    public class ReportService : IReportService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="report"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public void ExportMakingToXlsx(Stream stream, List<MeetingReport> report, string path)
        {
            
            var dmsCodes = report.Select(t => t.DmsDecription).Distinct().ToList();
            var listResults = new List<LineResultReport>();
            var resultLine = report.Select(t => t.ListResult).FirstOrDefault();
            foreach (var lineItem in resultLine)
            {
                listResults.Add(lineItem);
            }


            listResults = listResults.ToList();
            if (stream == null)
                throw new ArgumentNullException("stream");

            // ok, we can run the real code of the sample now
            using (var xlPackage = new ExcelPackage(stream))
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Orders");
               

                #region create header

                worksheet.Cells["A1:A2"].Merge = true;
                worksheet.Cells["A1:A2"].Value = "DMS";
                worksheet.Cells["A1:A2"].Style.Font.Size = 12;
                worksheet.Cells["A1:A2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A1:A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:A2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheet.Cells["A1:A2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:A2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheet.Cells["A1:A2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheet.Cells["B1:B2"].Merge = true;
                worksheet.Cells["B1:B2"].Value = "IP/OP";
                worksheet.Cells["B1:B2"].Style.Font.Size = 12;
                worksheet.Cells["B1:B2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["B1:B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B1:B2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige);
                worksheet.Cells["B1:B2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["B1:B2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                worksheet.Cells["B1:B2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheet.Cells["C1:C2"].Merge = true;
                worksheet.Cells["C1:C2"].Style.WrapText = true;
                worksheet.Cells["C1:C2"].Value = "Measure";
                worksheet.Cells["C1:C2"].Style.Font.Size = 12;
                worksheet.Cells["C1:C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["C1:C2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["C1:C2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige);
                worksheet.Cells["C1:C2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["C1:C2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                worksheet.Cells["C1:C2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheet.Cells["D1:D2"].Merge = true;
                worksheet.Cells["D1:D2"].Value = "Owner";
                worksheet.Cells["D1:D2"].Style.Font.Size = 12;
                worksheet.Cells["D1:D2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["D1:D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["D1:D2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige);
                worksheet.Cells["D1:D2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["D1:D2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                worksheet.Cells["D1:D2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheet.Cells["E1:E2"].Merge = true;
                worksheet.Cells["E1:E2"].Value = "Target";
                worksheet.Cells["E1:E2"].Style.Font.Size = 12;
                worksheet.Cells["E1:E2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["E1:E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["E1:E2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige);
                worksheet.Cells["E1:E2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["E1:E2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                worksheet.Cells["E1:E2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheet.Cells["F1:F2"].Merge = true;
                worksheet.Cells["F1:F2"].Value = "Unit";
                worksheet.Cells["F1:F2"].Style.Font.Size = 12;
                worksheet.Cells["F1:F2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["F1:F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["F1:F2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige);
                worksheet.Cells["F1:F2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["F1:F2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                worksheet.Cells["F1:F2"].Style.Font.Color.SetColor(Color.White); // set color font
                worksheet.Column(1).Width = 25;
                worksheet.Column(2).Width = 20;
                worksheet.Column(3).Width = 41;
                worksheet.Column(4).Width = 25;
                int totelItem = report[0].ListResult.Count + 6;
                worksheet.Cells[1, 1, 27, totelItem].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                for (int i = 0; i < report[0].ListResult.Count; i++)
                {
                    if (i % 5 == 0)
                    { // set day for group line

                        worksheet.Cells[1, 7 + i, 1, 11 + i].Merge = true;
                        worksheet.Cells[1, 7 + i, 1, 11 + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, 7 + i, 1, 11 + i].Value = report[0].ListResult[i].DateTimeCreate.ToShortDateString();
                        worksheet.Cells[1, 7 + i, 1, 11 + i].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige);
                        worksheet.Cells[1, 7 + i, 1, 11 + i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 7 + i, 1, 11 + i].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                        worksheet.Cells[1, 7 + i, 1, 11 + i].Style.Font.Color.SetColor(Color.White); // set color font
                        worksheet.Cells[1, 7 + i, 1, 11 + i].Style.Font.Size = 12;
                    }
                    worksheet.Cells[2, 7 + i].Value = report[0].ListResult[i].LineName;
                    worksheet.Cells[2, 7 + i].Style.Font.Size = 12;
                    worksheet.Cells[2, 7 + i].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige);
                    worksheet.Cells[2, 7 + i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[2, 7 + i].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                }
                worksheet.Cells[2,7,2,100].Style.Font.Color.SetColor(Color.White); // set color font
                #endregion

                #region create  content for excell

                var evenRow = 0;
                int col = 0; // colum

                for (int i = 0; i < dmsCodes.Count; i++)
                {
                    var currentRow = 0;
                    var totalDmsMeasures = 0;
                    evenRow++;
                    var data = report.Where(t => t.DmsDecription == dmsCodes[i]);

                    totalDmsMeasures = data.Count();
                    if (totalDmsMeasures < 1)
                    {
                        totalDmsMeasures = 1;
                    }

                    foreach (var meetingResultModel in data)
                    {
                        currentRow++;
                        if (currentRow == 1)
                        {
                            worksheet.Cells[3 + col, 1, 2 + col + totalDmsMeasures, 1].Merge =
                                true;
                            worksheet.Cells[3 + col, 1, 2 + col + totalDmsMeasures, 1].AutoFitColumns();
                            worksheet.Cells[3 + col, 1, 2 + col + totalDmsMeasures, 1].Value =
                                meetingResultModel.DmsDecription;
                            worksheet.Cells[3 + col, 1, 2 + col + totalDmsMeasures, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[3 + col, 1, 2 + col + totalDmsMeasures, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells[3 + col, 1, 2 + col + totalDmsMeasures, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige);
                            worksheet.Cells[3 + col, 1, 2 + col + totalDmsMeasures, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[3 + col, 1, 2 + col + totalDmsMeasures, 1].Style.Fill.BackgroundColor.SetColor(Color.Orange);
                            worksheet.Cells[3 + col, 1, 2 + col + totalDmsMeasures, 1].Style.Font.Color.SetColor(Color.White);

                        }

                        worksheet.Cells[3 + col, 2].Value = meetingResultModel.MeasureType;
                        worksheet.Cells[3 + col, 3].Value = meetingResultModel.MeasureName;
                        worksheet.Cells[3 + col, 4].Value = meetingResultModel.Owner;
                        worksheet.Cells[3 + col, 5].Value = meetingResultModel.Target;
                        worksheet.Cells[3 + col, 6].Value = meetingResultModel.Unit;
                        int j = 0;
                        foreach (var str in listResults)
                        {

                            var lineValue = meetingResultModel.ListResult.FirstOrDefault(s => s.LineCode == str.LineCode);
                            meetingResultModel.ListResult.Remove(lineValue);
                            DmsCode dmsc;
                            Enum.TryParse(dmsCodes[i], out dmsc);
                            var dmsci = (int)dmsc;

                            var measurec = meetingResultModel.MeasureCode;
                            NoisMainMeasureType mc;
                            Enum.TryParse(measurec, out mc);
                            var mci = (int)mc;

                            var linec = str.LineCode.Replace("(", "").Replace(")", "");
                            LineHardCodeType lc;
                            Enum.TryParse(linec, out lc);
                            var lci = (int)lc;
                            worksheet.Cells[3 + col, j + 7].Value = lineValue.Result;
                            if (j%5 == 0)
                            {
                                worksheet.Cells[3 + col, j + 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[3 + col, j + 6].Style.Border.Right.Color.SetColor(System.Drawing.Color.DarkGray);
                            }
                            j++;
                        }
                        col++;
                    }

                }

                #endregion

                if (String.IsNullOrEmpty(path))
                {
                    xlPackage.Save(); // save to excell
                }
                else
                {
                    //Stream streamPath = File.Create(path);
                    //xlPackage.SaveAs(streamPath);

                    xlPackage.SaveAs(new FileInfo(path));

                    stream.Close();
                }
            }

        }

        public string checkColor(string target, string number)
        {
            try
            {
                if (float.Parse(target.Replace("%", "")) > float.Parse(number.Replace("%", "")))
                    return "no-match-target";
                else
                {
                    return "match-target";
                    
                }

            }
            catch (Exception)
            {

                return "";
            }
        }

        public string checkColorPacking(string target, string number)
        {
            try
            {
                if (target.Contains(">"))
                {
                    target = target.Replace(">","").Replace("=","");
                    if (float.Parse(target) > float.Parse(number))
                        return "no-match-target";
                    else
                    {
                        return "match-target";

                    }
                }
                if (target.Contains("<"))
                {
                    target = target.Replace("<", "").Replace("=", "");
                    if (float.Parse(target) < float.Parse(number))
                        return "no-match-target";
                    else
                    {
                        return "match-target";

                    }
                }
                else
                {
                    return "exception-target";
                }

            }
            catch (Exception)
            {

                return "exception-target";
            }
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
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="report"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public void ExportQualityAlertToXlsx(Stream stream, List<QualityAlertFullObject> report, string path)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            // ok, we can run the real code of the sample now
            using (var xlPackage = new ExcelPackage(stream))
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Quality Alert");

                var header = new Dictionary<string, string>
                {
                    { "A1", "STT Alert"},
                    { "B1", "Alert ID"},
                    { "C1", "Alert Date"},
                    { "D1", "Found by"},
                    { "E1", "Line"},
                    { "F1", "Machine"},
                    { "G1", "Description"},
                    { "H1", "Immediate Action"},
                    { "I1", "GCAS"},
                    { "J1", "SAP Lot"},
                    { "K1", "Supplier Lot"},
                    { "L1", "Supplier name"},
                    { "M1", "Material name"},
                    { "N1", "Blocked Qty"},
                    { "O1", "Unit"},
                    { "P1", "Owner"},
                    { "Q1", "Follow up action"},
                    { "R1", "When"},
                    { "S1", "Status"},
                    { "T1", "Classification"},
                    { "U1", "Category"},
                    { "V1", "Complaint Type"},
                    { "W1", "Classification Defect"},
                    { "X1", "Quantity Return"},
                    { "Y1", "Defect Repeat"},
                    { "Z1", "Supplier Reply Date"},
                    //{ "Z2", "Cost Impacted"},
                    { "AA1", "Defected Qty"},
                    { "AB1", "%PR Loss"},
                    { "AC1", "#Stop"},
                    { "AD1", "Date Informed To Supplier"},
                    { "AE1", "Effort Loss (PGer)"},
                    { "AF1", "Effort Loss (Contractor)"},
                };

                foreach(var cell in header)
                {
                    HeaderStyle(worksheet.Cells[cell.Key], cell.Value);
                }


                #region create headers

                worksheet.Column(1).Width = 10;
                worksheet.Column(2).Width = 15;
                worksheet.Column(3).Width = 20;
                worksheet.Column(4).Width = 10;
                worksheet.Column(5).Width = 10;
                worksheet.Column(6).Width = 15;
                worksheet.Column(7).Width = 30;
                worksheet.Column(8).Width = 30;
                worksheet.Column(9).Width = 20;
                worksheet.Column(10).Width = 15;
                worksheet.Column(11).Width = 15;
                worksheet.Column(12).Width = 15;
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
                worksheet.Column(30).Width = 25;
                worksheet.Column(31).Width = 20;
                worksheet.Column(32).Width = 25;

                worksheet.Cells[1, 1, report.Count + 1, 32].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                #endregion

                #region create  content for excell           

                for (int i = 0; i < report.Count; i++ )
                {
                    worksheet.Cells[i + 2, 1].Value = i + 1;
                    worksheet.Cells[i + 2, 2].Value = report[i].Id;
                    worksheet.Cells[i + 2, 3].Value = report[i].AlertDateTime == new DateTime() ? string.Empty : report[i].AlertDateTime.ToString("MM/dd/yyyy HH:mm");
                    worksheet.Cells[i + 2, 4].Value = report[i].UserNameCreated;
                    worksheet.Cells[i + 2, 5].Value = report[i].LineName;
                    worksheet.Cells[i + 2, 6].Value = report[i].Machine;
                    worksheet.Cells[i + 2, 7].Value = report[i].Detail;
                    worksheet.Cells[i + 2, 8].Value = report[i].Action;
                    worksheet.Cells[i + 2, 9].Value = report[i].GCAS;
                    worksheet.Cells[i + 2, 10].Value = report[i].SAPLot;
                    worksheet.Cells[i + 2, 11].Value = report[i].SupplierLot;
                    worksheet.Cells[i + 2, 12].Value = report[i].SupplierName;
                    //worksheet.Cells[i + 2, 12].Value = report[i].MaterialId;
                    worksheet.Cells[i + 2, 13].Value = report[i].Material;
                    worksheet.Cells[i + 2, 14].Value = report[i].NumBlock;
                    worksheet.Cells[i + 2, 15].Value = report[i].Unit;
                    worksheet.Cells[i + 2, 16].Value = report[i].OwnerName;
                    worksheet.Cells[i + 2, 17].Value = report[i].FollowUpAction;
                    worksheet.Cells[i + 2, 18].Value = report[i].When == new DateTime() ? string.Empty : report[i].When.ToString("MM/dd/yyyy");
                    worksheet.Cells[i + 2, 19].Value = report[i].QualityAlertStatus.ToString();
                    worksheet.Cells[i + 2, 20].Value = report[i].ClassificationName;
                    worksheet.Cells[i + 2, 21].Value = report[i].CategoryName;
                    worksheet.Cells[i + 2, 22].Value = report[i].ComplaintTypeId == 0 ? string.Empty : report[i].ComplaintType.ToString();
                    worksheet.Cells[i + 2, 23].Value = report[i].ClassificationDefectName;
                    worksheet.Cells[i + 2, 24].Value = report[i].QuantityReturn;
                    worksheet.Cells[i + 2, 25].Value = report[i].DefectRepeatId == 0 ? string.Empty : report[i].DefectRepeat.ToString();
                    worksheet.Cells[i + 2, 26].Value = report[i].SupplierReplyDate == new DateTime() ? string.Empty : report[i].SupplierReplyDate.ToString("MM/dd/yyyy");
                    //worksheet.Cells[i + 2, 26].Value = report[i].CostImpacted;
                    worksheet.Cells[i + 2, 27].Value = report[i].DefectedQty;
                    worksheet.Cells[i + 2, 28].Value = report[i].PRLossPercent;
                    worksheet.Cells[i + 2, 29].Value = report[i].NumStop;
                    worksheet.Cells[i + 2, 30].Value = report[i].InformedToSupplierDate == new DateTime() ? string.Empty : report[i].InformedToSupplierDate.ToString("MM/dd/yyyy");
                    worksheet.Cells[i + 2, 31].Value = report[i].PGerEffortLoss;
                    worksheet.Cells[i + 2, 32].Value = report[i].ContractorEffortLoss;
                }
                #endregion

                if (String.IsNullOrEmpty(path))
                    {
                        xlPackage.Save(); // save to excell
                    }
                    else
                    {
                        //Stream streamPath = File.Create(path);
                        //xlPackage.SaveAs(streamPath);

                        xlPackage.SaveAs(new FileInfo(path));

                        stream.Close();
                    }
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="report"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public void ExportDdsMeetingToXlsx(Stream stream, List<MeetingReport> report, string filePath)
        {

            var dmsCodes = report.Select(t => t.DmsDecription).Distinct().ToList();
            var listResults = new List<LineResultReport>();
            var resultLine = report.Select(t => t.ListResult).FirstOrDefault();
            foreach (var lineItem in resultLine)
            {
                listResults.Add(lineItem);
            }


            listResults = listResults.ToList();
            if (stream == null)
                throw new ArgumentNullException("stream");

            // ok, we can run the real code of the sample now
            using (var xlPackage = new ExcelPackage(stream))
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Orders");


                #region create header

                worksheet.Cells["A1:A2"].Merge = true;
                worksheet.Cells["A1:A2"].Value = "DMS";
                worksheet.Cells["A1:A2"].Style.Font.Size = 12;
                worksheet.Cells["A1:A2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A1:A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:A2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige); // set border color
                worksheet.Cells["A1:A2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:A2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue); // set background color
                worksheet.Cells["A1:A2"].Style.Font.Color.SetColor(Color.White); // set color font

                worksheet.Cells["B1:B2"].Merge = true;
                worksheet.Cells["B1:B2"].Value = "IP/OP";
                worksheet.Cells["B1:B2"].Style.Font.Size = 12;
                worksheet.Cells["B1:B2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["B1:B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B1:B2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige);
                worksheet.Cells["B1:B2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["B1:B2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                worksheet.Cells["B1:B2"].Style.Font.Color.SetColor(Color.White); // set color font

                worksheet.Cells["C1:C2"].Merge = true;
                worksheet.Cells["C1:C2"].Style.WrapText = true;
                worksheet.Cells["C1:C2"].Value = "Measure";
                worksheet.Cells["C1:C2"].Style.Font.Size = 12;
                worksheet.Cells["C1:C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["C1:C2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["C1:C2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige);
                worksheet.Cells["C1:C2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["C1:C2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                worksheet.Cells["C1:C2"].Style.Font.Color.SetColor(Color.White); // set color font

                worksheet.Cells["D1:D2"].Merge = true;
                worksheet.Cells["D1:D2"].Value = "Owner";
                worksheet.Cells["D1:D2"].Style.Font.Size = 12;
                worksheet.Cells["D1:D2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["D1:D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["D1:D2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige);
                worksheet.Cells["D1:D2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["D1:D2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                worksheet.Cells["D1:D2"].Style.Font.Color.SetColor(Color.White); // set color font

                worksheet.Cells["E1:E2"].Merge = true;
                worksheet.Cells["E1:E2"].Value = "Target";
                worksheet.Cells["E1:E2"].Style.Font.Size = 12;
                worksheet.Cells["E1:E2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["E1:E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["E1:E2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige);
                worksheet.Cells["E1:E2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["E1:E2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                worksheet.Cells["E1:E2"].Style.Font.Color.SetColor(Color.White); // set color font

                worksheet.Cells["F1:F2"].Merge = true;
                worksheet.Cells["F1:F2"].Value = "Unit";
                worksheet.Cells["F1:F2"].Style.Font.Size = 12;
                worksheet.Cells["F1:F2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["F1:F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["F1:F2"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige);
                worksheet.Cells["F1:F2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["F1:F2"].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                worksheet.Cells["F1:F2"].Style.Font.Color.SetColor(Color.White); // set color font


                int totalItem = report[0].ListResult.Count + 6;
                
                //for (int i = 0; i < report[0].ListResult.Count; i++)
                //{
                //    if (i % 5 == 0)
                //    { // set day for group line

                //        worksheet.Cells[1, 7 + i, 1, 11 + i].Merge = true;
                //        worksheet.Cells[1, 7 + i, 1, 11 + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //        worksheet.Cells[1, 7 + i, 1, 11 + i].Value = report[0].ListResult[i].DateTimeCreate.ToShortDateString();
                //        worksheet.Cells[1, 7 + i, 1, 11 + i].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige);
                //        worksheet.Cells[1, 7 + i, 1, 11 + i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //        worksheet.Cells[1, 7 + i, 1, 11 + i].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                //        worksheet.Cells[1, 7 + i, 1, 11 + i].Style.Font.Color.SetColor(Color.White); // set color font
                //        worksheet.Cells[1, 7 + i, 1, 11 + i].Style.Font.Size = 12;
                //    }
                //    worksheet.Cells[2, 7 + i].Value = report[0].ListResult[i].LineName;
                //    worksheet.Cells[2, 7 + i].Style.Font.Size = 12;
                //    worksheet.Cells[2, 7 + i].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige);
                //    worksheet.Cells[2, 7 + i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //    worksheet.Cells[2, 7 + i].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                //}
                //worksheet.Cells[2, 7, 2, 100].Style.Font.Color.SetColor(Color.White); // set color font

                var listTemp = new List<DateTime>();
                int count;
                for (int i = 0; i < report[0].ListResult.Count; i++)
                {
                    if (!listTemp.Contains(report[0].ListResult[i].DateTimeCreate))
                    {
                        count = 6;
                        count += report[0].ListResult.Where(x => x.DateTimeCreate == report[0].ListResult[i].DateTimeCreate).Count();
                        listTemp.Add(report[0].ListResult[i].DateTimeCreate);

                        worksheet.Cells[1, 7 + i, 1, count + i].Merge = true;
                        worksheet.Cells[1, 7 + i, 1, count + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, 7 + i, 1, count + i].Value = report[0].ListResult[i].DateTimeCreate.ToShortDateString();
                        worksheet.Cells[1, 7 + i, 1, count + i].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige);
                        worksheet.Cells[1, 7 + i, 1, count + i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 7 + i, 1, count + i].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                        worksheet.Cells[1, 7 + i, 1, count + i].Style.Font.Color.SetColor(Color.White); // set color font
                        worksheet.Cells[1, 7 + i, 1, count + i].Style.Font.Size = 12;
                        for (int j = 0; j < report.Count; j++)
                        {
                            worksheet.Cells[3 + j, count + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[3 + j, count + i].Style.Border.Right.Color.SetColor(System.Drawing.Color.DarkGray);
                        }                       
                    }
                    worksheet.Cells[2, 7 + i].Value = report[0].ListResult[i].LineName;
                    worksheet.Cells[2, 7 + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[2, 7 + i].Style.Font.Size = 12;
                    worksheet.Cells[2, 7 + i].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige);
                    worksheet.Cells[2, 7 + i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[2, 7 + i].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                    worksheet.Cells[2, 7 + i].Style.Font.Color.SetColor(Color.White);
                }

                for (int j = 0; j < report.Count; j++)
                {
                    worksheet.Cells[3 + j, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[3 + j, 6].Style.Border.Right.Color.SetColor(System.Drawing.Color.DarkGray);
                }
                #endregion

                //fill value to each cell
                for (int i = 0; i < report.Count; i++)
                {
                    worksheet.Cells[i + 3, 1].Value = report[i].DmsDecription;
                    worksheet.Cells[i + 3, 2].Value = report[i].MeasureType;
                    worksheet.Cells[i + 3, 3].Value = report[i].MeasureName;
                    worksheet.Cells[i + 3, 4].Value = report[i].Owner;
                    worksheet.Cells[i + 3, 5].Value = report[i].Target;
                    worksheet.Cells[i + 3, 6].Value = report[i].Unit;
                    for (int j = 0; j < report[0].ListResult.Count; j++)
                    {
                        worksheet.Cells[i + 3, j + 7].Value = report[i].ListResult[j].Result;
                        if(report[i].ListResult[j].IsLastLineOfDay)
                        {
                            worksheet.Cells[i + 3, j + 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[i + 3, j + 7].Style.Border.Right.Color.SetColor(System.Drawing.Color.DimGray);
                        }
                    }
                }


                //margin cell of measure has DmsCode == DeedmacOperation
                listTemp.Clear();
                for (int j = 0; j < report.Count; j++)
                {
                    if (report[j].DmsId == 35)
                    {
                        for (int i = 0; i < report[0].ListResult.Count; i++)
                        {
                            if (!listTemp.Contains(report[0].ListResult[i].DateTimeCreate))
                            {
                                count = 6;
                                count += report[0].ListResult.Where(x => x.DateTimeCreate == report[0].ListResult[i].DateTimeCreate).Count();
                                listTemp.Add(report[0].ListResult[i].DateTimeCreate);

                                worksheet.Cells[3 + j, 7 + i, 3 + j, count + i].Merge = true;
                            }
                        }
                        listTemp.Clear();
                    }
                }



                var listDms = report.Select(x => x.DmsDecription).ToList();
                var listRemarkDms = new List<string>();

                //margin dms column
                for (int i = 0; i < report.Count; i++)
                {
                    if (!listRemarkDms.Contains(listDms[i]))
                    {
                        count = 2;
                        count += listDms.Where(x => x == listDms[i]).Count();
                        worksheet.Cells[3 + i, 1, count + i, 1].Merge =
                                true;
                        worksheet.Cells[3 + i, 1, count + i, 1].AutoFitColumns();
                        worksheet.Cells[3 + i, 1, count + i, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[3 + i, 1, count + i, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[3 + i, 1, count + i, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige);
                        worksheet.Cells[3 + i, 1, count + i, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[3 + i, 1, count + i, 1].Style.Fill.BackgroundColor.SetColor(Color.Orange);
                        worksheet.Cells[3 + i, 1, count + i, 1].Style.Font.Color.SetColor(Color.White);
                        listRemarkDms.Add(listDms[i]);
                    }
                }

                worksheet.Column(1).Width = 20;
                worksheet.Column(2).Width = 10;
                worksheet.Column(3).Width = 40;
                worksheet.Column(4).Width = 25;

                worksheet.Cells[3, 6, report.Count + 2, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[3, 6, report.Count + 2, 6].Style.Border.Right.Color.SetColor(System.Drawing.Color.DimGray);

                worksheet.Cells[1, 1, report.Count + 2, totalItem].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                /*
                #region create  content for excell

                var evenRow = 0;
                int col = 0; // colum

                for (int i = 0; i < dmsCodes.Count; i++)
                {
                    var currentRow = 0;
                    var totalDmsMeasures = 0;
                    evenRow++;
                    var data = report.Where(t => t.DmsCode == dmsCodes[i]);

                    totalDmsMeasures = data.Count();
                    if (totalDmsMeasures < 1)
                    {
                        totalDmsMeasures = 1;
                    }

                    foreach (var meetingResultModel in data)
                    {
                        currentRow++;
                        if (currentRow == 1)
                        {
                            worksheet.Cells[3 + col, 1, 2 + col + totalDmsMeasures, 1].Merge =
                                true;
                            worksheet.Cells[3 + col, 1, 2 + col + totalDmsMeasures, 1].AutoFitColumns();
                            worksheet.Cells[3 + col, 1, 2 + col + totalDmsMeasures, 1].Value =
                                meetingResultModel.DmsCode;
                            worksheet.Cells[3 + col, 1, 2 + col + totalDmsMeasures, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[3 + col, 1, 2 + col + totalDmsMeasures, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells[3 + col, 1, 2 + col + totalDmsMeasures, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Beige);
                            worksheet.Cells[3 + col, 1, 2 + col + totalDmsMeasures, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[3 + col, 1, 2 + col + totalDmsMeasures, 1].Style.Fill.BackgroundColor.SetColor(Color.Orange);
                            worksheet.Cells[3 + col, 1, 2 + col + totalDmsMeasures, 1].Style.Font.Color.SetColor(Color.White);

                        }

                        worksheet.Cells[3 + col, 2].Value = meetingResultModel.MeasureType;
                        worksheet.Cells[3 + col, 3].Value = meetingResultModel.MeasureName;
                        worksheet.Cells[3 + col, 4].Value = meetingResultModel.Owner;
                        worksheet.Cells[3 + col, 5].Value = meetingResultModel.Target;
                        worksheet.Cells[3 + col, 6].Value = meetingResultModel.Unit;
                        int j = 0;
                        foreach (var str in listResults)
                        {

                            var lineValue = meetingResultModel.ListResult.FirstOrDefault(s => s.LineCode == str.LineCode);
                            meetingResultModel.ListResult.Remove(lineValue);
                            DmsCode dmsc;
                            Enum.TryParse(dmsCodes[i], out dmsc);
                            var dmsci = (int)dmsc;

                            var measurec = meetingResultModel.MeasureCode;
                            NoisMainMeasureType mc;
                            Enum.TryParse(measurec, out mc);
                            var mci = (int)mc;

                            var linec = str.LineCode.Replace("(", "").Replace(")", "");
                            LineHardCodeType lc;
                            Enum.TryParse(linec, out lc);
                            var lci = (int)lc;
                            worksheet.Cells[3 + col, j + 7].Value = lineValue.Result;
                            if (j % 5 == 0)
                            {
                                worksheet.Cells[3 + col, j + 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[3 + col, j + 6].Style.Border.Right.Color.SetColor(System.Drawing.Color.DarkGray);
                            }
                            j++;
                        }
                        col++;
                    }

                }
                worksheet.Cells[1, 1, report.Count + 2, totelItem].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                #endregion
                */

                if (String.IsNullOrEmpty(filePath))
                {
                    xlPackage.Save(); // save to excell
                }
                else
                {
                    //Stream streamPath = File.Create(path);
                    //xlPackage.SaveAs(streamPath);

                    xlPackage.SaveAs(new FileInfo(filePath));

                    stream.Close();
                }
            }

        }

        /// <summary>
        /// export complaint letter
        /// </summary>
        /// <param name="streamSource"></param>
        /// <param name="streamDestination"></param>
        /// <param name="listQualityAlert"></param>
        /// <param name="path"></param>
        public void ExportComplaintLetterToXlsx(Stream streamSource, Stream streamDestination, List<QualityAlertFullObject> report, string path)
        {
            if (streamSource == null)
                throw new ArgumentNullException("stream");
            using (var xlPackageSource = new ExcelPackage(streamSource))
            {
                var xlPackageDestination = new ExcelPackage(streamDestination);
                xlPackageDestination.Workbook.Worksheets.Add("Complaint flow chart", xlPackageSource.Workbook.Worksheets[1]);
                var worksheet = xlPackageDestination.Workbook.Worksheets.Add("Summary", xlPackageSource.Workbook.Worksheets[2]);
                xlPackageDestination.Workbook.Worksheets.Add("Flow of investigation", xlPackageSource.Workbook.Worksheets[3]);
                xlPackageDestination.Workbook.Worksheets.Add("WWA", xlPackageSource.Workbook.Worksheets[4]);

                worksheet.Select();

                for (int i = 0; i < report.Count; i++)
                {
                    worksheet.Cells[i + 7, 1].Value = report[i].SupplierName + "-" + report[i].CreatedDate.ToString("MM", new System.Globalization.CultureInfo("en-US")) + "-" + report[i].ComplaintType.ToString().FirstOrDefault() + "-" + report[i].Id;
                    worksheet.Cells[i + 7, 2].Value = report[i].InformedToSupplierDate == new DateTime() ? string.Empty : report[i].InformedToSupplierDate.ToString("dd-MMM-yyyy");
                    worksheet.Cells[i + 7, 3].Value = report[i].CreatedDate == new DateTime() ? string.Empty : report[i].CreatedDate.ToString("dd-MMM-yyyy");
                    worksheet.Cells[i + 7, 4].Value = report[i].Material;
                    worksheet.Cells[i + 7, 5].Value = report[i].GCAS;
                    worksheet.Cells[i + 7, 6].Value = report[i].SAPLot;
                    worksheet.Cells[i + 7, 7].Value = report[i].SupplierLot;
                    if(report[i].QuantityReturn!=0)
                    worksheet.Cells[i + 7, 8].Value = report[i].QuantityReturn;
                    if(report[i].DefectedQty!=0)
                    worksheet.Cells[i + 7, 9].Value = report[i].DefectedQty;
                    worksheet.Cells[i + 7, 10].Value = report[i].Unit;
                    worksheet.Cells[i + 7, 11].Value = report[i].Detail;
                }
                if (String.IsNullOrEmpty(path))
                {
                    xlPackageDestination.Save(); // save to excell
                }
                else
                {
                    xlPackageDestination.SaveAs(new FileInfo(path));

                    streamDestination.Close();
                }
            }
        }
    }
}
