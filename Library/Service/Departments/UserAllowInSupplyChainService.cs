using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;


using RepositoryPattern.Repositories;
using Service.Interface;
using Entities.Domain.Users;

namespace Service.Implement
{
    public class UserAllowInSupplyChainService : IUserAllowInSupplyChainService
    {
        private readonly IRepositoryAsync<UserAllowInSupplyChain> _userAllowRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;

        public UserAllowInSupplyChainService(IRepositoryAsync<UserAllowInSupplyChain> repository, 
            IRepositoryAsync<UserAllowInSupplyChain> userAllowRepositoryAsync, 
            IRepositoryAsync<User> userRepositoryAsync)
        {
            _userAllowRepositoryAsync = userAllowRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
        }


        public Task<List<UserAllowInSupplyChain>> GetUserByDMSType(DmsType type)
        {
            throw new NotImplementedException();
        }

        public bool CheckUserByDMSTypeNotAsync(DmsType type, string userName)
        {

            var user =
                _userRepositoryAsync.Table.FirstOrDefault(p => p.Username == userName);
            if (user == null)
            {
                throw new Exception("Not exist user");
            }
            var countUserMeeting = _userAllowRepositoryAsync.Table
               .Count(x => x.DmsTypeId == (int)type && x.UserId == user.Id);

            return countUserMeeting > 0;

            //if (listUserMeeting.Count > 0)
            //    return true;
            //return false;
        }

        public async Task<bool> CheckUserByDMSType(DmsType type, string  userName)
        {
            var user = _userRepositoryAsync.Table.FirstOrDefaultAsync(u=>u.Email == userName);
            var listUserMeeting = await _userAllowRepositoryAsync.Table
               .Where(x => x.DmsType == type && x.UserId == user.Result.Id).ToListAsync();

            if(listUserMeeting.Count>0)
                return true;
            return false;
        }

        public async Task<List<string>> GetUsernameByDMSType(DmsType type)
        {
            var listUserMeeting = await _userAllowRepositoryAsync.Table.Where(x => x.DmsType == type).ToListAsync();
          
            var result = new List<string>();

            foreach (var userMeeting in listUserMeeting)
            {
                var user = _userRepositoryAsync.Table.FirstOrDefault(p => p.Id == userMeeting.UserId && p.Active);
                if (user != null)
                {
                    result.Add(user.Username);
                }
            }
            return result;
        }

        public Task DeleteAllUserOfType(DmsType type)
        {
            var allRecords = _userAllowRepositoryAsync.Table.Where(p => p.DmsType == type);

            return _userAllowRepositoryAsync.DeleteAsync(allRecords);
        }

        public Task AddUserToDMS(DmsType type, User user)
        {
            return _userAllowRepositoryAsync.InsertAsync(new UserAllowInSupplyChain()
            {
                DmsType = type,
                // Khang comment UserId = user.Id
            });
        }

        public Task CreateAsync(UserAllowInSupplyChain userAllowInSupplyChain)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(UserAllowInSupplyChain userAllowInSupplyChain)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(UserAllowInSupplyChain userAllowInSupplyChain)
        {
            throw new NotImplementedException();
        }
    }
}
