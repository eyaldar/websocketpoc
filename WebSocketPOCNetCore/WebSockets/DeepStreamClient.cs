using DeepStreamNet.Contracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketsPOC.Data;

namespace WebSocketsPOC.WebSockets
{
    public class DeepStreamClient : DisposableWebSocketClient
    {
        private DeepStreamNet.DeepStreamClient client;
        private List<IAsyncDisposable> subscriptions;

        public DeepStreamClient(string hostname, int port)
        {
            subscriptions = new List<IAsyncDisposable>();
            client = new DeepStreamNet.DeepStreamClient(hostname, (short)port);
        }

        public override JObject ExtractMessage(object data)
        {
            return data as JObject;
        }

        public override Task<bool> InitializeAsync()
        {
            return client.LoginAsync(Constants.DeepStreamDefaultUsername, Constants.DeepStreamDefaultPassword);
        }

        public override void Publish<TDataType>(string topic, TDataType data)
        {
            client.Events.Publish(topic, data);
        }

        public override async Task SubscribeAsync(string topic, Action<object> handler)
        {
            var disp = await client.Events.SubscribeAsync(topic, handler);
            subscriptions.Add(disp);
        }

        public static IWebSocketClient Create(string hostname, int port)
        {
            return new DeepStreamClient(hostname, port);
        }

        #region Disposable Pattern

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                var tasks = new List<Task>();
                foreach (var disp in subscriptions)
                {
                    var task = disp.DisposeAsync();
                    tasks.Add(task);
                }

                Task.WaitAll(tasks.ToArray());

                client.Dispose();
                client = null;
            }
        }

        #endregion
    }
}
