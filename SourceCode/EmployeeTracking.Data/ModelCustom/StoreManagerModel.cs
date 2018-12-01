using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Data.ModelCustom
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class StoreManagerModel
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        [Display(Name = "ID")]
        public Guid? Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string CreatedBy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ModifiedBy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime? CreatedDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime? ModifiedDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool? IsActive { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        [Required(ErrorMessage = "{0} là bắt buộc!")]
        [Display(Name = "Mã cửa hàng")]
        public string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        [Required(ErrorMessage = "{0} là bắt buộc!")]
        [Display(Name = "Tên cửa hàng")]
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        //[Required(ErrorMessage = "{0} là bắt buộc!")]
        [Display(Name = "Loại cửa hàng")]
        public string StoreType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string StoreTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string HouseNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string StreetNames { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public long? ProvinceId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public long? DistrictId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DistrictName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public long? WardId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string WardName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Region { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? LAT { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public double? LNG { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid? MasterStoreDetailId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsEdit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}
