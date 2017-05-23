using Entities.Domain.ClassificationDefects;
using Entities.Domain.Suppliers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Service.Suppliers
{
    public interface ISupplierService : IBaseService<Supplier>
    {
        /// <summary>
        /// Get all supplier
        /// </summary>
        /// <returns>paged list supplier</returns>
        Task<IPagedList<Supplier>> GetAllSupplierAsync(string searchBySupplierName = null, string searchVendorCode = null, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// check name of supplier existed or not
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true or false</returns>
        Task<bool> CheckNameHasExisted(string name);

        /// <summary>
        /// check supplier has existed by supplier identity
        /// </summary>
        /// <param name="supplierId"></param>
        /// <returns>true or false</returns>
        Task<bool> CheckSupplierHasExistedBySupplierId(int supplierId);

        /// <summary>
        /// delete list supplier
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        Task DeleteSuppliersAsync(List<int> listId);

        /// <summary>
        /// get supplier by name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        Task<Supplier> GetSupplierByNameAsync(string name);
    }
}
