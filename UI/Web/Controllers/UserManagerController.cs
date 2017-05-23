using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Service.Implement;
using Service.Interface;
using Service.Security;
using Service.Users;
using Web.Extend;
using Web.Models;
using Web.Models.UserManager;
using Entities.Domain.Users;
using Web.Models.Common;
using Service.Departments;
using Web.Models.Department;
using Nois.Web.Framework.Kendoui;
using System.IO;

namespace Web.Controllers
{
    [Authorize]
    public class UserManagerController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IUserRoleService _userRoleService;
        private readonly IPermissionService _permissionService;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IEncryptionService _encryptionService;
        private readonly IDepartmentService _departmentService;

        public UserManagerController(IUserService userService,
            IUserRoleService userRoleService,
            IPermissionService permissionService,
            IUserRegistrationService userRegistrationService,
            IEncryptionService encryptionService,
            IDepartmentService departmentService
            )
        {
            _userService = userService;
            _userRoleService = userRoleService;
            _permissionService = permissionService;
            _userRegistrationService = userRegistrationService;
            _encryptionService = encryptionService;
            _departmentService = departmentService;
        }

        //get all userrole for usermanager
        [HttpPost]
        public JsonResult GetAllUserRole(DataSourceRequest command)
        {
            var userRoles = _userService.GetAllUserRolesAsync();
            var data = userRoles.Result.Where(u => u.SystemName != SystemUserRoleNames.MeetingLeaders && u.SystemName != SystemUserRoleNames.Guest)
                                        .Select(x => new UserRoleViewModel
            {
                Id = x.Id,
                Name = x.Name
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Index()
        {
            if (await _permissionService.Authorize(PermissionProvider.ManageUser, User.Identity.Name))
                return View("List");

            return View("List");
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, SearchUserModel model)
        {
            var users = _userService.GetAllUsersAsync(searchKeyword: model.SearchKeyword, pageIndex:command.Page-1, pageSize:command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = users.Select(u => new User_Model
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    UserName = u.Username,
                    Active = u.Active,
                    //Role = GetUserRoleForViewModel(u.UserRoles),
                    Roles = u.UserRoles.Select( ur => new UserRoleViewModel() { Id = ur.Id, Name = ur.Name }).ToList(),
                    ResetPassword = "",
                    Departments = u.Departments.Select(ud=> new DepartmentViewModel() { Id = ud.Id, Name = ud.Name }).ToList()
                }),
                Total = users.TotalCount
            };

            // Return the result as JSON
            return Json(gridModel);
        }
        
        [HttpPost]
        public async Task<ActionResult> Create(User_Model model)
        {
            if (ModelState.IsValid)
            {
                var user = new User()
                {
                    UserGuid = Guid.NewGuid(),
                    Username = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Active = model.Active,
                    //DepartmentId = model.Roles.Any(r => r.Name ==  "Administrators") ? 0 : model.Department.Id
                };

                foreach (var department in model.Departments)
                {
                    var d = _departmentService.GetByIdAsync(department.Id).Result;
                    user.Departments.Add(d);
                }

                foreach (var role in model.Roles)
                {
                    var userRole = _userService.GetUserRoleByIdAsync(role.Id);
                    user.UserRoles.Add(userRole);
                }          

                var result = await _userRegistrationService.RegisterUser(new UserRegistrationRequest(user, model.Email, model.UserName, model.Password));
                if(!result.Success)
                {
                    return Content(string.Join(", ", result.Errors));
                }
                else
                    return Json(new{ status = "success" });
            }

            // If we got this far, something failed, redisplay form
            return Json(new { Data = new[] { model } });
            //return View(model);
        }
        
        [HttpPost]
        public async Task<ActionResult> Update(User_Model model)
        {
            var listError = new List<string>();
            //if (ModelState.IsValid)
            {

                var existUser = await _userService.GetUserByUsernameAsync(model.UserName);
                if (existUser == null)
                    listError.Add("Not exist user");
                else
                {
                    var existUserByEmail = await _userService.GetUserByEmailAsync(model.Email);
                    if (existUserByEmail != null)
                    {
                        if (existUserByEmail.Username == model.UserName)
                        {
                            existUserByEmail.FirstName = model.FirstName;
                            existUserByEmail.LastName = model.LastName;
                            existUserByEmail.Active = model.Active;
                            existUserByEmail.PhoneNumber = model.PhoneNumber;

                            //User departments
                            if (model.Departments != null && model.Departments.Count() > 0)
                            {
                                foreach (var department in existUserByEmail.Departments.ToList())
                                {
                                    existUserByEmail.Departments.Remove(department);
                                }

                                var addedDepartments = await _departmentService.GetDepartmentByIdsAsync(model.Departments.Select(d => d.Id).ToList());//  _userRoleService.GetUserRolesByIdsAsync(model.Roles.Where(mr => existUserByEmail.UserRoles.All(r => r.Id != mr.Id)).Select(mr => mr.Id).ToList());

                                foreach (var department in addedDepartments.ToList())
                                {
                                    existUserByEmail.Departments.Add(department);
                                }
                            }
                            if (!string.IsNullOrEmpty(model.ResetPassword))
                            {
                                string saltKey = _encryptionService.CreateSaltKey(5);
                                existUserByEmail.PasswordSalt = saltKey;
                                existUserByEmail.Password = _encryptionService.CreatePasswordHash(model.ResetPassword, saltKey);
                            }

                            //User roles
                            foreach (var role in existUserByEmail.UserRoles.Where(r=>r.SystemName!=SystemUserRoleNames.MeetingLeaders && model.Roles.All(mr=>mr.Id!=r.Id)).ToList())
                            {
                                existUserByEmail.UserRoles.Remove(role);
                            }

                            var addedRoles = await _userRoleService.GetUserRolesByIdsAsync(model.Roles.Where(mr => existUserByEmail.UserRoles.All(r => r.Id != mr.Id)).Select(mr => mr.Id).ToList());

                            foreach (var role in addedRoles)
                            {
                                existUserByEmail.UserRoles.Add(role);
                            }
                            await _userService.UpdateUserAsync(existUserByEmail);


                            return Json(new
                            {
                                status = "success",
                            });
                        }
                        else
                        {
                            listError.Add("This email is belong to another user");
                        }
                    }
                    else
                    {
                        existUser.Email = model.Email;
                        existUser.FirstName = model.FirstName;
                        existUser.LastName = model.LastName;
                        existUser.Active = model.Active;
                        existUser.PhoneNumber = model.PhoneNumber;

                        foreach (var department in existUser.Departments.ToList())
                        {
                            existUser.Departments.Remove(department);
                        }

                        var addedDepartments = await _departmentService.GetDepartmentByIdsAsync(model.Departments.Select(d => d.Id).ToList());//  _userRoleService.GetUserRolesByIdsAsync(model.Roles.Where(mr => existUserByEmail.UserRoles.All(r => r.Id != mr.Id)).Select(mr => mr.Id).ToList());

                        foreach (var department in addedDepartments.ToList())
                        {
                            existUser.Departments.Add(department);
                        }

                        foreach (var role in existUser.UserRoles.Where(r=>r.SystemName!=SystemUserRoleNames.MeetingLeaders).ToList())
                        {
                            existUser.UserRoles.Remove(role);
                        }

                        foreach (var role in model.Roles)
                        {
                            var userRole = _userService.GetUserRoleByIdAsync(role.Id);
                            existUser.UserRoles.Add(userRole);
                        }

                        await _userService.UpdateUserAsync(existUser);
                        return Json(new
                        {
                            status = "success",
                        });
                    }
                }
            }
            return Json(new
            {
                status = "failed",
                listError = listError
            });
        }
        
        private UserRoleViewModel GetUserRoleForViewModel(ICollection<UserRole> userRoles)
        {
            var userRole = userRoles.FirstOrDefault(r => r.SystemName != SystemUserRoleNames.MeetingLeaders);
            if (userRole == null)
                return new UserRoleViewModel();
            return new UserRoleViewModel
            {
                Id = userRole.Id,
                Name = userRole.Name
            };
        }

        //get all user
        [HttpPost]
        public JsonResult GetAllUser(DataSourceRequest command)
        {
            var users = _userService.GetAllUsersAsync();
            var data = users.Where(u => u.Active == true).Select(x => new Web.Models.Meeting.UserForMeetingViewModel
                {
                    Id = x.Id,
                    Name = x.Username
                });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UsernameList(int departmentId)
        {
            var users = _userService.GetAllUsersAsync();
            var data = users.Where(u => u.Active == true && u.Departments.Any(d=>d.Id == departmentId)).Select(x => x.Username);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAllDepartment(DataSourceRequest command)
        {
            var departments = _departmentService.SearchDepartment(null,true, includeSupplyChain:true);

            var data = departments.Select(x => new DepartmentModel
            {
                Id = x.Id,
                Name = x.Name
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ExportToExcel()
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageUser))
                return AccessDeniedView();

            var users = _userService.GetAllUsersAsync();


            #region export excell

            var path = "";
            try
            {
                var currentTime = DateTime.Now.ToLongTimeString().Replace("-", "").Replace(" ", "").Replace(":", "");

                var r = new Random();
                int u = r.Next(10000);

                var filename = "UserList_" + currentTime + ".xlsx";
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    _userService.ExportToXlsx(stream, users, path);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", filename);
            }
            catch (Exception e)
            {
                var message = e.Message;
                return RedirectToAction("List");
            }

            #endregion
        }

        #region Old code
        //[HttpPost]
        //public async Task<ActionResult> Delete(string username)
        //{
        //    if (String.IsNullOrEmpty(username))
        //    {
        //        ModelState.AddModelError("username", "username is empty.");
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userService.GetUserByUsernameAsync(username);
        //        if (user != null)
        //        {
        //            await _userService.DeleteUserAsync(user);
        //        }
        //    }
        //    return new NullJsonResult();
        //}

        public async Task<ViewResult> Edit(int userId)
        {
            if (userId == 0)
            {
                ModelState.AddModelError("username", "userId is empty.");
            }
            var user = await _userService.GetUserByIdAsync(userId);

            var userInfoModel = new UserInfoViewModel()
            {
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };
            var passModel = new ManageUserViewModel();

            var defaultRole = user.UserRoles.FirstOrDefault();

            var allRole = await _userService.GetAllUserRolesAsync();

            var listRole = allRole.Select(ur => new SelectListItem()
            {
                Selected = ur.Name == defaultRole.Name,
                Text = ur.Name,
                Value = ur.Id.ToString()
            }).ToList();

            var roleModel = new RoleViewModel()
            {
                Role = defaultRole.Name,
                ListRole = listRole,
                Username = user.Username
            };

            var resetPass = new SetPasswordModel()
                            {
                                Username = user.Username
                            };

            var model = new ManageModel()
            {
                InfoModel = userInfoModel,
                PassModel = passModel,
                RoleModel = roleModel,
                ResetPasswordModel = resetPass,
            };
            return View(model);

        }
        

        [HttpPost]
        public async Task<ActionResult> Edit(UserInfoViewModel model)
        {
            var listError = new List<string>();
            if (ModelState.IsValid)
            {

                var existUser = await _userService.GetUserByUsernameAsync(model.Username);
                if (existUser == null)
                    listError.Add("Not exist user");
                else
                {
                    var existUserByEmail = await _userService.GetUserByEmailAsync(model.Email);
                    if (existUserByEmail != null)
                    {
                        if (existUserByEmail.Username == model.Username)
                        {
                            existUserByEmail.FirstName = model.FirstName;
                            existUserByEmail.LastName = model.LastName;
                            existUserByEmail.PhoneNumber = model.PhoneNumber;
                            await _userService.UpdateUserAsync(existUserByEmail);
                            return Json(new
                            {
                                status = "success",
                            });
                        }
                        else
                        {
                            listError.Add("This email is belong to another user");
                        }
                    }
                    else
                    {
                        existUser.Email = model.Email;
                        existUser.FirstName = model.FirstName;
                        existUser.LastName = model.LastName;
                        existUser.PhoneNumber = model.PhoneNumber;
                        await _userService.UpdateUserAsync(existUser);
                        return Json(new
                        {
                            status = "success",
                        });
                    }

                }


            }
            return Json(new
            {
                status = "failed",
                listError = listError
            });
        }
       

        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> SetPasswordAjax(SetPasswordModel model)
        {
            var listError = new List<string>();
            if (ModelState.IsValid)
            {
                string username = model.Username;
                string newpassword = model.ConfirmPassword;

                var user = await _userService.GetUserByUsernameAsync(username);
                if (user == null)
                    listError.Add("Not exist user");
                else
                {
                    var result = await _userRegistrationService.ChangePassword(new ChangePasswordRequest(model.Username, false, model.ConfirmPassword));

                    if(result.Success)
                        return Json(new
                                {
                                    status = "success"
                                });
                    listError = result.Errors.ToList();
                }
            }
            return Json(new
            {
                status = "failed",
                listError = listError
            });
        }


        [HttpPost]
        public async Task<ActionResult> SetRoleAjax(RoleViewModel model)
        {
            var listError = new List<string>();
            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByUsernameAsync(model.Username);
                if (user == null)
                    listError.Add("Not exist user");
                else
                {
                    //var userRole = await _userService.GetRolesAsync(user);
                    ////clear role
                    //foreach (var role in user.UserRoles)
                    //{
                    //    await _userService.RemoveFromRoleAsync(user, role);
                    //}
                    ////add role
                    //await _userService.AddToRoleAsync(user, model.Role);
                    //return Json(new
                    //{
                    //    status = "success"
                    //});
                }
            }
            return Json(new
            {
                status = "failed",
                listError = listError
            });
        }

        #endregion
    }
}