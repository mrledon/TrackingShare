﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Data.ModelCustom
{
    public class UserViewModel
    {
        public long Id { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ ")]
        [Required(ErrorMessage = "Email không được rỗng")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được rỗng")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được rỗng")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không hợp lệ")]
        public string PasswordConfirm { get; set; }

        public string PhoneNumber { get; set; }

        public string FullName { get; set; }

        public string UserTypeCode { get; set; }

        public string UserTypeName { get; set; }

        public bool? IsActive { get; set; }

        public bool IsEdit { get; set; }

        public string NewPassWord { get; set; }

        [Compare("NewPassWord", ErrorMessage = "Mật khẩu xác nhận không hợp lệ")]
        public string NewPassWordonfirm { get; set; }

    }
}
