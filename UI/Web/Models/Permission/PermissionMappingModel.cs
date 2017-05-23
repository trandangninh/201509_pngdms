using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Models.Permission
{
    public class PermissionMappingModel
    {

        public PermissionMappingModel()
        {
            AvailablePermissions = new List<PermissionRecordModel>();
            AvailableRoles = new List<RoleModel>();
            Allowed = new Dictionary<string, IDictionary<int, bool>>();
        }
        public IList<PermissionRecordModel> AvailablePermissions { get; set; }
        public IList<RoleModel> AvailableRoles { get; set; }

        //[permission system name] / [customer role id] / [allowed]
        public IDictionary<string, IDictionary<int, bool>> Allowed { get; set; }
    }


    public class PermissionRecordModel 
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string SystemName { get; set; }
    }

    public class RoleModel : BaseNoisEntityModel
    {
        [AllowHtml]
        public string Name { get; set; }
        public string SystemName { get; set; }
    }
}