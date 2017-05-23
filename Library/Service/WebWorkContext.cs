using System;
using System.Web;
using Service.Authentication;
using Service.Users;
using Utils;
using Entities.Domain.Users;

namespace Service
{
    public class WebWorkContext : IWorkContext
    {
        private const string UserCookieName = "PG.user";

        private readonly HttpContextBase _httpContext;
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        private User _cacheUser;
        public WebWorkContext(HttpContextBase httpContext, 
            IUserService userService, 
            IAuthenticationService authenticationService)
        {
            this._httpContext = httpContext;
            this._userService = userService;
            this._authenticationService = authenticationService;
        }

        protected virtual HttpCookie GetUserCookie()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;
            return _httpContext.Request.Cookies[UserCookieName];
        }

        protected virtual void SetUserCookie(Guid userGuid)
        {
            if (_httpContext != null && _httpContext.Response != null)
            {
                var cookie = new HttpCookie(UserCookieName);
                cookie.HttpOnly = true;
                cookie.Value = userGuid.ToString();
                if (userGuid == Guid.Empty)
                {
                    cookie.Expires = DateTime.Now.AddMonths(-1);
                }
                else
                {
                    int cookieExpires = 24*365;
                    cookie.Expires = DateTime.Now.AddHours(cookieExpires);
                }
                _httpContext.Response.Cookies.Remove(UserCookieName);
                _httpContext.Response.Cookies.Add(cookie);
            }
        }

        public User CurrentUser
        {
            get
            {
                if (_cacheUser != null)
                    return _cacheUser;

                User user = null;
                //if (_httpContext == null) // || _httpContext is FakeHttpContext)
                    //Todo check again
                //    user = new User();
                //if (user == null || user.IsDeleted || !user.IsActive)
                //{
                    user = _authenticationService.GetAuthenticatedCustomer();
                //}

                if (user == null)
                {
                    SetUserCookie(Guid.NewGuid());
                }

                if (user!=null && !user.Deleted && user.Active)
                {
                    //Todo check again
                    SetUserCookie(user.UserGuid);
                    _cacheUser = user;
                }
                return _cacheUser;
            }
            set
            {
                //Todo check again
                SetUserCookie(Guid.NewGuid());
                _cacheUser = value;
            }
        }
    }
}
