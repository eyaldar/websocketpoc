using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketsPOC.Data
{
    public class BoundingBoxRequest
    {
        public BoundingBoxRequest(string name, double minLongitude, double minLatitude, double maxLongitude, double maxLatitude)
        {
            this.name = name;
            this.minLongitude = minLongitude;
            this.minLatitude = minLatitude;
            this.maxLongitude = maxLongitude;
            this.maxLatitude = maxLatitude;
        }

        public string name { get; set; }
        public double minLongitude { get; set; }
        public double minLatitude { get; set; }
        public double maxLongitude { get; set; }
        public double maxLatitude { get; set; }
    }
}
