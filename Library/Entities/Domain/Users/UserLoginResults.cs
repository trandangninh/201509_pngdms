using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Domain.Users
{
    public enum UserLoginResults
    {
        Successful = 1,
        UserNotExist = 2,
        WrongPassword = 3,
        NotActive = 4,
        Deleted = 5
    }
}
