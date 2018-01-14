using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketsPOC.Utils
{
    public class StatisticsCollector: IStatisticsCollector
    {
        private ReaderWriterLockSlim rwLock;
        private List<TimeSpan> sourceList;

        public StatisticsCollector()
        {
            rwLock = new ReaderWriterLockSlim();
            sourceList = new List<TimeSpan>();
        }

        public TimeSpan Average
        {
            get
            {
                double doubleAverageTicks = 0;
                using (rwLock.Read())
                {
                    doubleAverageTicks = sourceList.Average(timeSpan => timeSpan.Ticks);
                }

                long longAverageTicks = Convert.ToInt64(doubleAverageTicks);

                return new TimeSpan(longAverageTicks);
            }
        }

        public void Add(TimeSpan time)
        {
            using (rwLock.Write())
            {
                sourceList.Add(time);
            }
        }

        public void Reset()
        {
            using (rwLock.Write())
            {
                sourceList.Clear();
            }
        }
    }
}
