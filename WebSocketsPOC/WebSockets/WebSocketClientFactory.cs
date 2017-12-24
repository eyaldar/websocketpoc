using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketsPOC.Data;

namespace WebSocketsPOC.WebSockets
{
    public class WebSocketClientFactory
    {
        private Dictionary<WebSocketClientType, Func<string, int, IWebSocketClient>> typeToCreateFunction;

        public WebSocketClientFactory()
        {
            typeToCreateFunction = new Dictionary<WebSocketClientType, Func<string, int, IWebSocketClient>>
            {
                { WebSocketClientType.DeepStream, DeepStreamClient.Create },
                { WebSocketClientType.VertX, VertXClient.Create },
                //{ WebSocketClientType.SocketCluster, SocketClusterClient.Create },
                { WebSocketClientType.Default, DeepStreamClient.Create }
            };
        }

        public IWebSocketClient Create(string hostname, int port, WebSocketClientType clientType = WebSocketClientType.Default)
        {
            Func<string, int, IWebSocketClient> createFunc;
            if (typeToCreateFunction.TryGetValue(clientType, out createFunc))
            {
                return createFunc(hostname, port);
            }

            return typeToCreateFunction[WebSocketClientType.Default](hostname, port);
        }
    }
}
