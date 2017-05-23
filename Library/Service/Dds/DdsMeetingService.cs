using System.Diagnostics;
using System.Xml;
using Entities.Domain;
using Entities.Domain.Dds;
using Entities.Domain.Departments;
using RepositoryPattern.Repositories;
using Service.Security;
using Service.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Utils.Caching;
using System.Data.Entity;

namespace Service.Dds
{
    public partial class DdsMeetingService : BaseService<DdsMeeting>, IDdsMeetingService
    {
        #region constant for cache
        protected override string PatternKey
        {
            get { return "PG.DdsMeeting."; }
        }

        //private const string DDSCONFIG_BY_DEPARTMENT_KEY = "PG.DdsConfig.ByDepartment-{0}";
        #endregion

        private readonly IRepositoryAsync<DdsMeeting> _ddsMeetingRepositoryAsync;
        private readonly ICacheManager _cacheManager;
        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;

        public DdsMeetingService(IRepositoryAsync<DdsMeeting> ddsMeetingRepositoryAsync,
            ICacheManager cacheManager,
            IUserService userService, IPermissionService permissionService)
            : base(ddsMeetingRepositoryAsync, cacheManager)
        {
            this._ddsMeetingRepositoryAsync = ddsMeetingRepositoryAsync;
            this._cacheManager = cacheManager;
            this._userService = userService;
            this._permissionService = permissionService;
        }

        //public List<DdsConfig> GetDdsConfigByDepartmentId(int departmentId)
        //{
        //    if (departmentId <= 0)
        //        return null;

        //    var key = string.Format(DDSCONFIG_BY_DEPARTMENT_KEY, departmentId);
        //    return _cacheManager.Get(key, () => _ddsConfigRepositoryAsync.Table.Where(m => m.Measure.Dms.DepartmentId == departmentId).ToList());
        //}

        public Task<DdsMeeting> GetDdsMeetingByDate(DateTime date, int departmentId)
        {
            date = date.Date;
            return _ddsMeetingRepositoryAsync.Table.FirstOrDefaultAsync(d => d.CreatedDateTime == date && d.DepartmentId == departmentId);
        }

        public Task<IPagedList<DdsMeeting>> GetDdsMeetingByDate(DateTime fromDate, DateTime toDate, int departmentId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _ddsMeetingRepositoryAsync.Table.Where(d => d.CreatedDateTime >= fromDate && d.CreatedDateTime <= toDate);

            if (departmentId != 0)
            {
                query = query.Where(d => d.DepartmentId == departmentId);
                return Task.FromResult(new PagedList<DdsMeeting>(query.OrderByDescending(d => d.CreatedDateTime), pageIndex, pageSize) as IPagedList<DdsMeeting>);
            }
            else
            {
                var result = new List<DdsMeeting>();
                foreach (var d in query.ToList())
                {
                    if (_permissionService.Authorize(PermissionProvider.ViewAttendance))
                        result.Add(d);
                }
                return Task.FromResult(new PagedList<DdsMeeting>(result.OrderByDescending(d => d.CreatedDateTime).AsQueryable(), pageIndex, pageSize) as IPagedList<DdsMeeting>);
            }
        }

        public Dictionary<int, string> LineRemarkParser(string lineRemarkXml)
        {
            var result = new Dictionary<int, string>();
            if (String.IsNullOrEmpty(lineRemarkXml))
                return result;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(lineRemarkXml);

                var nodeList1 = xmlDoc.SelectNodes(@"//LineRemark/Line");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes != null && node1.Attributes["ID"] != null)
                    {
                        string str1 = node1.Attributes["ID"].InnerText.Trim();
                        int id;
                        if (int.TryParse(str1, out id))
                        {
                            result.Add(id, node1.InnerText.Trim());
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }
            return result;
        }

        public string AddLineRemarkToXmlData(string lineRemarkXml, int lineId, string remark)
        {
            var xmlDoc = new XmlDocument();
            if (String.IsNullOrEmpty(lineRemarkXml))
            {
                var element1 = xmlDoc.CreateElement("LineRemark");
                xmlDoc.AppendChild(element1);
            }
            else
            {
                xmlDoc.LoadXml(lineRemarkXml);
            }
            var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//LineRemark");

            XmlElement lineElement = null;
            //find existing
            var nodeList1 = xmlDoc.SelectNodes(@"//LineRemark/Line");
            foreach (XmlNode node1 in nodeList1)
            {
                if (node1.Attributes != null && node1.Attributes["ID"] != null)
                {
                    string str1 = node1.Attributes["ID"].InnerText.Trim();
                    int id;
                    if (int.TryParse(str1, out id))
                    {
                        if (id == lineId)
                        {
                            lineElement = (XmlElement)node1;
                            break;
                        }
                    }
                }
            }

            //create new one if not found
            if (lineElement == null)
            {
                lineElement = xmlDoc.CreateElement("Line");
                lineElement.SetAttribute("ID", lineId.ToString());
                rootElement.AppendChild(lineElement);
            }
            lineElement.InnerText = remark;
            return xmlDoc.OuterXml;
        }
    }
}
