using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domain.Logging;

using Entities.Domain.Users;

namespace Utils.Logging
{
    public class DbLogger:ILogger
    {
        public bool IsEnabled(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return false;
                default:
                    return true;
            }
        }

        public void DeleteLog(Log log)
        {
            throw new NotImplementedException();
        }

        public void ClearLog()
        {
            throw new NotImplementedException();
        }

        public IList<Log> GetAllLogs(DateTime? fromUtc, DateTime? toUtc, string message, LogLevel? logLevel, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Log GetLogById(int logId)
        {
            throw new NotImplementedException();
        }

        public IList<Log> GetLogByIds(int[] logIds)
        {
            throw new NotImplementedException();
        }

        public Log InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", User user = null)
        {
            throw new NotImplementedException();
        }
    }
}
