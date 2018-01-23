using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics.Contracts;

namespace WebSocketsPOC.Utils
{
    public abstract class StatisticsCollectorBase<TDataType>: IStatisticsCollector<TDataType>
    {
        protected ReaderWriterLockSlim rwLock;
        protected List<TDataType> sourceList;

        public StatisticsCollectorBase()
        {
            rwLock = new ReaderWriterLockSlim();
            sourceList = new List<TDataType>();
        }

        public TDataType Average
        {
            get
            {
                double doubleAverage = 0;
                using (rwLock.Read())
                {
                    doubleAverage = sourceList.Average(x => ExtractValue(x));
                }

                return CalculateAverage(doubleAverage);
            }
        }

        public TDataType[] DataPoints 
        {
            get 
            {
                TDataType[] dataPoints;
                using (rwLock.Read())
                {
                    dataPoints = sourceList.ToArray();
                }

                return dataPoints;
            }    
        }

        protected abstract double ExtractValue(TDataType data);
        protected abstract TDataType CalculateAverage(double doubleAvg);

        public void Add(TDataType time)
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
