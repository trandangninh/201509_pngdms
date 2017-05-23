using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;
using RepositoryPattern.Repositories;


namespace Repository.Implement
{
   
    public static class ProductPlanningRepository
    {
        public static Task<ProductPlanning> GetProductPlanningByIdAsync(this IRepositoryAsync<ProductPlanning> repository, int productPlanningId)
        {

            if (productPlanningId == 0)
            {
                throw new ArgumentException("Null or empty argument: productPlanningId");
            }
            return repository
                .Table
                .FirstOrDefaultAsync(x => x.Id == productPlanningId);

        }

        public static Task<ProductPlanning> GetProductPlanningByDateShiftLineAsync(this IRepositoryAsync<ProductPlanning> repository, DateTime dateTime,PlanShiftHardCodeType shift,PlanLineHardCodeType line)
        {

            var startTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
            var endTime = startTime.AddDays(1);

            return repository
                .Table
                .FirstOrDefaultAsync(x => x.CreatedDate<endTime&&x.CreatedDate>=startTime && x.Shift==shift && x.Line==line);

        }
    }
}
