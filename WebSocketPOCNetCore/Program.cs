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
            POCViewModel vm = null;
            var rnd = new Random(Guid.NewGuid().GetHashCode());
            var clientName = ConfigData.Instance.ClientBaseName + rnd.Next();
            var factory = new WebSocketClientFactory();
            var client = factory.Create(ConfigData.Instance.Hostname, ConfigData.Instance.Port, ConfigData.Instance.ClientType);

            if(await client.InitializeAsync())
            {
                vm = new POCViewModel(client, clientName);
                await vm.StartUpdatesListener();
            }

            await Task.Delay(ConfigData.Instance.TotalRuntime);

            if(vm != null)
                vm.Dispose();

            Environment.Exit(0);
        }
    }
}
