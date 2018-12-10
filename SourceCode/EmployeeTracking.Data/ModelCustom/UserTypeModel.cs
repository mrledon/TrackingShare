using System.Collections.Generic;

namespace EmployeeTracking.Data.ModelCustom
{
    public class UserTypeModel
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<UserTypeDetailModel> details { get; set; } = new List<UserTypeDetailModel>();
    }
}
