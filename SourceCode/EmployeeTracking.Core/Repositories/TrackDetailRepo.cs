using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            catch(Exception ex)
            {
                return new MessageReturnModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
