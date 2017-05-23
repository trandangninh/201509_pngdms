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


    public static class NoisMainMeasureConfigRepository
    {
        public static Task<List<NoisMainMeasureConfig>> GetMainMeasureByListLineIdAndDateAndMeasureAsync(this IRepositoryAsync<NoisMainMeasureConfig> repository, List<string> listLineCode, List<string> listMeasure)
        {
           
            //var notAsyncRepo = repository.GetRepository<NoisMainMeasureConfig>();
            //var listResultInDay = notAsyncRepo.Table;
            //var result = listResultInDay.Where(p => listLineCode.Contains(p.LineHardCode.ToString()));
            //result = result.Where(p => listMeasure.Contains(p.TypeHardCode.ToString()));
            //return result.ToListAsync();
            return null;
        }

        public static NoisMainMeasureConfig GetMainMeasureByLineCodeAndMeasureCodeAndDate(
            this IRepositoryAsync<NoisMainMeasureConfig> repository, string lineCode, string measureCode)
        {
            //if (!string.IsNullOrEmpty(lineCode))
            //{
            //    lineCode = lineCode.Replace("(", "");
            //    lineCode = lineCode.Replace(")", "");
            //}
            ////var endDay = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day + 1);
            //var notAsyncRepo = repository.GetRepository<NoisMainMeasureConfig>();
            //var result = notAsyncRepo.Table.FirstOrDefault(p=> p.LineHardCode.ToString() == lineCode && p.TypeHardCode.ToString() == measureCode);
            //return result;
            return null;
        }
    }
}
