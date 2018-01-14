using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using WebSocketsPOC.Data;

namespace WebSocketsPOC.WebSockets
{
    public interface IWebSocketClient: IDisposable
    {
        Task<bool> InitializeAsync();
        void Publish<TDataType>(string topic, TDataType data);
        Task SubscribeAsync(string topic, Action<object> handler);
        JObject ExtractMessage(object data);
    }
}
