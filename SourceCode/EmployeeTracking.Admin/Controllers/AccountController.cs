﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmployeeTracking.Data.Database;
using EmployeeTracking.Data.ModelCustom;
using EmployeeTracking.Core;
using EmployeeTracking.Core.Repositories;
using EmployeeTracking.Admin.Filters;

namespace EmployeeTracking.Admin.Controllers
{
    [AllowAnonymous]
    public class AccountController : BasicController
    {
        // GET: Account
        private UsersRepo _usersRepo;
        public AccountController()
        {
            _usersRepo = new UsersRepo();
        }
        public ActionResult Login()
        {
            var useraccount = CheckCookie();

            if (!string.IsNullOrEmpty(useraccount.UserName))
                useraccount.Remember = true;
            return View(useraccount);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(string username = "", string password = "", int Remember = 0)
        {
            try
            {
                var acc = _usersRepo.Login(username, password);
                //var acc = new Tuple<user, string>(new user() {
                //    UserName = username,5
                //    PasswordHash = password
                //}, "");
                
                AccountModel accM = new AccountModel();
                accM.Remember = (Remember == 1);
                if ((username ?? "").Trim().Length == 0 || (password ?? "").Trim().Length == 0)
                {
                    SetMessage(TempData, "Tên đăng nhập hoặc mật khẩu không được để trống !");
                    return View(accM);
                }
                else
                {
                    if (acc.Item1 != null)
                    {
                        TempData["MessagePage"] = null;
                        Session["Account"] = acc.Item1;

                        List<RoleUserTypeViewModel> lstRoleUserType = _usersRepo.GetRoleByUserType(acc.Item1.UserType);
                        List<String> lstRole = new List<string>();
                        for (int i = 0; i < lstRoleUserType.Count; i++)
                        {
                            lstRole.Add(lstRoleUserType[i].RoleCode);
                        }
                        Session["Roles"] = lstRole;

                        if (Remember == 1)
                        {
                            Response.Cookies.Add(new HttpCookie("username")
                            {
                                Expires = DateTime.Now.AddDays(14d),
                                Value = Remember == 1 ? username : ""
                            });
                            Response.Cookies.Add(new HttpCookie("password")
                            {
                                Expires = DateTime.Now.AddDays(14d),
                                Value = Remember == 1 ? password : ""
                            });
                        }
                        else
                        {
                            Response.Cookies.Add(new HttpCookie("username")
                            {
                                Expires = DateTime.Now.AddDays(14d),
                                Value = ""
                            });
                            Response.Cookies.Add(new HttpCookie("password")
                            {
                                Expires = DateTime.Now.AddDays(14d),
                                Value = ""
                            });
                        }
                        return ToHome();
                    }
                    else
                    {
                        SetMessage(TempData, acc.Item2);

                        Response.Cookies.Add(new HttpCookie("username")
                        {
                            Expires = DateTime.Now.AddDays(14d),
                            Value = ""
                        });
                        Response.Cookies.Add(new HttpCookie("password")
                        {
                            Expires = DateTime.Now.AddDays(14d),
                            Value = ""
                        });

                        return View(accM);
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Account");
            }


        }
        private AccountModel CheckCookie()
        {
            AccountModel temp = new AccountModel();

            string username = "";
            string password = "";
            if (Request.Cookies["username"] != null)
            {
                username = Request.Cookies["username"].Value;
            }
            if (Request.Cookies["password"] != null)
            {
                password = Request.Cookies["password"].Value;
            }
            if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
            {
                temp = new AccountModel()
                {
                    UserName = username,
                    PasswordHash = password
                };
            }
            return temp;
        }


        public ActionResult Logout()
        {
            Session["Account"] = null;
            return ToHome();
        }
    }
}