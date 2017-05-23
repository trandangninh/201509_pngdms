using System;
using System.Threading.Tasks;
using Entities.Domain.Users;
using Service.Security;

namespace Service.Users
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly IUserService _userService;
        private readonly IEncryptionService _encryptionService;
        public UserRegistrationService(IUserService userService,
            IEncryptionService encryptionService)
        {
            this._userService = userService;
            this._encryptionService = encryptionService;
        }

        public async Task<UserLoginResults> ValidateUser(string username, string password)
        {
            var user = await _userService.GetUserByUsernameAsync(username);

            if (user == null)
                return UserLoginResults.UserNotExist;
            if (user.Deleted)
                return UserLoginResults.Deleted;
            if (!user.Active)
                return UserLoginResults.NotActive;

            string pwd = _encryptionService.CreatePasswordHash(password, user.PasswordSalt);

            bool isValid = pwd == user.Password;

            if (!isValid)
                return UserLoginResults.WrongPassword;

            //save last login date
            user.LastLoginDate = DateTime.UtcNow;
            await _userService.UpdateUserAsync(user);
            return UserLoginResults.Successful;
        }

        public async Task<UserRegistrationResult> RegisterUser(UserRegistrationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.User == null)
                throw new ArgumentException("Can't load current user");

            var result = new UserRegistrationResult();
            
            //if (String.IsNullOrEmpty(request.Email))
            //{
            //    result.AddError(UserRegistrationMessage.UsernameNotFound);
            //    return result;
            //}
            //if (!CommonHelper.IsValidEmail(request.Email))
            //{
            //    result.AddError(UserRegistrationMessage.UsernameNotFound);
            //    return result;
            //}
            if (String.IsNullOrWhiteSpace(request.Password))
            {
                result.AddError(UserRegistrationMessage.PasswordIsNotProvided);
                return result;
            }
      
            //validate unique user
            if (await _userService.GetUserByUsernameAsync(request.Username) != null)
            {
                result.AddError(UserRegistrationMessage.UsernameAlreadyExists);
                return result;
            }
            
            //at this point request is valid
            request.User.Username = request.Username;
            request.User.Email = request.Email;

            string saltKey = _encryptionService.CreateSaltKey(5);
            request.User.PasswordSalt = saltKey;
            request.User.Password = _encryptionService.CreatePasswordHash(request.Password, saltKey);
                    
            request.User.Active = request.IsApproved;

            //Add reward points for user registration (if enabled)
            request.User.UserGuid = Guid.NewGuid();
            await _userService.InsertUserAsync(request.User);
            return result;
        }

        public async Task<PasswordChangeResult> ChangePassword(ChangePasswordRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new PasswordChangeResult();
            if (String.IsNullOrWhiteSpace(request.Username))
            {
                result.AddError(UserRegistrationMessage.UsernameNotFound);
                return result;
            }
            if (String.IsNullOrWhiteSpace(request.NewPassword))
            {
                result.AddError(UserRegistrationMessage.PasswordIsNotProvided);
                return result;
            }

            var user = await _userService.GetUserByUsernameAsync(request.Username);
            if (user == null)
            {
                result.AddError(UserRegistrationMessage.UsernameNotFound);
                return result;
            }

            var requestIsValid = false;
            if (request.ValidateRequest)
            {
                //password
                var oldPwd = _encryptionService.CreatePasswordHash(request.OldPassword, user.PasswordSalt);
                
                bool oldPasswordIsValid = oldPwd == user.Password;
                if (!oldPasswordIsValid)
                    result.AddError(UserRegistrationMessage.OldPasswordDoesntMatch);

                if (oldPasswordIsValid)
                    requestIsValid = true;
            }
            else
                requestIsValid = true;

            //at this point request is valid
            if (requestIsValid)
            {
                
                string saltKey = _encryptionService.CreateSaltKey(5);
                user.PasswordSalt = saltKey;
                user.Password = _encryptionService.CreatePasswordHash(request.NewPassword, saltKey);
                       
                await _userService.UpdateUserAsync(user);
            }

            return result;
        }
    }
}
