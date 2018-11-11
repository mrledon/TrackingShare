using EmployeeTracking.Core.Repositories;
using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;
using EmployeeTracking.Data.ModelCustom.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EmployeeTracking.API.Controllers
{
    public class MobileController : ApiController
    {


        private DistrictRepo _DistrictRepo;
        private ProvinceRepo _ProvinceRepo;
        private WardRepo _WardRepo;
        private EmployeeRepo _EmployeeRepo;
        private TrackAttendanceRepo _TrackAttendanceRepo;

        public MobileController()
        {
            _EmployeeRepo = new EmployeeRepo();
            _TrackAttendanceRepo = new TrackAttendanceRepo();
            _DistrictRepo = new DistrictRepo();
            _ProvinceRepo = new ProvinceRepo();
            _WardRepo = new WardRepo();
        }


        // GET api/<controller>
        public object Get()
        {
            //return _db.employee.ToList();

            return null;
        }

        #region LOGIN

        [HttpPost]
        public object Login(employee model)
        {
            try
            {
                var obj = _EmployeeRepo.LoginAPI(model);
                return Json(
                   new JsonResultModel<EmployeeApiModel>()
                   {
                       HasError = false,
                       Message = string.Empty,
                       Data = obj
                   });
            }
            catch (Exception ex)
            {
                return Json(
                    new JsonResultModel<EmployeeApiModel>()
                    {
                        HasError = true,
                        Message = ex.Message,
                        Data = new EmployeeApiModel()
                        {

                        }
                    });
            }
        }
        #endregion


        #region ATTENDANCE
        [HttpPost]
        public object Attendance(AttendanceApiModel model)
        {
            try
            {
                var date = DateTime.Now;

                var emp = _EmployeeRepo.CheckToken(model.Id, model.Token);
                if (emp == null)
                    throw new Exception("Employee not found. Pleae reconnect.");

                if (string.IsNullOrEmpty(model.AttendanceStart) && string.IsNullOrEmpty(model.AttendanceEnd))
                    throw new Exception("Please input Start or End value");
                else if (!string.IsNullOrEmpty(model.AttendanceStart) && string.IsNullOrEmpty(model.AttendanceEnd))
                {
                    TimeSpan time = TimeSpan.Parse(model.AttendanceStart); //HH:mm:ss
                    _TrackAttendanceRepo.AttendanceStart(new track_attendance()
                    {
                        Date = date,
                        EmployeeId = emp.Id,
                        Start = time
                    });

                    return Json(
                    new JsonResultModel<EmployeeApiModel>()
                    {
                        HasError = false,
                        Message = string.Empty,
                        Data = new EmployeeApiModel()
                        {

                        }
                    });
                }
                else if (!string.IsNullOrEmpty(model.AttendanceEnd) && string.IsNullOrEmpty(model.AttendanceStart))
                {
                    TimeSpan time = TimeSpan.Parse(model.AttendanceEnd); //HH:mm:ss
                    _TrackAttendanceRepo.AttendanceEnd(new track_attendance()
                    {
                        Date = date,
                        EmployeeId = emp.Id,
                        End = time
                    });

                    return Json(
                    new JsonResultModel<EmployeeApiModel>()
                    {
                        HasError = false,
                        Message = string.Empty,
                        Data = new EmployeeApiModel()
                        {

                        }
                    });
                }
                else
                    throw new Exception("");
            }
            catch (Exception ex)
            {
                return Json(
                    new JsonResultModel<EmployeeApiModel>()
                    {
                        HasError = true,
                        Message = ex.Message,
                        Data = new EmployeeApiModel()
                        {

                        }
                    });
            }
        }
        #endregion



        #region TRACKING
        public object Provinces()
        {
            try
            {
                var obj = _ProvinceRepo.GetAll();
                return Json(
                   new JsonResultModel<IList<province>>()
                   {
                       HasError = false,
                       Message = string.Empty,
                       Data = obj
                   });
            }
            catch (Exception ex)
            {
                return Json(
                    new JsonResultModel<IList<province>>()
                    {
                        HasError = true,
                        Message = ex.Message,
                        Data = new List<province>()
                    });
            }
        }
        public object Districts(DistrictApiModel model)
        {
            try
            {
                var obj = _DistrictRepo.GetByProvinceId(model.ProvinceId);
                return Json(
                   new JsonResultModel<IList<district>>()
                   {
                       HasError = false,
                       Message = string.Empty,
                       Data = obj
                   });
            }
            catch (Exception ex)
            {
                return Json(
                    new JsonResultModel<IList<district>>()
                    {
                        HasError = true,
                        Message = ex.Message,
                        Data = new List<district>()
                    });
            }
        }
        public object Wards(WardApiModel model)
        {
            try
            {
                var obj = _WardRepo.GetByDistrictId(model.DistrictId);
                return Json(
                   new JsonResultModel<IList<ward>>()
                   {
                       HasError = false,
                       Message = string.Empty,
                       Data = obj
                   });
            }
            catch (Exception ex)
            {
                return Json(
                    new JsonResultModel<IList<ward>>()
                    {
                        HasError = true,
                        Message = ex.Message,
                        Data = new List<ward>()
                    });
            }
        }
        #endregion




        #region TRACKING DETAIL
        //public object (WardApiModel model)
        //{
        //    try
        //    {
        //        var obj = _WardRepo.GetByDistrictId(model.DistrictId);
        //        return Json(
        //           new JsonResultModel<IList<ward>>()
        //           {
        //               HasError = false,
        //               Message = string.Empty,
        //               Data = obj
        //           });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(
        //            new JsonResultModel<IList<ward>>()
        //            {
        //                HasError = true,
        //                Message = ex.Message,
        //                Data = new List<ward>()
        //            });
        //    }
        //}
        #endregion



        #region GET WARD

        #endregion


        #region GET PROVINCE

        #endregion



        #region GET DISTRICT

        #endregion








        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}