using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Users
{
    public static class UserRegistrationMessage
    {
        public static string PasswordIsNotProvided = "Password is not provided";
        public static string UsernameAlreadyExists = "Username already exists";
        public static string UsernameNotFound = "User is not found";
        public static string OldPasswordDoesntMatch = "Old password doesn't match";
        public static string UserNotExist = "User not exist";
        public static string Deleted = "User was deleted";
        public static string NotActive = "User was not active";
        public static string WrongCredentials = "Invalid username or password";
    }
}
