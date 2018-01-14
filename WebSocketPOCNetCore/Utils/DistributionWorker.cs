using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using WebSocketsPOC.Data;
using WebSocketsPOC.WebSockets;

namespace WebSocketsPOC.Utils
{
    public class DistributionWorker
    {
        private BlockingCollection<DistributionWorkItem> pendingQueue;
        private IStatisticsCollector distributionStatisticsCollector;
        private IStatisticsCollector lastUpdateStatisticsCollector;
        private IWebSocketClient client;
        private Task distributionWorkerTask;

        public DistributionWorker(IWebSocketClient client,
                                  IStatisticsCollector distributionStatisticsCollector,
                                  IStatisticsCollector lastUpdateStatisticsCollector,
                                  BlockingCollection<DistributionWorkItem> pendingQueue)
        {
            this.client = client;
            this.pendingQueue = pendingQueue;
            this.distributionStatisticsCollector = distributionStatisticsCollector;
            this.lastUpdateStatisticsCollector = lastUpdateStatisticsCollector;
        }

        public bool IsRunning { get; private set; }

        public void Start()
        {
            Stop();

            IsRunning = true;
            distributionWorkerTask = new Task(HandleMessages, TaskCreationOptions.LongRunning);
            distributionWorkerTask.Start();
        }

        public void Stop()
        {
            if (distributionWorkerTask != null)
            {
                IsRunning = false;
                distributionWorkerTask.Wait();
            }
        }

        private void HandleMessages()
        {
            while (IsRunning)
            {
                foreach(var workItem in pendingQueue.GetConsumingEnumerable())
                { 
                    var msg = client.ExtractMessage(workItem.Data);

                    if (msg["action"].ToString() == "delete")
                    {
                        //Console.Out.WriteLine(msg);
                    }
                    else
                    {
                        Entity entity = JsonConvert.DeserializeObject<Entity>(msg.ToString());
                        var lastUpdateDelta = workItem.ArrivalTime - entity.lastUpdateTime;
                        var distributionDelta = workItem.ArrivalTime - entity.distributionTime;
                        //Console.Out.WriteLine(msg);
                        distributionStatisticsCollector.Add(distributionDelta);
                        lastUpdateStatisticsCollector.Add(lastUpdateDelta);
                        Console.Out.WriteLine($"last update avg: {lastUpdateStatisticsCollector.Average.TotalMilliseconds}ms distribution delta: {distributionStatisticsCollector.Average.TotalMilliseconds}ms");
                    }
                }
            }
        }
    }
}