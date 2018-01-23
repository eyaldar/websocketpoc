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
        private int entitiesCounter = 0;
        private BlockingCollection<DistributionWorkItem> pendingQueue;
        private IStatisticsCollector<double> triggerStatisticsCollector;
        private IStatisticsCollector<double> redisStatisticsCollector;
        private IStatisticsCollector<double> entitiesAmountStatisticsCollector;
        private IWebSocketClient client;
        private Task distributionWorkerTask;

        public DistributionWorker(IWebSocketClient client,
                                  IStatisticsCollector<double> triggerStatisticsCollector,
                                  IStatisticsCollector<double> redisStatisticsCollector,
                                  IStatisticsCollector<double> entitiesAmountStatisticsCollector,
                                  BlockingCollection<DistributionWorkItem> pendingQueue)
        {
            this.client = client;
            this.pendingQueue = pendingQueue;
            this.triggerStatisticsCollector = triggerStatisticsCollector;
            this.redisStatisticsCollector = redisStatisticsCollector;
            this.entitiesAmountStatisticsCollector = entitiesAmountStatisticsCollector;
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
                pendingQueue.CompleteAdding();
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

                    if (msg["triggerTime"] != null)  
                    {
                        Entity entity = JsonConvert.DeserializeObject<Entity>(msg.ToString());
                        TimeSpan triggerDelta = workItem.ArrivalTime.ToUniversalTime() - entity.triggerTime.ToUniversalTime();

                        entitiesAmountStatisticsCollector.Add(entitiesCounter);
                        entitiesCounter = 0;

                        triggerStatisticsCollector.Add(triggerDelta.TotalMilliseconds);
                        redisStatisticsCollector.Add(entity.redisDelta);
                        Console.Out.WriteLine($"redis delta avg: {redisStatisticsCollector.Average}ms " +
                                              $"trigger delta: {triggerStatisticsCollector.Average}ms, " +
                                              $"entities avg: {entitiesAmountStatisticsCollector.Average}");
                    }
                    else
                    {
                        entitiesCounter++;
                    }
                }
            }
        }
    }
}