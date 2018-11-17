using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeTracking.Data.ModelCustom
{
    public class FileUploadModel
    {
        public string TypeId { get; set; }
        public string TypeName { get; set; }
        public int PosmNumber { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}