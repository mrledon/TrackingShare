using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeTracking.Data.ModelCustom
{
    public class AddImageModel
    {
        public string TrackId { get; set; }
        public string EmployeeId { get; set; }
        public string MasterStoreId { get; set; }
        public DateTime DateUpdate { get; set; }
        public List<FileUploadModel> FileUploads { get; set; }
    }
}