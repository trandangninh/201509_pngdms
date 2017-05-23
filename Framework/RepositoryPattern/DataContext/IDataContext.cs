#region

using System;
using System.Data.Entity;

#endregion

namespace RepositoryPattern.DataContext
{
    public interface IDataContext : IDisposable
    {
        int SaveChanges();
        /// <summary>
        /// Get DbSet
        /// </summary>
        /// <typeparam name="T">Base entity type</typeparam>
        /// <returns>DbSet</returns>
        IDbSet<T> Set<T>() where T : BaseEntity;
    }
}