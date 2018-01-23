using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketsPOC.Data;
using WebSocketsPOC.VM;
using WebSocketsPOC.WebSockets;

namespace WebSocketsPOC
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 6)
                return;

            POCViewModel vm = null;
            var rnd = new Random(Guid.NewGuid().GetHashCode());
            var clientName = ConfigData.Instance.ClientBaseName + rnd.Next();
            var bbr = new BoundingBoxRequest(name: clientName,
                                             minLongitude: int.Parse(args[1]),
                                             minLatitude: int.Parse(args[3]),
                                             maxLongitude: int.Parse(args[2]),
                                             maxLatitude: int.Parse(args[4]));

            var factory = new WebSocketClientFactory();
            var client = factory.Create(ConfigData.Instance.Hostname, ConfigData.Instance.Port, ConfigData.Instance.ClientType);

            if(await client.InitializeAsync())
            {
                vm = new POCViewModel(client, clientName, bbr);
                await vm.StartUpdatesListener();
            }

            await Task.Delay(int.Parse(args[5]));

            if(vm != null)
                vm.Dispose();

            Environment.Exit(0);
        }
    }
}
