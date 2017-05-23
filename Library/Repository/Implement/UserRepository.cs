using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Entities.IdentityEntities;
using RepositoryPattern.Repositories;

namespace Repository.Implement
{
   
    public static class UserRepository
    {
        
        //public static Task<User> GetUserByUsernameAsync(this IRepositoryAsync<User> repository, string username)
        //{

        //    if (string.IsNullOrEmpty(username))
        //    {
        //        throw new ArgumentException("Null or empty argument: userName");
        //    }
        //    return repository
        //        .Table
        //        .FirstOrDefaultAsync(x => x.Username == username);

        //}


        //public static Task<User> GetUserByIdAsync(this IRepositoryAsync<User> repository, string id)
        //{

        //    if (string.IsNullOrEmpty(id))
        //    {
        //        throw new ArgumentException("Null or empty argument: userName");
        //    }
        //    return null; // Khang comment
        //    //return repository
        //      //  .Table
        //        //.FirstOrDefaultAsync(x => x.Id == id);

        //}

        //public static Task<User> GetUserByEmailAsync(this IRepositoryAsync<User> repository, string email)
        //{

        //    if (string.IsNullOrEmpty(email))
        //    {
        //        throw new ArgumentException("Null or empty argument: email");
        //    }
        //    return repository
        //        .Table
        //        .FirstOrDefaultAsync(x => x.Email == email);

        //}

        //public static Task<List<User>> GetAllUser(this IRepositoryAsync<User> repository)
        //{
        //    return repository
        //        .Table.OrderBy(p=>p.Username).ToListAsync();

        //}






    }
}
