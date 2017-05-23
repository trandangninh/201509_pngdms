using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Entities.Domain.Users;
using Service.Authentication;
using Service.Users;
using Utils;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IWorkContext _workContext;

        public AccountController(IUserService userService, 
            IUserRegistrationService userRegistrationService, 
            IAuthenticationService authenticationService, 
            IWorkContext workContext)
        {
            _userService = userService;
            _userRegistrationService = userRegistrationService;
            _authenticationService = authenticationService;
            _workContext = workContext;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var loginResult = await _userRegistrationService.ValidateUser(model.Username, model.Password);

                switch (loginResult)
                {
                    case UserLoginResults.Successful:
                    {
                        var user = await _userService.GetUserByUsernameAsync(model.Username);
                        _authenticationService.SignIn(user, model.RememberMe);
                        return RedirectToLocal(returnUrl);
                    }
                    case UserLoginResults.UserNotExist:
                    ModelState.AddModelError("", UserRegistrationMessage.UserNotExist);
                    break;
                    case UserLoginResults.Deleted:
                    ModelState.AddModelError("", UserRegistrationMessage.Deleted);
                    break;
                    case UserLoginResults.NotActive:
                    ModelState.AddModelError("", UserRegistrationMessage.NotActive);
                    break;
                    case UserLoginResults.WrongPassword:
                    default:
                    ModelState.AddModelError("", UserRegistrationMessage.WrongCredentials);
                    break;
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if(_workContext.CurrentUser != null)
                _authenticationService.SignOut();

            if (ModelState.IsValid)
            {
                var user = new User{ Username = model.Username };
                var result = await _userRegistrationService.RegisterUser(new UserRegistrationRequest(user, "", model.Username, model.Password));
                if (result.Success)
                {
                    _authenticationService.SignIn(user, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach(var error in result.Errors)
                        ModelState.AddModelError("",error);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            //ManageMessageId? message = null;
            //var result = _authenticationService.SignOut();
            //if (result.Succeeded)
            //{
            //    message = ManageMessageId.RemoveLoginSuccess;
            //}
            //else
            //{
            //    message = ManageMessageId.Error;
            //}
            //return RedirectToAction("Manage", new { Message = message });
            return null;
        }

        //
        // GET: /Account/Manage
        public async Task<ActionResult> Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            //ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");

            var user = await _userService.GetUserByUsernameAsync(User.Identity.Name);
            var userInfoModel = new UserInfoViewModel()
            {
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };
            var passModel = new ManageUserViewModel();
            var model = new ManageModel()
            {
                InfoModel = userInfoModel,
                PassModel = passModel
            };
            return View(model);
        }

        [HttpPost]

        public async Task<ActionResult> ChangeInfo(UserInfoViewModel model)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ManageUserViewModel model)
        {
            var listError = new List<string>();
            if (ModelState.IsValid)
            {
                var result = await _userRegistrationService.ChangePassword(new ChangePasswordRequest(_workContext.CurrentUser.Username, true, model.NewPassword, model.OldPassword));

                if (result.Success)
                {
                    return Json(new {status = "success",});
                }
                listError.AddRange(result.Errors);
            }
            return Json(new
            {
                status = "failed",
                listError = listError
            });
        }


        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            //bool hasPassword = HasPassword();
            //ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            //if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    var result = await _userRegistrationService.ChangePassword(new ChangePasswordRequest("", true, model.NewPassword, model.OldPassword));
                    if (result.Success)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }

                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error);
                }
            }
            //else
            //{
            //    // User does not have a password so remove any validation errors caused by a missing OldPassword field
            //    ModelState state = ModelState["OldPassword"];
            //    if (state != null)
            //    {
            //        state.Errors.Clear();
            //    }

            //    if (ModelState.IsValid)
            //    {
            //        var result = await _userRegistrationService.ChangePassword(new ChangePasswordRequest("", true, model.NewPassword, model.OldPassword));
            //        if (result.Success)
            //        {
            //            return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
            //        }

            //        foreach (var error in result.Errors)
            //            ModelState.AddModelError("", error);
            //    }
            //}

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    // Request a redirect to the external login provider
        //    return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        //}

        //
        // GET: /Account/ExternalLoginCallback
        ////[AllowAnonymous]
        ////public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        ////{
        ////    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        ////    if (loginInfo == null)
        ////    {
        ////        return RedirectToAction("Login");
        ////    }

        ////    // Sign in the user with this external login provider if the user already has a login
        ////    var user = null;//await UserManager.FindAsync(loginInfo.Login);
        ////    if (user != null)
        ////    {
        ////        await SignInAsync(user, isPersistent: false);
        ////        return RedirectToLocal(returnUrl);
        ////    }
        ////    else
        ////    {
        ////        // If the user does not have an account, then prompt the user to create an account
        ////        ViewBag.ReturnUrl = returnUrl;
        ////        ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        ////        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Username = loginInfo.DefaultUsername });
        ////    }
        ////}

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), _workContext.CurrentUser.Id.ToString());
        }

        //
        // GET: /Account/LinkLoginCallback
        //public async Task<ActionResult> LinkLoginCallback()
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        //    }
        //    var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("Manage");
        //    }
        //    return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        //}

        //
        // POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Manage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("ExternalLoginFailure");
        //        }
        //        var user = new User() { Username = model.Username };
        //        var result = await UserManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await UserManager.AddLoginAsync(user.Id, info.Login);
        //            if (result.Succeeded)
        //            {
        //                await SignInAsync(user, isPersistent: false);
        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            _authenticationService.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        //[ChildActionOnly]
        //public ActionResult RemoveAccountList()
        //{

           
        //    var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
        //    ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
        //    return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && UserManager != null)
        //    {
        //        UserManager.Dispose();
        //        UserManager = null;
        //    }
        //    base.Dispose(disposing);
        //}

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";


        //private async Task SignInAsync(User user, bool isPersistent)
        //{
        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        //    var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
        //    AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        //}

        
        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                //var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    //properties.Dictionary[XsrfKey] = UserId;
                }
                //context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}