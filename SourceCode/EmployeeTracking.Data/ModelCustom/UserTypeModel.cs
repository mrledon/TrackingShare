using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeeTracking.Data.ModelCustom
{
    public class UserTypeModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã không được rỗng")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Tên không được rỗng")]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool Insert { get; set; } = false;

        public List<UserTypeDetailModel> details { get; set; } = new List<UserTypeDetailModel>();
    }
}
