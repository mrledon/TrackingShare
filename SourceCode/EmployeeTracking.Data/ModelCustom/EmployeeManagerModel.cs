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
    public class EmployeeManagerModel
    {
        [DataMember]
        [Display(Name = "ID")]
        public string Id { get; set; }
        [DataMember]
        [Required(ErrorMessage = "{0} là bắt buộc!")]
        [Display(Name = "Mã nhân viên")]
        public string Code { get; set; }
        [DataMember]
        [Required(ErrorMessage = "{0} là bắt buộc!")]
        [Display(Name = "Tên nhân viên")]
        public string Name { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public bool? Gender { get; set; }
        public string GenderString { get; set; }
        [DataMember]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Birthday { get; set; }
        public string BirthdayString { get; set; }
        [DataMember]
        public string IdentityCard { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public string Owner { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public DateTime? CreatedDate { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public DateTime? ModifiedDate { get; set; }
        public int Index { get; set; }
        public bool IsEdit { get; set; }
    }
}
