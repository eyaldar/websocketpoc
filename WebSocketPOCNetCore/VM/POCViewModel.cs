using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebSocketsPOC.Data;
using WebSocketsPOC.Utils;
using WebSocketsPOC.WebSockets;
using WebSocketPOCNetCore.Utils;

namespace WebSocketsPOC.VM
{
    public class POCViewModel: IDisposable
    {
        private BlockingCollection<DistributionWorkItem> workQueue;
        private IWebSocketClient client;
        private ZoomChangeGenerator generator;
        private IStatisticsCollector<double> triggerStatisticsCollector;
        private IStatisticsCollector<double> redisStatisticsCollector;
        private IStatisticsCollector<double> entitiesAmountStatisticsCollector;
        private ICsvExporter csvExporter;
        private List<DistributionWorker> workers;

        public string ClientName { get; protected set; }
        public bool IsInitialized { get; protected set; }
        public Dictionary<string, Entity> IDToEntity { get; protected set; }

        public POCViewModel(IWebSocketClient client, string clientName, BoundingBoxRequest bbr, string clientIndex)
        {
            this.client = client;
            this.ClientName = clientName;
            IDToEntity = new Dictionary<string, Entity>();

            triggerStatisticsCollector = new NumberStatisticsCollector();
            redisStatisticsCollector = new NumberStatisticsCollector();
            entitiesAmountStatisticsCollector = new NumberStatisticsCollector();

            csvExporter = new CsvHelperCsvExporter(clientIndex);

            generator = new ZoomChangeGenerator(this, bbr);
            generator.Start();

            workQueue = new BlockingCollection<DistributionWorkItem>();
            workers = new List<DistributionWorker>();

            for (int i = 0; i < ConfigData.Instance.Workers; i++)
            {
                var worker = new DistributionWorker(client, triggerStatisticsCollector, redisStatisticsCollector, entitiesAmountStatisticsCollector, workQueue);
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

                CreateCSVs();
            }
        }

        private void CreateCSVs()
        {
            csvExporter.Export("timespanData.csv",
                               new string[] { "Redis Delta", "Trigger Delta" },
                               new double[][]
                                {
                                        redisStatisticsCollector.DataPoints,
                                        triggerStatisticsCollector.DataPoints
                                });
            csvExporter.Export("numberOfEntities.csv",
               new string[] { "Num of Entities" },
               new double[][]
                {
                        entitiesAmountStatisticsCollector.DataPoints,
                });
        }

        ~POCViewModel()
        {
            Dispose(false);
        }

        #endregion
    }
}
