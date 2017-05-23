using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Entities.Domain;

namespace Web.Models.UserAllowInSupplyChain
{

    public class UserAllowInSupplyChainModel
    {
        public int Id { get; set; }
        public List<string> ListUsername { get; set; }
        public int DMSCode { get; set; }
        public string DMSName { get; set; }
        public UserAllowInSupplyChainModel()
        {
            ListUsername = new List<string>();
        }
    }


}