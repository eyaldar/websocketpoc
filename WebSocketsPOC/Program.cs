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
            var factory = new WebSocketClientFactory();
            var client = factory.Create(ConfigData.Instance.Hostname, ConfigData.Instance.Port, ConfigData.Instance.ClientType);

            if(await client.InitializeAsync())
            {
                var vm = new POCViewModel(client);
                await vm.StartUpdatesListener();
            }

            await Task.Delay(30000);

            Console.ReadKey();
        }
    }
}
