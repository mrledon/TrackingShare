using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Data.ModelCustom
{
    public class POSMTrackModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Ngày không được rỗng")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Mã cửa hàng không được rỗng")]
        public string StoreCode { get; set; }

        [Required(ErrorMessage = "Tên cửa hàng không được rỗng")]
        public string StoreName { get; set; }

        public string PhoneNumber { get; set; }

        public string HouseNumber { get; set; }

        public string StreetName { get; set; }

        public long? ProvinceId { get; set; }

        public long? DistrictId { get; set; }

        public long? WardId { get; set; }

        public string Notes { get; set; }

        public string StoreType { get; set; }

        public double? LNG { get; set; }

        public double? LAT { get; set; }

        public bool Success { get; set; } = false;

        public bool UnSuccess { get; set; } = true;

        public List<FileUploadModel> FileUploads { get; set; } = new List<FileUploadModel>();

        public string CreateBy { get; set; } = "";

    }
}
