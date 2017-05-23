using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.ShutdownRequest
{
   
    public class ShutdownRequestModel
    {
        public int Id { get; set; }

        public string ShutdownId { get; set; }

        public string Content { get; set; }

        public string CreatedDate { get; set; }

        public string UpdatedDate { get; set; }

        public string UserCreated { get; set; }

        public int UserCreatedId { get; set; }

        public string MakingScope { get; set; }

        public string PackingScope { get; set; }

        public string BasePlanDate { get; set; }
        
        public string Comment { get; set; }

        public string Remark { get; set; }

        public bool CanEdit { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
    }

    public class SearchShutDownModel
    {
        public string Datetime { get; set; }
        public string SearchKeyword { get; set; }
        public int StatusId { get; set; }
    }

    public class SearchShutDownByAdminModel
    {
        public string Datetime { get; set; }
        public string SearchKeyword { get; set; }
        public int UserId { get; set; }
        public int StatusId { get; set; }
    }
}