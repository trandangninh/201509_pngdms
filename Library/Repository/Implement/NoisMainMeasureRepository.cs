using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain;
using RepositoryPattern.Infrastructure;
using RepositoryPattern.Repositories;

namespace Repository.Implement
{


    public static class NoisMainMeasureRepository
    {
        public static Task<List<NoisMainMeasure>> GetMainMeasureByListLineIdAndDateAndMeasureAsync(this IRepositoryAsync<NoisMainMeasure> repository, DateTime createdDate, List<string> listLineCode, List<string> listMeasure)
        {
            var startDay = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day);
            var endDay = startDay.AddDays(1);
            //var notAsyncRepo = repository.GetRepository<NoisMainMeasure>();
            //var listResultInDay = notAsyncRepo.Table.Where(p => p.CreatedDateTime < endDay && p.CreatedDateTime >= startDay);
            //var result = listResultInDay.Where(p => listLineCode.Contains(p.LineHardCode.ToString()));
            //result = result.Where(p => listMeasure.Contains(p.TypeHardCode.ToString()));
            //return result.ToListAsync();
            return null;
        }

        public static NoisMainMeasure GetMainMeasureByLineCodeAndMeasureCodeAndDate(
            this IRepositoryAsync<NoisMainMeasure> repository, DateTime createdDate, string lineCode, string measureCode)
        {
            if (!string.IsNullOrEmpty(lineCode))
            {
                lineCode = lineCode.Replace("(", "");

                lineCode = lineCode.Replace(")", "");
            }
            var startDay = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day);
            var endDay = startDay.AddDays(1);
            //var endDay = new DateTime(createdDate.Year, createdDate.Month, createdDate.Day + 1);
            var notAsyncRepo = EngineContext.Current.Resolve<IRepositoryAsync<NoisMainMeasure>>();//repository.GetRepository<NoisMainMeasure>();
            var result = notAsyncRepo.Table.FirstOrDefault(p => p.CreatedDateTime < endDay && p.CreatedDateTime >= startDay && p.LineHardCode.ToString() == lineCode && p.TypeHardCode.ToString() == measureCode);
            return result;
        }
    }
}
