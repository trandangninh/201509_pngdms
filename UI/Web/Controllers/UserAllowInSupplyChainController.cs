using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Entities.Domain;
using Service.Interface;
using Service.Users;
using Web.Extend;
using Utils;
using Web.Models.UserAllowInSupplyChain;
using Nois.Web.Framework.Kendoui;

namespace Web.Controllers
{
    [Authorize]
    public class UserAllowInSupplyChainController : Controller
    {
        private readonly IUserService _userService;
        public UserAllowInSupplyChainController(IUserService userService)
        {
            _userService = userService;
        }

        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> List(DataSourceRequest command)
        {
            var allUserAllowInSupplyModel = new List<UserAllowInSupplyChainModel>();

            allUserAllowInSupplyModel.AddRange(DmsType.DDS.EnumToList()
                .Select(e => new UserAllowInSupplyChainModel()
                             {
                                 DMSCode = e.Key,
                                 DMSName = e.Value
                             }));

            var total = allUserAllowInSupplyModel.Count;
            if (command.Page > 0)
                allUserAllowInSupplyModel = allUserAllowInSupplyModel.Skip(command.PageSize * (command.Page - 1)).Take(command.PageSize).ToList();

            foreach (var item in allUserAllowInSupplyModel)
            {
                DmsType type;
                Enum.TryParse(item.DMSName, out type);
                var users = await _userService.GetUsersInDms(type);
                item.ListUsername = users.Select(u => u.Username).ToList();
            }
            var gridModel = new DataSourceResult
            {
                Data = allUserAllowInSupplyModel,
                Total = total
            };

            return Json(gridModel);

        }
        [HttpPost]
        public async Task<ActionResult> Update(UserAllowInSupplyChainModel model)
        {
            DmsType type;
            Enum.TryParse(model.DMSName, out type);
            //delete user of type
            await _userService.DeleteAllUsersInDms(type);

            //add again user for type
            foreach (var username in model.ListUsername)
            {
                var user = await _userService.GetUserByUsernameAsync(username);
                if (user != null)
                {
                    await _userService.AddUserToDms(type, user);
                }
            }
            return new NullJsonResult();
        }

    }
}