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


        private EmployeeRepo _EmployeeRepo;

        public MobileController()
        {
            _EmployeeRepo = new EmployeeRepo();
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
                throw new Exception();
                //var obj = _EmployeeRepo.Attendance(model);
                //return Json(
                //   new JsonResultModel<EmployeeApiModel>()
                //   {
                //       HasError = false,
                //       Message = string.Empty,
                //       Data = obj
                //   });
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

        #endregion




        #region TRACKING DETAIL

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