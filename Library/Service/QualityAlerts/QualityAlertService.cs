using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Domain.QualityAlerts;
using RepositoryPattern.Repositories;
using Utils.Caching;
using Utils;
using Entities.Domain.Categories;
using Entities.Domain.ClassificationDefects;
using Entities.Domain.Suppliers;
using Entities.Domain;
using Entities.Domain.Users;
using Entities.Domain.Classifications;
using System.Data.Entity;
using Entities.Domain.FoundByFunction;

namespace Service.QualityAlerts
{
    public partial class QualityAlertService : BaseService<QualityAlert>, IQualityAlertService
    {
        protected override string PatternKey
        {
            get { return "PG.qualityalert."; }
        }

        private readonly IRepositoryAsync<QualityAlert> _qualityAlertRepositoryAsync;
        private readonly IRepositoryAsync<Category> _categroyRepositoryAsync;
        private readonly IRepositoryAsync<ClassificationDefect> _cDefectRepositoryAsync;
        private readonly IRepositoryAsync<Supplier> _supplierRepositoryAsync;
        private readonly IRepositoryAsync<Line> _lineRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<Classification> _classificationRepositoryAsync;
        private readonly IRepositoryAsync<FoundByFunction> _foundByFunctionRepositoryAsync;
        private readonly ICacheManager _cacheManager;

        public QualityAlertService(IRepositoryAsync<QualityAlert> qualityAlerRepositoryAsync,
            ICacheManager cacheManager,
            IRepositoryAsync<Category> categroyRepositoryAsync,
            IRepositoryAsync<ClassificationDefect> cDefectRepositoryAsync,
            IRepositoryAsync<Supplier> supplierRepositoryAsync,
            IRepositoryAsync<Line> lineRepositoryAsync,
            IRepositoryAsync<User> userRepositoryAsync,
            IRepositoryAsync<Classification> classificationRepositoryAsync,
            IRepositoryAsync<FoundByFunction> foundByFunctionRepositoryAsync)
            : base(qualityAlerRepositoryAsync, cacheManager)
        {
            this._qualityAlertRepositoryAsync = qualityAlerRepositoryAsync;
            this._cacheManager = cacheManager;
            this._categroyRepositoryAsync = categroyRepositoryAsync;
            this._cDefectRepositoryAsync = cDefectRepositoryAsync;
            this._supplierRepositoryAsync = supplierRepositoryAsync;
            this._lineRepositoryAsync = lineRepositoryAsync;
            this._userRepositoryAsync = userRepositoryAsync;
            this._classificationRepositoryAsync = classificationRepositoryAsync;
            this._foundByFunctionRepositoryAsync = foundByFunctionRepositoryAsync;
        }

        public Task<IPagedList<QualityAlert>> SearchQualityAlertAsync(int? lineId = null, DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _qualityAlertRepositoryAsync.Table.OrderByDescending(x => x.CreatedDate).AsQueryable();
            if (lineId != null)
            {
                query = query.Where(x => x.LineId == lineId);
            }
            if (startDate != null)
            {
                //DateTime startDayTemp = startDate.Value.AddDays(-1);
                query = query.Where(x => x.CreatedDate > startDate);
            }
            if (endDate != null)
            {
                //DateTime endDayTemp = endDate.Value.AddDays(1);
                query = query.Where(x => x.CreatedDate < endDate);
            }

            return Task.FromResult(new PagedList<QualityAlert>(query, pageIndex, pageSize) as IPagedList<QualityAlert>);
        }

