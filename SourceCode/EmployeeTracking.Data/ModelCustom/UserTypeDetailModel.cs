namespace EmployeeTracking.Data.ModelCustom
{
    public class UserTypeDetailModel
    {
        public int Id { get; set; }

        public string UserType { get; set; } = "";

        public string RoleCode { get; set; } = "";

        public string RoleName { get; set; } = "";

        public string FormCode { get; set; } = "";

        public string FormName { get; set; } = "";

        public bool Selected { get; set; } = false;
    }
}
