using System;

namespace WebSocketsPOC.Data
{
    public class Entity
    {
        public Entity()
        {

        }
        
        public Entity(string id, double longitude, double latitude, DateTime lastUpdateTime, double someData, string action)
        {
            this.id = id;
            this.longitude = longitude;
            this.latitude = latitude;
            this.lastUpdateTime = lastUpdateTime;
            this.someData = someData;
            this.action = action;
        }

        public string id { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public DateTime lastUpdateTime { get; set; }
        public DateTime distributionTime { get; set; }
        public double someData { get; set; }
        public string action { get; set; }
    }
}
