using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketsPOC.Data;
using WebSocketsPOC.Utils;
using WebSocketsPOC.WebSockets;

namespace WebSocketsPOC.VM
{
    public class POCViewModel: IDisposable
    {
        private BlockingCollection<DistributionWorkItem> workQueue;
        private IWebSocketClient client;
        private ZoomChangeGenerator generator;
        private List<DistributionWorker> workers;

        public string ClientName { get; protected set; }
        public bool IsInitialized { get; protected set; }
        public Dictionary<string, Entity> IDToEntity { get; protected set; }

        public POCViewModel(IWebSocketClient client, string clientName)
        {
            this.client = client;
            this.ClientName = clientName;
            IDToEntity = new Dictionary<string, Entity>();
            generator = new ZoomChangeGenerator(this);
            generator.Start();

            workQueue = new BlockingCollection<DistributionWorkItem>();
            workers = new List<DistributionWorker>();

            for (int i = 0; i < ConfigData.Instance.Workers; i++)
            {
                var worker = new DistributionWorker(client, workQueue);
                workers.Add(worker);
                worker.Start();
            }
        }

        public void ChangeZoom(BoundingBoxRequest boundingBox)
        {
            client.Publish("setClientState", boundingBox);
        }

        public async Task StartUpdatesListener()
        {
            await client.SubscribeAsync(ClientName, OnEntityArrived);
        }

        private void OnEntityArrived(object data)
        {
            workQueue.Add(new DistributionWorkItem { Data = data, ArrivalTime = DateTime.UtcNow });
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

                foreach (var worker in workers)
                {
                    worker.Stop();
                }

                workers.Clear();
            }
        }

        ~POCViewModel()
        {
            Dispose(false);
        }

        #endregion
    }
}
