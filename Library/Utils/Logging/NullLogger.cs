using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain.Logging;

using Entities.Domain.Users;

namespace Utils.Logging
{
    public class NullLogger:ILogger
    {
        public bool IsEnabled(LogLevel level)
        {
            return false;
        }

        public void DeleteLog(Log log)
        {
           
        }

        public void ClearLog()
        {
           
        }

        public IList<Log> GetAllLogs(DateTime? fromUtc, DateTime? toUtc, string message, LogLevel? logLevel, int pageIndex, int pageSize)
        {
            return new List<Log>();
        }

        public Log GetLogById(int logId)
        {
            return null;
        }

        public IList<Log> GetLogByIds(int[] logIds)
        {
            return new List<Log>();
        }

        public Log InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", User user = null)
        {
            return null;
        }
    }
}
