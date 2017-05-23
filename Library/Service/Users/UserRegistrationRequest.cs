using Entities.Domain.Users;

namespace Service.Users
{
    public class UserRegistrationRequest
    {
        public User User { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsApproved { get; set; }

        public UserRegistrationRequest(User user, string email, string username,
            string password,
            bool isApproved = true)
        {
            this.User = user;
            this.Email = email;
            this.Username = username;
            this.Password = password;
            this.IsApproved = isApproved;
        }
    }
}
