using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketsPOC.Data;
using WebSocketsPOC.Utils;
using WebSocketsPOC.WebSockets;

namespace WebSocketsPOC.VM
{
    public class POCViewModel: IDisposable
    {
        private IWebSocketClient client;
        private ZoomChangeGenerator generator;

        public bool IsInitialized { get; protected set; }
        public Dictionary<string, Entity> IDToEntity { get; protected set; }

        public POCViewModel(IWebSocketClient client)
        {
            this.client = client;
            IDToEntity = new Dictionary<string, Entity>();
            generator = new ZoomChangeGenerator(this);
            generator.Start();
        }

        public void ChangeZoom(BoundingBoxRequest boundingBox)
        {
            client.Publish("setClientState", boundingBox);
        }

        public async Task StartUpdatesListener()
        {
            await client.SubscribeAsync(ConfigData.Instance.ClientName, OnEntityArrived);
        }

        private void OnEntityArrived(object data)
        {
            Console.WriteLine(data);
        }

        #region Disposable Pattern

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                generator.Stop();
                generator = null;

                client.Dispose();
                client = null;
            }
        }

        ~POCViewModel()
        {
            Dispose(false);
        }

        #endregion
    }
}
