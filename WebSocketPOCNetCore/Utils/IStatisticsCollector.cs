using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketsPOC.Utils
{
    public interface IStatisticsCollector<TDataType>
    {
        TDataType Average { get; }
        TDataType[] DataPoints { get; }
        void Add(TDataType time);
        void Reset();
    }
}