        public Task<IPagedList<QualityAlertFullObject>> SearchQualityAlertObjectAsync(int departmentId = 0,
            int userId = 0,
                                                                                        int lineId = 0,
                                                                                        DateTime? startDate = null,
                                                                                        DateTime? endDate = null,
                                                                                        List<int> supplierIds = null,
                                                                                        int categoryId = 0,
                                                                                        int complaintTypeId = 0,
                                                                                        int classificationDefectId = 0,
                                                                                        int defectRepeatId = 0,
                                                                                        DateTime? supplierReplayDate = null,
                                                                                        int statusId = 0,
                                                                                        int foundByFunctionId=0,
                                                                                        int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from q in _qualityAlertRepositoryAsync.Table
                        join c in _categroyRepositoryAsync.Table
                        on q.CategoryId equals c.Id into ct
                        join d in _cDefectRepositoryAsync.Table
                        on q.ClassificationDefectId equals d.Id into dt
                        join s in _supplierRepositoryAsync.Table
                        on q.SupplierId equals s.Id into st
                        join u in _userRepositoryAsync.Table
                        on q.UserId equals u.Id into ut
                        join l in _lineRepositoryAsync.Table
                        on q.LineId equals l.Id into lt
                        join o in _userRepositoryAsync.Table
                        on q.OwnerId equals o.Id into ot
                        join cl in _classificationRepositoryAsync.Table
                        on q.ClassificationId equals cl.Id into clt
                        join f in _foundByFunctionRepositoryAsync.Table
                        on q.FoundByFunctionId equals f.Id into ff
                        from cObj in ct.DefaultIfEmpty()
                        from dObj in dt.DefaultIfEmpty()
                        from sObj in st.DefaultIfEmpty()
                        from uObj in ut.DefaultIfEmpty()
                        from lObj in lt.DefaultIfEmpty()
                        from oObj in ot.DefaultIfEmpty()
                        from clObj in clt.DefaultIfEmpty()
                        from ffObj in ff.DefaultIfEmpty()
                        select new QualityAlertFullObject
                        {
                            Id = q.Id,
                            Action = q.Action,
                            AlertDateTime = q.AlertDateTime,
                            CategoryId = q.CategoryId,
                            CategoryName = cObj.Name ?? "",
                            ClassificationDefectId = q.ClassificationDefectId,
                            ClassificationDefectName = dObj.Name ?? "",
                            ClassificationId = q.ClassificationId,
                            ClassificationName = clObj.Name ?? "",
                            ComplaintTypeId = q.ComplaintTypeId,
                            //CostImpacted = q.CostImpacted,
                            CreatedDate = q.CreatedDate,
                            DefectRepeatId = q.DefectRepeatId,
                            Detail = q.Detail,
                            FollowUpAction = q.FollowUpAction,
                            GCAS = q.GCAS,
                            DepartmentId = lObj == null ? 0 : lObj.DepartmentId,
                            LineId = q.LineId,
                            LineName = lObj.LineName ?? "",
                            Machine = q.Machine,
                            //MaterialId = q.MaterialId,
                            Material = q.Material,
                            NumBlock = q.NumBlock,
                            OwnerId = q.OwnerId,
                            OwnerName = oObj.Username ?? "",
                            PRLossPercent = q.PRLossPercent,
                            QualityAlertStatusId = q.QualityAlertStatusId,
                            SAPLot = q.SAPLot,
                            SupplierId = q.SupplierId,
                            SupplierName = sObj != null ? (sObj.Name ?? "") : "",
                            SupplierEmail = sObj.VendorContact,
                            SupplierLocation = sObj != null ? sObj.LocationTypeId : 1,
                            SupplierLot = q.SupplierLot,
                            SupplierReplyDate = q.SupplierReplyDate,
                            Unit = q.Unit,
                            UserId = q.UserId,
                            UserName = uObj.Username ?? "",
                            When = q.When,
                            UserNameCreated = q.UserNameCreated,
                            QuantityReturn = q.QuantityReturn,
                            NumStop = q.NumStop,
                            DownTime = q.DownTime,
                            InformedToSupplierDate = q.InformedToSupplierDate,
                            DefectedQty = q.DefectedQty,
                            PGerEffortLoss = q.PGerEffortLoss,
                            ContractorEffortLoss = q.ContractorEffortLoss,
                            QARelatedToMaterials = q.QARelatedToMaterials,
                            QARelatedToFG = q.QARelatedToFG,
                            FoundByFunctionId = q.FoundByFunctionId,
                            FoundByFunctionName = ffObj.Name ?? "",                            
                            ClassificationRPN = q.ClassificationRPN

                        };

            if (departmentId != 0)
            {
                query = query.Where(x => x.DepartmentId == departmentId);
            }
            if (lineId != 0)
            {
                query = query.Where(x => x.LineId == lineId);
            }
            if (userId != 0)
            {
                var lineIds = _lineRepositoryAsync.Table.Where(l => l.Users.Any(u => u.Id == userId)).Select(l => l.Id).ToList();
                query = query.Where(x => lineIds.Contains(x.LineId));
            }
            if (startDate != null)
            {
                //DateTime startDayTemp = startDate.Value.AddDays(-1);
                query = query.Where(x => x.AlertDateTime > startDate);
            }
            if (endDate != null)
            {
                //DateTime endDayTemp = endDate.Value.AddDays(1);
                query = query.Where(x => x.AlertDateTime < endDate);
            }

            if (supplierIds != null && supplierIds.Count > 0 && supplierIds.FirstOrDefault() != 0)
                query = query.Where(x => supplierIds.Any(s => s == x.SupplierId));

            if (categoryId != 0)
                query = query.Where(x => x.CategoryId == categoryId);

            if (complaintTypeId != 0)
                query = query.Where(x => x.ComplaintTypeId == complaintTypeId);

            if (classificationDefectId != 0)
                query = query.Where(x => x.ClassificationDefectId == classificationDefectId);

            if (defectRepeatId != 0)
                query = query.Where(x => x.DefectRepeatId == defectRepeatId);

            if (supplierReplayDate != null)
                query = query.Where(x => x.SupplierReplyDate == supplierReplayDate.Value);

            if (statusId != 0)
                query = query.Where(x => x.QualityAlertStatusId == statusId);
            if (foundByFunctionId != 0)
            {
                query = query.Where(x => x.FoundByFunctionId == foundByFunctionId);
            }

            query = query.OrderByDescending(x => x.CreatedDate);

            return Task.FromResult(new PagedList<QualityAlertFullObject>(query, pageIndex, pageSize) as IPagedList<QualityAlertFullObject>);
        }

