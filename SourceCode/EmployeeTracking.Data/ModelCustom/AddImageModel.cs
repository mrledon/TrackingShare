using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmployeeTracking.Data.ModelCustom
{
    public class AddImageModel
    {
        public string TrackId { get; set; }
        public string EmployeeId { get; set; }
        public string MasterStoreId { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateUpdate { get; set; }
        public List<FileUploadModel> FileUploads { get; set; }
        public string TrackSessionId { get; set; }
        public string TypeSub { get; set; }
    }
}