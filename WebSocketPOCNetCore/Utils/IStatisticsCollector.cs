using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketsPOC.Utils
{
    public interface IStatisticsCollector
    {
        TimeSpan Average { get; }
        void Add(TimeSpan time);
        void Reset();
    }
}
