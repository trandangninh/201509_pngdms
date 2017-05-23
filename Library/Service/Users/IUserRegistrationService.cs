using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain.Users;

namespace Service.Users
{
    public interface IUserRegistrationService
    {
        Task<UserLoginResults> ValidateUser(string username, string password);
        Task<UserRegistrationResult> RegisterUser(UserRegistrationRequest request);
        Task<PasswordChangeResult> ChangePassword(ChangePasswordRequest request);
    }
}
