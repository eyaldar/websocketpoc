using System;
using WebSocketsPOC.Utils;

namespace WebSocketPOCNetCore.Utils
{
    public class NumberStatisticsCollector: StatisticsCollectorBase<double>
    {
        protected override double ExtractValue(double data)
        {
            return data;
        }

        protected override double CalculateAverage(double doubleAvg)
        {
            return doubleAvg;
        }
    }
}
