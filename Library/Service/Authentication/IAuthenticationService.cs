using Entities.Domain.Users;

namespace Service.Authentication
{
    /// <summary>
    /// Authentication service interface
    /// </summary>
    public partial interface IAuthenticationService 
    {
        void SignIn(User user, bool createPersistentCookie);
        void SignOut();
        User GetAuthenticatedCustomer();
    }
}