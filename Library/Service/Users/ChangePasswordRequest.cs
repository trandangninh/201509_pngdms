
namespace Service.Users
{
    public class ChangePasswordRequest
    {
        public string Username { get; set; }
        public bool ValidateRequest { get; set; }
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }

        public ChangePasswordRequest(string username, bool validateRequest, string newPassword, string oldPassword = "")
        {
            this.Username = username;
            this.ValidateRequest = validateRequest;
            this.NewPassword = newPassword;
            this.OldPassword = oldPassword;
        }
    }
}
