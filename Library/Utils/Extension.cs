using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class Extension
    {
        public static Dictionary<int, string> EnumToList<TEnum>(this TEnum enumObj, int[] valuesToExclude = null) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new ArgumentException("An Enumeration type is required.", "enumObj");

            //var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            //var workContext = EngineContext.Current.Resolve<IWorkContext>();

            var values = from TEnum enumValue in Enum.GetValues(typeof(TEnum))
                         where valuesToExclude == null || !valuesToExclude.Contains(Convert.ToInt32(enumValue))
                         select new { ID = Convert.ToInt32(enumValue), Name = enumValue.ToString() };
            var result = new Dictionary<int, string>();
            Enumerable.ToList(values).ForEach(v => result.Add(v.ID,v.Name));
            
            return result;
        }

        public static DateTime Parse(string dateString, DateTime defaultDate)
        {
            DateTime result;

            if (String.IsNullOrEmpty(dateString)) return defaultDate;
            else
            {
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                const DateTimeStyles styles = DateTimeStyles.None;
                if (!DateTime.TryParse(dateString, culture, styles, out result))
                {
                    result = defaultDate;
                }
            }
            return result;
        }
        public static DateTime ParseKendoDateTimeString(string dateString, DateTime defaultDate)
        {
            DateTime result;
            if (String.IsNullOrEmpty(dateString)) result = defaultDate;
            else
            {
                if (!DateTime.TryParseExact(dateString, "MM/dd/yyyy", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out result))
                {
                    if(!DateTime.TryParseExact(dateString.Substring(0, 24),
                        "ddd MMM d yyyy HH:mm:ss",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                        result = defaultDate;
                }
            }
            return result;
        }

        public static Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize)
        {
            return Task.Run(() => new PagedList<T>(source, pageIndex, pageSize) as IPagedList<T>);
        }
    }
}
