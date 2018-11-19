using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;
using EmployeeTracking.Data.ModelCustom.Mobile;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Repositories
{
    public class TrackAttendanceRepo
    {
        public List<AttendanceManagerModel> GetAllAttendance()
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    var query = (from ta in _data.track_attendance
                                 join e in _data.employees
                                      on ta.EmployeeId equals e.Id
                                 select new AttendanceManagerModel
                                 {
                                     Id = ta.Id,
                                     Date = ta.Date,
                                     EmployeeId = ta.EmployeeId,
                                     EmployeeCode = e.Code,
                                     EmployeeName = e.Name,
                                     End = ta.End,
                                     Start = ta.Start
                                 }).ToList();

                    int index = 1;
                    foreach (var item in query)
                    {
                        item.DateString = item.Date.ToString("dd/MM/yyyy");
                        item.Index = index;
                        index++;
                    }
                    return query;
                }
            }
            catch (Exception ex)
            {
                return new List<AttendanceManagerModel>();
            }
        }

        public void AttendanceStart(track_attendance model)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var trackAttend = _db.track_attendance.FirstOrDefault(
                    _ =>
                    _.EmployeeId == model.EmployeeId &&
                    _.Date.Year == model.Date.Year &&
                    _.Date.Month == model.Date.Month &&
                    _.Date.Day == model.Date.Day
                );
                if (trackAttend == null)
                {
                    var newtrackAttend = new track_attendance()
                    {
                        CreatedBy = model.EmployeeId,
                        Date = model.Date,
                        EmployeeId = model.EmployeeId,
                        End = null,
                        Id = Guid.NewGuid(),
                        IsActive = true,
                        Start = model.Start,
                        StartCoordinates = model.StartCoordinates
                    };
                    _db.track_attendance.Add(newtrackAttend);
                    _db.SaveChanges();
                }
                //else
                //    throw new Exception("");
            }
        }

        public void AttendanceEnd(track_attendance model)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var trackAttend = _db.track_attendance.FirstOrDefault(
                    _ =>
                    _.EmployeeId == model.EmployeeId &&
                    _.Date.Year == model.Date.Year &&
                    _.Date.Month == model.Date.Month &&
                    _.Date.Day == model.Date.Day
                );
                if (trackAttend == null)
                {
                    throw new Exception("Bạn chưa thực hiện điểm danh bắt đầu.");
                }
                else
                {
                    if (trackAttend.End.HasValue)
                    {
                        //throw new Exception("Attendance End");
                    }
                    else
                    {
                        trackAttend.End = model.End;

                        trackAttend.ModifiedBy = model.EmployeeId;
                        trackAttend.ModifiedDate = DateTime.Now;
                        trackAttend.EndCoordinates = model.EndCoordinates;
                        _db.SaveChanges();
                    }
                }
            }
        }

        public Byte[] GetExportTrackList()
        {
            using (employeetracking_devEntities _data = new employeetracking_devEntities())
            {
                var query = (from ta in _data.track_attendance
                             join e in _data.employees
                                  on ta.EmployeeId equals e.Id
                             select new AttendanceManagerModel
                             {
                                 Id = ta.Id,
                                 Date = ta.Date,
                                 EmployeeId = ta.EmployeeId,
                                 EmployeeCode = e.Code,
                                 EmployeeName = e.Name,
                                 End = ta.End,
                                 Start = ta.Start
                             }).ToList();

                int index = 1;
                foreach (var item in query)
                {
                    item.DateString = item.Date.ToString("dd/MM/yyyy");
                    item.Index = index;
                    index++;
                }

                /////////////////////////////////////////////////
                // Write data to excel

                using (ExcelPackage p = new ExcelPackage())
                {
                    //Here setting some document properties
                    p.Workbook.Properties.Author = "Công ty";
                    p.Workbook.Properties.Title = "Báo cáo";

                    //Create a sheet
                    p.Workbook.Worksheets.Add("Báo cáo");
                    ExcelWorksheet ws = p.Workbook.Worksheets[1];
                    ws.Name = "Báo cáo"; //Setting Sheet's name
                    ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                    ws.Cells.Style.Font.Name = "Times New Roman"; //Default Font name for whole sheet

                    // Create header column
                    string[] arrColumnHeader = {
                                                "STT",
                                                "Mã nhân viên",
                                                "Họ và tên",
                                                "Ngày",
                                                "Giờ bắt đầu",
                                                "Giờ kết thúc"
                    };
                    var countColHeader = arrColumnHeader.Count();

                    int colIndex = 1;
                    int rowIndex = 1;

                    //Creating Headings
                    foreach (var item in arrColumnHeader)
                    {
                        ws.Cells[rowIndex, colIndex, rowIndex, colIndex].Style.Font.Bold = true;
                        ws.Cells[rowIndex, colIndex, rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[rowIndex, colIndex, rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        var cell = ws.Cells[rowIndex, colIndex, rowIndex, colIndex];

                        //Setting the background color of header cells to Gray
                        var fill3 = cell.Style.Fill;
                        fill3.PatternType = ExcelFillStyle.Solid;
                        fill3.BackgroundColor.SetColor(1, 0, 176, 240);

                        //Setting Top/left,right/bottom borders.
                        var border = cell.Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        //Setting Value in cell
                        cell.Value = item;

                        colIndex++;
                    }

                    // Adding Data into rows
                    foreach (var item in query)
                    {
                        colIndex = 1;
                        rowIndex++;
                        //Setting Value in cell
                        ws.Cells[rowIndex, colIndex].Value = item.Index;
                        //Setting Top/left,right/bottom borders.
                        var border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.EmployeeCode;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.EmployeeName;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.DateString;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.Start.HasValue ? item.Start.Value.ToString(@"hh\:mm\:ss") : "";
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.End.HasValue ? item.End.Value.ToString(@"hh\:mm\:ss") : "";
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    //Generate A File with name
                    Byte[] bin = p.GetAsByteArray();

                    return bin;
                }
            }
        }

        public bool CheckAttendanceStart(track_attendance model)
        {
            try
            {
                bool rs = false;
                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
                    if (_db.track_attendance.FirstOrDefault(
                        _ =>
                        _.EmployeeId == model.EmployeeId &&
                        _.Date.Year == model.Date.Year &&
                        _.Date.Month == model.Date.Month &&
                        _.Date.Day == model.Date.Day &&
                        _.Start != null
                    ) != null)
                    {
                        rs =  true;
                    }
                    return rs;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool CheckAttendanceEnd(track_attendance model)
        {
            try
            {
                bool rs = false;
                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
                    if (_db.track_attendance.FirstOrDefault(
                        _ =>
                        _.EmployeeId == model.EmployeeId &&
                        _.Date.Year == model.Date.Year &&
                        _.Date.Month == model.Date.Month &&
                        _.Date.Day == model.Date.Day &&
                        _.End != null
                    ) != null)
                    {
                        rs = true;
                    }
                    return rs;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
