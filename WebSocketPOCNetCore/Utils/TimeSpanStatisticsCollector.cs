using System;
using System.Linq;
using WebSocketsPOC.Utils;

namespace WebSocketPOCNetCore.Utils
{
    public class TimeSpanStatisticsCollector: StatisticsCollectorBase<TimeSpan>
    {
        protected override double ExtractValue(TimeSpan data)
        {
            return data.Ticks;
        }

        protected override TimeSpan CalculateAverage(double doubleAvg)
        {
            long avg = Convert.ToInt64(doubleAvg);

            return TimeSpan.FromTicks(avg);
        }
    }
}
