using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace EmployeeTracking.Core.Repositories
{
    public class TrackDetailRepo
    {
        public string Insert(track_detail model)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                _db.track_detail.Add(model);
                _db.SaveChanges();
                return model.Id;
            }
        }

        public MessageReturnModel InsertImageAdmin(AddImageModel model)
        {
            try
            {
                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
                    track_session trackSession = new track_session();
                    trackSession.Id = Guid.NewGuid().ToString();
                    trackSession.CreatedBy = model.CreateBy;
                    trackSession.CreatedDate = model.CreateDate;
                    trackSession.Date = model.DateUpdate;
                    trackSession.Status = true;
                    trackSession.TrackId = model.TrackId;
                    _db.track_session.Add(trackSession);
                    _db.SaveChanges();
                    foreach (var fileUpload in model.FileUploads)
                    {
                        track_detail trackDetail = new track_detail();
                        trackDetail.Id = Guid.NewGuid().ToString();
                        trackDetail.CreateBy = model.CreateBy;
                        trackDetail.CreateDate = model.CreateDate;
                        trackDetail.EmployeeId = model.EmployeeId;
                        trackDetail.FileName = fileUpload.FileName;
                        trackDetail.IsActive = true;
                        trackDetail.MediaTypeId = fileUpload.TypeId;
                        trackDetail.MediaTypeSub = fileUpload.SubType;
                        trackDetail.PosmNumber = fileUpload.PosmNumber;
                        trackDetail.TrackSessionId = trackSession.Id;
                        trackDetail.Url = fileUpload.FilePath;
                        _db.track_detail.Add(trackDetail);
                        _db.SaveChanges();
                    }
                }
                return new MessageReturnModel
                {
                    IsSuccess = true,
                    Message = ""
                };
            }
            catch (Exception ex)
            {
                return new MessageReturnModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public MessageReturnModel UpdateUnSubmitTrackSession(string sessionId, AddImageModel model)
        {
            try
            {
                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
                    track_session trackSession = _db.track_session.FirstOrDefault(m => m.Id == sessionId);
                    if (trackSession == null)
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Không tìm thấy thông tin"
                        };
                    }
                    trackSession.Status = true;
                    _db.SaveChanges();

                    foreach (var fileUpload in model.FileUploads)
                    {
                        track_detail trackDetail = new track_detail();
                        var currentTrack = _db.track_detail.Where(m => m.TrackSessionId == sessionId && m.MediaTypeId == fileUpload.TypeId).Select(m => m.Id).ToList();
                        foreach (var item in currentTrack)
                        {
                            trackDetail = _db.track_detail.FirstOrDefault(m => m.Id == item);
                            if (trackDetail != null)
                            {
                                trackDetail.PosmNumber = fileUpload.PosmNumber;

                                _db.track_detail.Attach(trackDetail);
                                _db.SaveChanges();
                            }
                        }

                        if (fileUpload.FileId == null || fileUpload.FileId.Length > 0)
                        {
                            trackDetail = _db.track_detail.FirstOrDefault(m => m.Id == fileUpload.FileId);
                            if (trackDetail != null)
                            {


                                FileHelper.RemoveFileFromServer(WebConfigurationManager.AppSettings["rootMedia"] + trackDetail.Url + trackDetail.FileName); // remove old file
                                FileHelper.RemoveFileFromServer(WebConfigurationManager.AppSettings["rootMedia"] + "/WriteText" + trackDetail.Url + trackDetail.FileName); // remove old file

                                trackDetail.FileName = fileUpload.FileName;
                                trackDetail.PosmNumber = fileUpload.PosmNumber;
                                trackDetail.Url = fileUpload.FilePath;

                                _db.track_detail.Attach(trackDetail);
                                _db.SaveChanges();

                                continue;
                            }
                        }
                        trackDetail = new track_detail();
                        trackDetail.Id = Guid.NewGuid().ToString();
                        trackDetail.CreateBy = model.CreateBy;
                        trackDetail.CreateDate = model.CreateDate;
                        trackDetail.EmployeeId = model.EmployeeId;
                        trackDetail.IsActive = true;
                        trackDetail.FileName = fileUpload.FileName;
                        trackDetail.MediaTypeId = fileUpload.TypeId;
                        trackDetail.MediaTypeSub = fileUpload.SubType;
                        trackDetail.PosmNumber = fileUpload.PosmNumber;
                        trackDetail.TrackSessionId = sessionId;
                        trackDetail.Url = fileUpload.FilePath;

                        _db.track_detail.Add(trackDetail);
                        _db.SaveChanges();
                    }
                }
                return new MessageReturnModel
                {
                    IsSuccess = true,
                    Message = ""
                };
            }
            catch (Exception ex)
            {
                return new MessageReturnModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        /*Hieu.pt Update Posm in track_detail*/
        public MessageReturnModel Update(TrackPosmStatisticViewModel model)
        {
            try
            {
                using (employeetracking_devEntities _data = new employeetracking_devEntities())
                {
                    List<track_detail> updateModel = _data.track_detail.Where(x => x.TrackSessionId == model.TrackSessionId && x.MediaTypeId == model.MediaTypeId).ToList();
                    if (updateModel.Count > 0)
                    {
                        for (int i = 0; i < updateModel.Count; i++)
                        {
                            updateModel[i].PosmNumber = model.PosmNumber;
                        }
                        _data.SaveChanges();
                        return new MessageReturnModel
                        {
                            IsSuccess = true,
                            Message = "Cập nhật số lượng Posm thành công"
                        };
                    }
                    else
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Cập nhật số lượng Posm không thành công"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new MessageReturnModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public MessageReturnModel SavePosmType(string trackSessionsId, string mediaTypeId, int ValuePosmOfMediaType, string OldMediaTypeId)
        {
            try
            {
                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
                    List<track_detail> updateModel = _db.track_detail.Where(x => x.TrackSessionId == trackSessionsId && !x.MediaTypeId.Contains("DEFAULT") && !x.MediaTypeId.Contains("SELFIE") && !x.MediaTypeId.Contains("STORE_FAILED") && x.MediaTypeId.Contains(OldMediaTypeId)).ToList();
                    if (updateModel.Count > 0)
                    {
                        for (int i = 0; i < updateModel.Count; i++)
                        {
                            updateModel[i].MediaTypeId = mediaTypeId;
                            updateModel[i].PosmNumber = ValuePosmOfMediaType;
                        }
                        _db.SaveChanges();
                        return new MessageReturnModel
                        {
                            IsSuccess = true,
                            Message = "Cập nhật loại Posm thành công"
                        };
                    }
                    else
                    {
                        return new MessageReturnModel
                        {
                            IsSuccess = false,
                            Message = "Cập nhật loại Posm không thành công"
                        };
                    }
                }


            }
            catch (Exception ex)
            {
                return new MessageReturnModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public TrackDetailImageViewModel GetTrackSessionById(string id)
        {
            using (employeetracking_devEntities _db = new employeetracking_devEntities())
            {
                var model = (from tr_se in _db.track_session
                             join tr in _db.tracks on tr_se.TrackId equals tr.Id
                             where tr_se.Id == id
                             select new { tracksessions = tr_se, track = tr }).FirstOrDefault();

                TrackDetailImageViewModel value = new TrackDetailImageViewModel()
                {
                    EmployeeId = model.track.EmployeeId,
                    TrackSessionId = model.tracksessions.Id,
                    MasterStoreId = model.track.MasterStoreId.ToString(),
                    TrackId = model.track.Id
                };
                return value;
            }
        }

        public MessageReturnModel InsertImageForTrackDetail(AddImageModel model)
        {
            try
            {
                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
                    foreach (var fileUpload in model.FileUploads)
                    {
                        track_detail trackDetail = new track_detail();
                        trackDetail.Id = Guid.NewGuid().ToString();
                        trackDetail.CreateBy = model.CreateBy;
                        trackDetail.CreateDate = model.CreateDate;
                        trackDetail.EmployeeId = model.EmployeeId;
                        trackDetail.FileName = fileUpload.FileName;
                        trackDetail.IsActive = true;
                        trackDetail.MediaTypeId = fileUpload.TypeId;
                        trackDetail.MediaTypeSub = fileUpload.SubType;
                        trackDetail.PosmNumber = fileUpload.PosmNumber;
                        trackDetail.TrackSessionId = model.TrackSessionId;
                        trackDetail.Url = fileUpload.FilePath;
                        trackDetail.MediaTypeSub = model.TypeSub;
                        _db.track_detail.Add(trackDetail);
                        _db.SaveChanges();
                    }
                }
                return new MessageReturnModel
                {
                    IsSuccess = true,
                    Id = model.TrackSessionId,
                    Message = "Thêm hình ảnh thành công"
                };
            }
            catch (Exception ex)
            {
                return new MessageReturnModel
                {
                    IsSuccess = false,
                    Id = model.TrackSessionId,
                    Message = "Thêm hình ảnh không thành công"
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public MessageReturnModel SavePOSM(POSMTrackModel model)
        {
            try
            {
                using (employeetracking_devEntities _db = new employeetracking_devEntities())
                {
                    #region " [ Save master store ] "

                    var _store = _db.master_store.FirstOrDefault(m => m.Code == model.StoreCode);
                    if (_store == null)
                    {
                        _store = new master_store();
                        _store.Id = Guid.NewGuid();
                        _store.CreatedBy = model.CreateBy;
                        _store.CreatedDate = DateTime.Now;
                        _store.Code = model.StoreCode;
                        _store.Name = model.StoreName;
                        _store.StoreType = model.StoreType;
                        _store.HouseNumber = model.HouseNumber;
                        _store.StreetNames = model.StreetName;
                        _store.ProvinceId = model.ProvinceId;
                        _store.DistrictId = model.DistrictId;
                        _store.WardId = model.WardId;
                        _store.Region = "";
                        _store.LAT = model.LAT;
                        _store.LNG = model.LNG;
                        _store.PhoneNumber = model.PhoneNumber;
                        _db.master_store.Add(_store);
                        _db.Entry(_store).State = System.Data.EntityState.Added;
                        _db.SaveChanges();
                    }

                    #endregion

                    var _tr = _db.tracks.FirstOrDefault(_ => _.Date.Year == model.Date.Year 
                                                            && _.Date.Month == model.Date.Month 
                                                            && _.Date.Day == model.Date.Day 
                                                            && _.MasterStoreId == _store.Id 
                                                            && _.EmployeeId == model.EmployeeId);

                    #region " [ Track ] "

                    if(_tr == null)
                    {
                        _tr = new track();
                        _tr.Id = Guid.NewGuid().ToString();
                        _tr.EmployeeId = model.EmployeeId;
                        _tr.CreateDate = DateTime.Now;
                        _tr.Date = model.Date;
                        _tr.MaterStoreName = _store.Name;
                        _tr.HouseNumber = model.HouseNumber;
                        _tr.StreetNames = model.StreetName;
                        _tr.ProvinceId = model.ProvinceId;
                        _tr.DistrictId = model.DistrictId;
                        _tr.WardId = model.WardId;
                        _tr.Region = "";
                        _tr.Lat = model.LAT.HasValue ? model.LAT.ToString() : "";
                        _tr.Lng = model.LNG.HasValue ? model.LNG.ToString() : "";
                        _tr.Note = model.Notes;
                        _tr.MasterStoreId = _store.Id;
                        _tr.StoreStatus = model.Success;
                        _tr.PhoneNumber = model.PhoneNumber;
                        _tr.StoreIsChanged = model.StoreIsChange;
                        _tr.StoreType = model.StoreType;
                        _tr.QCNote = "";
                        _db.tracks.Add(_tr);
                        _db.Entry(_tr).State = System.Data.EntityState.Added;
                        _db.SaveChanges();
                    }

                    #endregion

                    #region " [ Track session ] "

                    track_session _trSession = new track_session();
                    _trSession.Id = Guid.NewGuid().ToString();
                    _trSession.CreatedBy = model.CreateBy.ToString();
                    _trSession.CreatedDate = DateTime.Now;
                    _trSession.TrackId = _tr.Id;
                    _trSession.Date = model.Date;
                    _trSession.Status = true;

                    _db.track_session.Add(_trSession);
                    _db.Entry(_trSession).State = System.Data.EntityState.Added;
                    _db.SaveChanges();

                    #endregion

                    #region " [ Track detail ] "

                    foreach (var fileUpload in model.FileUploads)
                    {
                        track_detail trackDetail = new track_detail();
                        trackDetail.Id = Guid.NewGuid().ToString();
                        trackDetail.CreateBy = model.CreateBy;
                        trackDetail.CreateDate = model.Date;
                        trackDetail.EmployeeId = model.EmployeeId;
                        trackDetail.FileName = fileUpload.FileName;
                        trackDetail.IsActive = true;
                        trackDetail.MediaTypeId = fileUpload.TypeId;
                        trackDetail.MediaTypeSub = fileUpload.SubType;
                        trackDetail.PosmNumber = fileUpload.PosmNumber;
                        trackDetail.TrackSessionId = _trSession.Id;
                        trackDetail.Url = fileUpload.FilePath;
                        trackDetail.MediaTypeSub = fileUpload.SubType;
                        _db.track_detail.Add(trackDetail);
                        _db.Entry(trackDetail).State = System.Data.EntityState.Added;
                        _db.SaveChanges();
                    }

                    #endregion

                }
                return new MessageReturnModel
                {
                    IsSuccess = true,
                    Message = "Thêm hình ảnh thành công"
                };
            }
            catch (Exception ex)
            {
                return new MessageReturnModel
                {
                    IsSuccess = false,
                    Message = "Thêm hình ảnh không thành công"
                };
            }
        }
    }
}
