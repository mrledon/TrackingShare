using EmployeeTracking.Data.CommonData;
using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Core.Repositories
{
    public class ImageManagementRepo
    {
        public List<ImageManagementViewModel> GetTrackList()
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var model = (from rs in (from tr in _db.tracks
                                         join em in _db.employees on tr.EmployeeId equals em.Id
                                         join tr_se in _db.track_session.DefaultIfEmpty() on tr.Id equals tr_se.TrackId
                                         select new
                                         {
                                             Id = tr.Id,
                                             EmployeeId = tr.EmployeeId,
                                             EmployeeName = em.Name,
                                             MasterStoreId = tr.MasterStoreId,
                                             MasterStoreName = tr.MaterStoreName,
                                             CreateDate = tr.Date,
                                             StoreStatus = tr.StoreStatus,
                                             Region = tr.Region,
                                             TrackSessionId = tr_se.Id,
                                             TrackCreateDate = tr_se.Date
                                         })
                             group rs by new
                             {
                                 rs.Id,
                                 rs.EmployeeId,
                                 rs.EmployeeName,
                                 rs.MasterStoreId,
                                 rs.MasterStoreName,
                                 rs.CreateDate,
                                 rs.StoreStatus,
                                 rs.Region
                             } into g
                             select new ImageManagementViewModel()
                             {
                                 Id = g.Key.Id,
                                 EmployeeId = g.Key.EmployeeId,
                                 EmployeeName = g.Key.EmployeeName,
                                 MasterStoreId = g.Key.MasterStoreId,
                                 MasterStoreName = g.Key.MasterStoreName,
                                 CreateDate = g.Key.CreateDate,
                                 StoreStatus = g.Key.StoreStatus,
                                 Region = g.Key.Region,
                                 TrackSessions = g.Select(x => new TrackSessionViewModel
                                 {
                                     Id = x.TrackSessionId,
                                     CreateDate = x.TrackCreateDate
                                 })
                             }).OrderByDescending(x => x.CreateDate).ToList();

                return model;
            }
        }

        public StoreInfoViewModel GetStoreInfoByTrackSessionId(string id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var model = (from tr in _db.tracks
                             join ts in _db.track_session on tr.Id equals ts.TrackId
                             // sbvp
                             join st in _db.master_store.DefaultIfEmpty() on tr.MasterStoreId equals st.Id
                             join type in _db.master_store_type.DefaultIfEmpty() on st.StoreType equals type.Id
                             join s_p in _db.provinces.DefaultIfEmpty() on st.ProvinceId equals s_p.Id into rs_p
                             from s_p in rs_p.DefaultIfEmpty()
                             join s_d in _db.districts.DefaultIfEmpty() on st.DistrictId equals s_d.Id into rs_d
                             from s_d in rs_p.DefaultIfEmpty()
                             join s_w in _db.wards.DefaultIfEmpty() on st.WardId equals s_w.Id into rs_w
                             from s_w in rs_w.DefaultIfEmpty()
                                 // digix
                             join d_p in _db.provinces.DefaultIfEmpty() on tr.ProvinceId equals d_p.Id into rs_p1
                             from d_p in rs_p1.DefaultIfEmpty()
                             join d_d in _db.districts.DefaultIfEmpty() on tr.DistrictId equals d_d.Id into rs_d1
                             from d_d in rs_d1.DefaultIfEmpty()
                             join d_w in _db.wards.DefaultIfEmpty() on tr.WardId equals d_w.Id into rs_w1
                             from d_w in rs_w1.DefaultIfEmpty()
                            
                             where ts.Id == id
                             select new StoreInfoViewModel
                             {
                                 Id = tr.Id,
                                 SbvpName = st.Name,
                                 SbvpType = type.Name,
                                 SbvpProvince = s_p.Name,
                                 SbvpDistrict = s_d.Type + " " + s_d.Name,
                                 SbvpWard = s_w.Name,
                                 SbvpStreetName = st.StreetNames,
                                 SbvpHouseNumber = st.HouseNumber,
                                 DigixName = tr.MaterStoreName,
                                 DigixType = type.Name,
                                 DigixProvince = d_p.Name,
                                 DigixDistrict = d_d.Type + " " + d_d.Name,
                                 DigixWard = d_w.Name,
                                 DigixStreetName = tr.StreetNames,
                                 DigixHouseNumber = tr.HouseNumber
                             }).FirstOrDefault();

                return model;
            }
        }

        public StoreInfoViewModel GetStoreInfoByTrackId(string id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var model = (from tr in _db.tracks
                                 // sbvp
                             join st in _db.master_store.DefaultIfEmpty() on tr.MasterStoreId equals st.Id
                             join type in _db.master_store_type.DefaultIfEmpty() on st.StoreType equals type.Id
                             join s_p in _db.provinces.DefaultIfEmpty() on st.ProvinceId equals s_p.Id
                             join s_d in _db.districts.DefaultIfEmpty() on st.DistrictId equals s_d.Id
                             join s_w in _db.wards.DefaultIfEmpty() on st.WardId equals s_w.Id into rs1
                             from s_w in rs1.DefaultIfEmpty()
                                 // digix
                             join d_p in _db.provinces.DefaultIfEmpty() on tr.ProvinceId equals d_p.Id
                             join d_d in _db.districts.DefaultIfEmpty() on tr.DistrictId equals d_d.Id
                             join d_w in _db.wards.DefaultIfEmpty() on tr.WardId equals d_w.Id into rs2
                             from d_w in rs2.DefaultIfEmpty()
                             where tr.Id == id
                             select new StoreInfoViewModel
                             {
                                 Id = tr.Id,
                                 SbvpName = st.Name,
                                 SbvpType = type.Name,
                                 SbvpProvince = s_p.Name,
                                 SbvpDistrict = s_d.Type + " " + s_d.Name,
                                 SbvpWard = s_w.Name,
                                 SbvpStreetName = st.StreetNames,
                                 SbvpHouseNumber = st.HouseNumber,
                                 DigixName = tr.MaterStoreName,
                                 DigixType = type.Name,
                                 DigixProvince = d_p.Name,
                                 DigixDistrict = d_d.Type + " " + d_d.Name,
                                 DigixWard = d_w.Name,
                                 DigixStreetName = tr.StreetNames,
                                 DigixHouseNumber = tr.HouseNumber
                             }).FirstOrDefault();

                return model;
            }
        }

        public List<TrackSessionViewModel> GetTrackSessionListByTrackId(string id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var model = (from tr in _db.tracks
                             join tr_se in _db.track_session on tr.Id equals tr_se.TrackId
                             where tr.Id == id
                             select new TrackSessionViewModel
                             {
                                 Id = tr_se.Id,
                                 CreateDate = tr_se.Date
                             }).ToList();

                return model;
            }
        }

        public List<TrackPosmStatisticViewModel> GetPOSMStatisticByTrackID(string id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var Tr_Session_Details = (from tr in _db.tracks
                                        join tr_se in _db.track_session.DefaultIfEmpty() on tr.Id equals tr_se.TrackId
                                        join td in _db.track_detail.DefaultIfEmpty() on tr_se.Id equals td.TrackSessionId
                                        join mt in _db.media_type.DefaultIfEmpty() on td.MediaTypeId equals mt.Code
                                        where tr.Id == id && MEDIA_TYPE.POSM.Contains(td.MediaTypeId)
                                        select new {
                                            TrackSessionID = td.TrackSessionId,
                                            MediaTypeID = td.MediaTypeId,
                                            MediaTypeName = mt.Name,
                                            createSessionDate =  tr_se.CreatedDate,
                                            posmnumber = td.PosmNumber
                                        }
                                        ).ToList();

                var model = (from tr in Tr_Session_Details 
                            group tr by new { tr.MediaTypeID, tr.MediaTypeName } into tmp
                            select new TrackPosmStatisticViewModel
                            {
                                MediaTypeId = tmp.Key.MediaTypeID,
                                MediaTypeName = tmp.Key.MediaTypeName,
                                PosmNumber = tmp.OrderBy(x=>x.TrackSessionID).FirstOrDefault().posmnumber
                            }).ToList();

                return model;
            }
        }

        public List<TrackDetailViewModel> GetTrackDetailListByTrackSessionId(string id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var model = (from rs in (from ts in _db.track_session
                                         join td in _db.track_detail on ts.Id equals td.TrackSessionId
                                         join mt in _db.media_type on td.MediaTypeId equals mt.Code
                                         where ts.Id == id
                                         select new
                                         {
                                             Id = td.Id,
                                             FileName = td.FileName,
                                             Url = td.Url,
                                             MediaTypeId = td.MediaTypeId,
                                             MediaTypeName = mt.Name,
                                             MediaTypeOrder = mt.OrderNumber,
                                             PosmNumber = td.PosmNumber,
                                             CreateDate = td.CreateDate,
                                             MediaTypeSub = td.MediaTypeSub
                                         })
                             group rs by new
                             {
                                 rs.MediaTypeId,
                                 rs.MediaTypeName,
                                 rs.MediaTypeOrder
                             } into g
                             select new TrackDetailViewModel
                             {
                                 MediaTypeId = g.Key.MediaTypeId,
                                 MediaTypeName = g.Key.MediaTypeName,
                                 MediaTypeOrder = g.Key.MediaTypeOrder,
                                 TrackDetailImages = g.Select(x => new TrackDetailImageViewModel
                                 {
                                     Id = x.Id,
                                     FileName = x.FileName,
                                     Url = x.Url,
                                     PosmNumber = x.PosmNumber,
                                     CreateDate = x.CreateDate,
                                     MediaTypeSub = x.MediaTypeSub
                                 })
                             }).OrderBy(x => x.MediaTypeOrder).ToList();

                return model;
            }
        }

        public track_detail GetSessionDetailById(string id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var model = _db.track_detail.Find(id);
                return model;
            }
        }

        public Byte[] GetExportTrackList()
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var model = (from tr in _db.tracks
                             join em in _db.employees on tr.EmployeeId equals em.Id
                             join st in _db.master_store.DefaultIfEmpty() on tr.MasterStoreId equals st.Id
                             join type in _db.master_store_type.DefaultIfEmpty() on st.StoreType equals type.Id
                             join tr_se in _db.track_session on tr.Id equals tr_se.TrackId

                             join s_p in _db.provinces.DefaultIfEmpty() on st.ProvinceId equals s_p.Id into rs_p
                             from s_p in rs_p.DefaultIfEmpty()
                             join s_d in _db.districts.DefaultIfEmpty() on st.DistrictId equals s_d.Id into rs_d
                             from s_d in rs_p.DefaultIfEmpty()
                             join s_w in _db.wards.DefaultIfEmpty() on st.WardId equals s_w.Id into rs_w
                             from s_w in rs_w.DefaultIfEmpty()
                                 // digix
                             join d_p in _db.provinces.DefaultIfEmpty() on tr.ProvinceId equals d_p.Id into rs_p1
                             from d_p in rs_p1.DefaultIfEmpty()
                             join d_d in _db.districts.DefaultIfEmpty() on tr.DistrictId equals d_d.Id into rs_d1
                             from d_d in rs_d1.DefaultIfEmpty()
                             join d_w in _db.wards.DefaultIfEmpty() on tr.WardId equals d_w.Id into rs_w1
                             from d_w in rs_w1.DefaultIfEmpty()
                             select new TrackExcelViewModel
                             {
                                 Id = tr.Id,
                                 EmployeeId = tr.EmployeeId,
                                 EmployeeName = em.Name,
                                 CreateDate = tr.Date,
                                 StoreStatus = tr.StoreStatus,
                                 Region = tr.Region,
                                 MasterStoreId = tr.MasterStoreId,
                                 Note = tr.Note,
                                 SessionId = tr_se.Id,
                                 SbvpName = tr.MaterStoreName,
                                 SbvpType = type.Name,
                                 SbvpProvince = s_p.Name,
                                 SbvpDistrict = s_d.Type + " " + s_d.Name,
                                 SbvpWard = s_w.Name,
                                 SbvpStreetName = st.StreetNames,
                                 SbvpHouseNumber = st.HouseNumber,
                                 DigixName = tr.MaterStoreName,
                                 DigixType = type.Name,
                                 DigixProvince = d_p.Name,
                                 DigixDistrict = d_d.Type + " " + d_d.Name,
                                 DigixWard = d_w.Name,
                                 DigixStreetName = tr.StreetNames,
                                 DigixHouseNumber = tr.HouseNumber
                             }).OrderByDescending(x => x.CreateDate).ToList();

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
                                                "Ngày thực hiện",
                                                "Khu vực",
                                                "User NV",
                                                "Nhân viên",
                                                "Mã CH",
                                                "Tên CH",
                                                "Loại hình cửa hàng",
                                                "Tỉnh",
                                                "Quận/Huyện",
                                                "Phường/Xã",
                                                "Đường",
                                                "Số nhà",
                                                "Địa chỉ",
                                                "Tên CH",
                                                "Loại hình cửa hàng",
                                                "Tỉnh",
                                                "Quận/Huyện",
                                                "Phường/Xã",
                                                "Đường",
                                                "Số nhà",
                                                "Địa chỉ",
                                                "Tranh Pepsi & 7Up",
                                                "Sticker 7Up",
                                                "Sticker Pepsi",
                                                "Banner Pepsi",
                                                "Banner 7Up Tết",
                                                "Banner Mirinda",
                                                "Banner Twister",
                                                "Banner Revive",
                                                "Banner Oolong",
                                                "Số lần viếng thăm",
                                                "Số lượng ảnh",
                                                "Check in",
                                                "Check out",
                                                "Thời gian làm việc tại CH",
                                                "Tọa độ check in",
                                                "Tọa độ check out",
                                                "Ghi chú NV"
                    };
                    var countColHeader = arrColumnHeader.Count();

                    int colIndex = 1;
                    int rowIndex = 2;

                    foreach (var item in arrColumnHeader)
                    {
                        ws.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
                        
                        colIndex++;
                    }
                    
                    ws.Cells[rowIndex, 6, rowIndex + 1, 6 + 7].Merge = false;
                    ws.Cells[rowIndex, 6, rowIndex, 6 + 7].Merge = true;
                    ws.Cells[rowIndex, 6, rowIndex, 6 + 7].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 6, rowIndex, 6 + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[rowIndex, 6, rowIndex, 6 + 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    
                    ws.Cells[rowIndex, 6 + 8, rowIndex + 1, 6 + 8 + 7].Merge = false;
                    ws.Cells[rowIndex, 6 + 8, rowIndex, 6 + 8 + 7].Merge = true;
                    ws.Cells[rowIndex, 6 + 8, rowIndex, 6 + 8 + 7].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 6 + 8, rowIndex, 6 + 8 + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[rowIndex, 6 + 8, rowIndex, 6 + 8 + 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    
                    ws.Cells[rowIndex, 6 + 8 + 8, rowIndex + 1, 6 + 8 + 7 + 9].Merge = false;
                    ws.Cells[rowIndex, 6 + 8 + 8, rowIndex, 6 + 8 + 7 + 9].Merge = true;
                    ws.Cells[rowIndex, 6 + 8 + 8, rowIndex, 6 + 8 + 7 + 9].Style.Font.Bold = true;
                    ws.Cells[rowIndex, 6 + 8 + 8, rowIndex, 6 + 8 + 7 + 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[rowIndex, 6 + 8 + 8, rowIndex, 6 + 8 + 7 + 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    
                    colIndex = 1;
                    //Creating Headings
                    foreach (var item in arrColumnHeader)
                    {
                        ws.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Style.Font.Bold = true;
                        ws.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        var cell = ws.Cells[rowIndex, colIndex, rowIndex + 1, colIndex];

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

                    ws.Cells[rowIndex, 6].Value = "Thông tin cửa hàng SPVB cung cấp";
                    //Setting the background color of header cells to Gray
                    var fill = ws.Cells[rowIndex, 6].Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(1, 244, 176, 132);

                    ws.Cells[rowIndex, 6 + 8].Value = "Thông tin cửa hàng Digix cập nhật";
                    //Setting the background color of header cells to Gray
                    fill = ws.Cells[rowIndex, 6 + 8].Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(1, 244, 176, 132);

                    ws.Cells[rowIndex, 6 + 8 + 8].Value = "Loại POSM";
                    //Setting the background color of header cells to Gray
                    fill = ws.Cells[rowIndex, 6 + 8 + 8].Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(1, 255, 217, 102);

                    rowIndex++;
                    // Adding Data into rows
                    foreach (var item in model)
                    {
                        colIndex = 1;
                        rowIndex++;

                        var details = (from rs in (from tr in _db.tracks
                                                join ts in _db.track_session on tr.Id equals ts.TrackId
                                                join td in _db.track_detail on ts.Id equals td.TrackSessionId
                                                join mt in _db.media_type on td.MediaTypeId equals mt.Code
                                                where tr.Id == item.Id
                                                select new
                                                {
                                                    Id = td.Id,
                                                    FileName = td.FileName,
                                                    Url = td.Url,
                                                    MediaTypeId = td.MediaTypeId,
                                                    MediaTypeName = mt.Name,
                                                    MediaTypeOrder = mt.OrderNumber,
                                                    PosmNumber = td.PosmNumber,
                                                    CreateDate = td.CreateDate,
                                                    SessionId = ts.Id,
                                                    MediaTypeSub = td.MediaTypeSub
                                                })
                                    group rs by new
                                    {
                                        rs.MediaTypeId,
                                        rs.MediaTypeName,
                                        rs.MediaTypeOrder,
                                        rs.SessionId
                                    } into g
                                    select new TrackDetailViewModel
                                    {
                                        MediaTypeId = g.Key.MediaTypeId,
                                        MediaTypeName = g.Key.MediaTypeName,
                                        MediaTypeOrder = g.Key.MediaTypeOrder,
                                        SessionId = g.Key.SessionId,
                                        TrackDetailImages = g.Select(x => new TrackDetailImageViewModel
                                        {
                                            Id = x.Id,
                                            FileName = x.FileName,
                                            Url = x.Url,
                                            PosmNumber = x.PosmNumber,
                                            CreateDate = x.CreateDate,
                                            MediaTypeSub = x.MediaTypeSub
                                        })
                                    }).ToList();

                        var tmpTRANH_PEPSI_AND_7UP = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.TRANH_PEPSI_AND_7UP && x.SessionId == item.SessionId);
                        var tmpSTICKER_7UP = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.STICKER_7UP && x.SessionId == item.SessionId);
                        var tmpSTICKER_PEPSI = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.STICKER_PEPSI && x.SessionId == item.SessionId);
                        var tmpBANNER_PEPSI = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.BANNER_PEPSI && x.SessionId == item.SessionId);
                        var tmpBANNER_7UP_TET = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.BANNER_7UP_TET && x.SessionId == item.SessionId);
                        var tmpBANNER_MIRINDA = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.BANNER_MIRINDA && x.SessionId == item.SessionId);
                        var tmpBANNER_TWISTER = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.BANNER_TWISTER && x.SessionId == item.SessionId);
                        var tmpBANNER_REVIVE = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.BANNER_REVIVE && x.SessionId == item.SessionId);
                        var tmpBANNER_OOLONG = details.FirstOrDefault(x => x.MediaTypeId == MEDIA_TYPE.BANNER_OOLONG && x.SessionId == item.SessionId);

                        //var sessions = _db.track_session.Where(x => x.TrackId == item.Id).ToList();

                        //Setting Value in cell
                        ws.Cells[rowIndex, colIndex].Value = item.CreateDate.ToString("dd-MM-yyyy");
                        //Setting Top/left,right/bottom borders.
                        var border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.Region;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.EmployeeId;
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

                        ws.Cells[rowIndex, colIndex].Value = item.MasterStoreId;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.SbvpName;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.SbvpType;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.SbvpProvince;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.SbvpDistrict;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.SbvpWard;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.SbvpStreetName;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.SbvpHouseNumber;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = $"{item.SbvpHouseNumber} {item.SbvpStreetName}, {item.SbvpWard}, {item.SbvpDistrict}, {item.SbvpProvince}";//Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.DigixName;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.DigixType;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.DigixProvince;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.DigixDistrict;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =

                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.DigixWard;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.DigixStreetName;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =

                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.DigixHouseNumber;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = $"{item.DigixHouseNumber} {item.DigixStreetName}, {item.DigixWard}, {item.DigixDistrict}, {item.DigixProvince}";
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = tmpTRANH_PEPSI_AND_7UP == null ? 0 : tmpTRANH_PEPSI_AND_7UP.TrackDetailImages.Sum(x => x.PosmNumber);
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = tmpSTICKER_7UP == null ? 0 : tmpSTICKER_7UP.TrackDetailImages.Sum(x => x.PosmNumber);
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = tmpSTICKER_PEPSI == null ? 0 : tmpSTICKER_PEPSI.TrackDetailImages.Sum(x => x.PosmNumber);
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = tmpBANNER_PEPSI == null ? 0 : tmpBANNER_PEPSI.TrackDetailImages.Sum(x => x.PosmNumber);
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = tmpBANNER_7UP_TET == null ? 0 : tmpBANNER_7UP_TET.TrackDetailImages.Sum(x => x.PosmNumber);
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = tmpBANNER_MIRINDA == null ? 0 : tmpBANNER_MIRINDA.TrackDetailImages.Sum(x => x.PosmNumber);
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = tmpBANNER_TWISTER == null ? 0 : tmpBANNER_TWISTER.TrackDetailImages.Sum(x => x.PosmNumber);
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = tmpBANNER_REVIVE == null ? 0 : tmpBANNER_REVIVE.TrackDetailImages.Sum(x => x.PosmNumber);
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = tmpBANNER_OOLONG == null ? 0 : tmpBANNER_OOLONG.TrackDetailImages.Sum(x => x.PosmNumber);
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = "Chưa biết";//sessions.Count;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        var tmp = 0;
                        foreach (var detail in details.Where(x => x.SessionId == item.SessionId))
                        {
                            tmp += detail.TrackDetailImages.Count();
                        }
                        ws.Cells[rowIndex, colIndex].Value = tmp;
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        var start = details.FirstOrDefault(x => x.SessionId == item.SessionId && x.TrackDetailImages.FirstOrDefault(y => y.MediaTypeSub == "HINH_TONG_QUAT") != null);

                        ws.Cells[rowIndex, colIndex].Value = start != null ? start.TrackDetailImages.FirstOrDefault(y => y.MediaTypeSub == "HINH_TONG_QUAT").CreateDate.ToString("hh:mm:ss") : ""; // Giờ chụp hình tổng quan
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        var end = details.FirstOrDefault(x => x.SessionId == item.SessionId && x.MediaTypeId == "SELFIE" && x.TrackDetailImages.Any());

                        ws.Cells[rowIndex, colIndex].Value = end != null ? end.TrackDetailImages.FirstOrDefault().CreateDate.ToString("hh:mm:ss") : ""; // giờ chụp hình chấm công đầu ra
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = ((start != null && end != null) ? (end.TrackDetailImages.FirstOrDefault().CreateDate - start.TrackDetailImages.FirstOrDefault(y => y.MediaTypeSub == "HINH_TONG_QUAT").CreateDate).ToString() : ""); // giờ chụp hình chấm công đầu ra
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;
                        
                        ws.Cells[rowIndex, colIndex].Value = "tọa độ in (chưa có)";
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = "tọa độ out (chưa có)";
                        //Setting Top/left,right/bottom borders.
                        border = ws.Cells[rowIndex, colIndex++].Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        ws.Cells[rowIndex, colIndex].Value = item.Note;
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


        public MessageReturnModel DeleteTrackDetail(string id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var detail = _db.track_detail.Find(id);
                if (detail != null)
                {
                    FileHelper.RemoveFileFromServer(detail.Url + detail.FileName); // remove old file

                    _db.track_detail.Remove(detail);
                    _db.SaveChanges();

                    return new MessageReturnModel
                    {
                        IsSuccess = true
                    };
                }
                else
                {
                    return new MessageReturnModel
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy hình ảnh!"
                    };
                }
            }
        }

        public MessageReturnModel DeleteTrackSession(string id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var session = _db.track_session.Find(id);
                if (session != null)
                {
                    var details = _db.track_detail.Where(x => x.TrackSessionId == id).ToList();
                    foreach (var item in details)
                    {
                        FileHelper.RemoveFileFromServer(item.Url + item.FileName); // remove old file
                        _db.track_detail.Remove(item);
                    }

                    _db.track_session.Remove(session);
                    _db.SaveChanges();

                    return new MessageReturnModel
                    {
                        IsSuccess = true
                    };
                }
                else
                {
                    return new MessageReturnModel
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy gói!"
                    };
                }
            }
        }

        public MessageReturnModel UpdateSessionDetail(track_detail model)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var detail = _db.track_detail.Find(model.Id);
                if (detail != null)
                {
                    detail.FileName = model.FileName;
                    detail.Url = model.Url;
                    detail.MediaTypeId = model.MediaTypeId;
                    detail.EmployeeId = model.EmployeeId;
                    detail.IsActive = model.IsActive;
                    detail.TrackSessionId = model.TrackSessionId;
                    detail.PosmNumber = model.PosmNumber;

                    _db.SaveChanges();

                    return new MessageReturnModel
                    {
                        IsSuccess = true,
                        Id = detail.Id
                    };
                }

                return new MessageReturnModel
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy!"
                };
            }
        }
    }
}
