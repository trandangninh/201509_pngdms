using Entities.Domain.Suppliers;
using RepositoryPattern.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Utils.Caching;
using System.Data.Entity;
using Entities.Domain.ClassificationDefects;

namespace Service.Suppliers
{
    public class SupplierService : BaseService<Supplier>, ISupplierService
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        /// </remarks>
        private const string SUPPLIER_LISTPAGED_KEY = "PG.Supplier.ListPaged-{0}-{1}-{2}-{3}";

        private const string SUPPLIER_BY_NAME = "PG.Supplier.ByName-{0}";

        protected override string PatternKey
        {
            get { return "PG.Supplier."; }
        }

        private readonly ICacheManager _cacheManager;
        private readonly IRepositoryAsync<Supplier> _supplierRepositoryAsync;

        public SupplierService(ICacheManager cacheManager,
            IRepositoryAsync<Supplier> supplierRepositoryAsync)
            : base(supplierRepositoryAsync,cacheManager)
        {
            _supplierRepositoryAsync = supplierRepositoryAsync;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Get all supplier
        /// </summary>
        /// <returns>paged list supplier</returns>
        public Task<IPagedList<Supplier>> GetAllSupplierAsync(string searchBySupplierName = null, string searchVendorCode = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = string.Format(SUPPLIER_LISTPAGED_KEY, searchBySupplierName, searchVendorCode, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = _supplierRepositoryAsync.Table.AsQueryable();

                if (!string.IsNullOrEmpty(searchBySupplierName))
                    query = query.Where(s => s.Name.Contains(searchBySupplierName));

                if (!string.IsNullOrEmpty(searchVendorCode))
                    query = query.Where(s => s.VendorCode.Contains(searchVendorCode));

                //default sort by name of supplier
                query = query.OrderBy(s => s.Name);
                return Task.FromResult(new PagedList<Supplier>(query, pageIndex, pageSize) as IPagedList<Supplier>);
            }
            );
        }

        /// <summary>
        /// check name of supplier existed or not
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true or false</returns>
        public Task<bool> CheckNameHasExisted(string name)
        {
            return Task.FromResult(_supplierRepositoryAsync.Table.Any(s => s.Name == name));
        }

        /// <summary>
        /// check supplier has existed by supplier identity
        /// </summary>
        /// <param name="supplierId"></param>
        /// <returns>true or false</returns>
        public Task<bool> CheckSupplierHasExistedBySupplierId(int supplierId)
        {
            return Task.FromResult(_supplierRepositoryAsync.Table.Any(s => s.Id == supplierId));
        }

        /// <summary>
        /// delete list supplier
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        public async Task DeleteSuppliersAsync(List<int> listId)
        {
            if (listId == null)
                throw new ArgumentNullException("listId is null");
            var deletedSuppliers = _supplierRepositoryAsync.Table.Where(i => listId.Contains(i.Id)).ToList();
            _cacheManager.RemoveByPattern(PatternKey);

            foreach (var item in deletedSuppliers)
            {
                item.ClassificationDefects.Clear();

                await _supplierRepositoryAsync.UpdateAsync(item);
            }

            await _supplierRepositoryAsync.DeleteAsync(deletedSuppliers);
        }

        /// <summary>
        /// get supplier by name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public Task<Supplier> GetSupplierByNameAsync(string name)
        {
            var key = string.Format(SUPPLIER_BY_NAME, name);
            return _cacheManager.Get(key, () => _supplierRepositoryAsync.Table.FirstOrDefaultAsync(c => c.Name == name));
        }
    }
}
