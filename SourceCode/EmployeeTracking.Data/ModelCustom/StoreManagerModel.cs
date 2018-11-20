using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Data.ModelCustom
{
    [DataContract]
    public class StoreManagerModel
    {
        [DataMember]
        [Display(Name = "ID")]
        public Guid? Id { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public DateTime? CreatedDate { get; set; }
        [DataMember]
        public DateTime? ModifiedDate { get; set; }
        [DataMember]
        public bool? IsActive { get; set; }
        [DataMember]
        [Required(ErrorMessage = "{0} là bắt buộc!")]
        [Display(Name = "Mã cửa hàng")]
        public string Code { get; set; }
        [DataMember]
        [Required(ErrorMessage = "{0} là bắt buộc!")]
        [Display(Name = "Tên cửa hàng")]
        public string Name { get; set; }
        [DataMember]
        //[Required(ErrorMessage = "{0} là bắt buộc!")]
        [Display(Name = "Loại cửa hàng")]
        public string StoreType { get; set; }
        public string StoreTypeName { get; set; }
        [DataMember]
        public string HouseNumber { get; set; }
        [DataMember]
        public string StreetNames { get; set; }
        [DataMember]
        public long? ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        [DataMember]
        public long? DistrictId { get; set; }
        public string DistrictName { get; set; }
        [DataMember]
        public long? WardId { get; set; }
        public string WardName { get; set; }
        [DataMember]
        public string Region { get; set; }
        [DataMember]
        public double? LAT { get; set; }
        [DataMember]
        public double? LNG { get; set; }
        [DataMember]
        public Guid? MasterStoreDetailId { get; set; }
        public int Index { get; set; }
        public bool IsEdit { get; set; }
        public string PhoneNumber { get; set; }
    }
}
