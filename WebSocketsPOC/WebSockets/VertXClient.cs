using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using io.vertx;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketsPOC.Data;

namespace WebSocketsPOC.WebSockets
{
    public class VertXClient : DisposableWebSocketClient
    {
        private Headers defaultHeaders;
        private Eventbus eventBus;
        private List<(string Topic, Handlers Handler)> subscriptions;

        public VertXClient(string hostname, int port)
        {
            eventBus = new Eventbus(hostname, port);
            subscriptions = new List<(string topic, Handlers handler)>();
            defaultHeaders = new Headers();
        }

        public override Task<bool> InitializeAsync()
        {
            return Task.FromResult(eventBus.isConnected());
        }

        public override void Publish<TDataType>(string topic, TDataType data)
        {
            string json = JsonConvert.SerializeObject(data);
            eventBus.publish(topic, JObject.Parse(json), defaultHeaders);
        }

        public override Task SubscribeAsync(string topic, Action<object> handler)
        {
            var handlers = new Handlers(topic, handler);
            subscriptions.Add((topic, handlers));

            eventBus.register(topic, handlers);

            return Task.FromResult(0);
        }

        public static IWebSocketClient Create(string hostname, int port)
        {
            return new VertXClient(hostname, port);
        }

        #region Disposable Pattern

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var topicToHandler in subscriptions)
                {
                    eventBus.unregister(topicToHandler.Topic);
                }

                subscriptions.Clear();

                if(eventBus.isConnected())
                    eventBus.CloseConnection(1000);
            }
        }

        #endregion
    }
}
