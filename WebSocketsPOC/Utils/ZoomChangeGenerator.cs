using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketsPOC.Data;
using WebSocketsPOC.VM;

namespace WebSocketsPOC.Utils
{
    public class ZoomChangeGenerator
    {
        private POCViewModel vm;
        private Thread generatorThread;

        public ZoomChangeGenerator(POCViewModel vm)
        {
            this.vm = vm;
        }

        public bool IsRunning { get; private set; }

        public void Start()
        {
            Stop();

            IsRunning = true;
            generatorThread = new Thread(GenerateZoomChanges);
            generatorThread.IsBackground = true;
            generatorThread.Name = "GeneratorThread"; 
            generatorThread.Start();
        }

        public void Stop()
        {
            if(generatorThread != null)
            {
                IsRunning = false;
                generatorThread.Join();
            }
        }

        private void GenerateZoomChanges()
        {
            Random r = new Random(Guid.NewGuid().GetHashCode());
            while(IsRunning)
            {
                var boundingBoxRequest = new BoundingBoxRequest(
                    name: ConfigData.Instance.ClientName,
                    minLongitude: 30 + r.NextDouble() + 5,
                    minLatitude: 30 + r.NextDouble() * 5,
                    maxLongitude: 35 + r.NextDouble() * 5,
                    maxLatitude: 35 + r.NextDouble() * 5);

                vm.ChangeZoom(boundingBoxRequest);

                Console.WriteLine($"Sent server change zoom request with values: Name: {boundingBoxRequest.name} " +
                    $"MinLongitude: {boundingBoxRequest.minLongitude} MinLatitude: {boundingBoxRequest.minLatitude} " +
                    $"MaxLongitude: {boundingBoxRequest.maxLongitude} MaxLatitude: {boundingBoxRequest.maxLatitude}");

                Thread.Sleep(ConfigData.Instance.ZoomChangeInterval);
            }
        }
    }
}
