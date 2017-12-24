using System;

namespace WebSocketsPOC.Data
{
    public class Entity
    {
        public Entity()
        {

        }
        
        public Entity(string id, double longitude, double latitude, DateTime lastUpdateTime, double someData)
        {
            ID = id;
            Longitude = longitude;
            Latitude = latitude;
            LastUpdateTime = lastUpdateTime;
            SomeData = someData;
        }

        public string ID { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public double SomeData { get; set; }
    }
}
