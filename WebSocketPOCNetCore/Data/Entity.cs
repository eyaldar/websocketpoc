using System;

namespace WebSocketsPOC.Data
{
    public class Entity
    {
        public Entity()
        {

        }
        
        public Entity(string id, double longitude, double latitude, DateTime triggerTime, double redisDelta)
        {
            this.entityId = id;
            this.longitude = longitude;
            this.latitude = latitude;
            this.triggerTime = triggerTime;
            this.redisDelta = redisDelta;
        }

        public string entityId { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public DateTime triggerTime { get; set; }
        public double redisDelta { get; set; }
    }
}