        public Task DeleteQualityAlertAsync(List<int> listId)
        {
            if (listId == null)
                throw new ArgumentNullException("listId");
            var deletedQualityAlert = _qualityAlertRepositoryAsync.Table.Where(q => listId.Contains(q.Id));
            _cacheManager.RemoveByPattern(PatternKey);
            return _qualityAlertRepositoryAsync.DeleteAsync(deletedQualityAlert);
        }

        public int CountAllQualityAlertByClassificationIdAsync(int classificationId)
        {
            var count = _qualityAlertRepositoryAsync.Table.Count(x => x.ClassificationId == classificationId);
            return count;
        }

        public int CountQuanlityAllertByLineAndDate(int lineId, DateTime date)
        {
            DateTime start;
            DateTime end;
            if (date.DayOfWeek == DayOfWeek.Monday)
            {
                start = date.AddDays(-3);
                start = new DateTime(start.Year, start.Month, start.Day, 7, 0, 0);
                end = start.AddDays(3);
            }
            else
            {
                start = date.AddDays(-1);
                start = new DateTime(start.Year, start.Month, start.Day, 7, 0, 0);
                end = start.AddDays(1);
            }
            return _qualityAlertRepositoryAsync.Table.Count(q => q.LineId == lineId && q.CreatedDate >= start && q.CreatedDate <= end);
        }

        public Task<List<Tuple<int, string>>> QualityAlertComplaintTracking()
        {
            var startDate = new DateTime(DateTime.Now.Year - 1, 6, 1);
            var endDate = new DateTime(DateTime.Now.Year, 5, 1);
            startDate = startDate.AddDays(-1);
            endDate = endDate.AddDays(1);
            var query = _qualityAlertRepositoryAsync.Table.Where(x => x.CreatedDate > startDate && x.CreatedDate < endDate)
                .OrderBy(x => x.CreatedDate)
                .GroupBy(qa => qa.CreatedDate.Month);

            return query.Select(g => new Tuple<int, string>(g.Key, String.Join(",", g.GroupBy(qa => qa.ClassificationDefectId).Select(cg => cg.Count())))).ToListAsync();
        }

        public List<QualityAlert> QualityAlertByClassificationId(int classificationId, DateTime? lastDate)
        {
            var quanlityAlerts = _qualityAlertRepositoryAsync.Table.AsQueryable().Where(x => x.ClassificationId == classificationId);

            if (lastDate != null)
                quanlityAlerts = quanlityAlerts.Where(x => x.CreatedDate >= lastDate);
           
            return quanlityAlerts.ToList();
        }
    }
}
