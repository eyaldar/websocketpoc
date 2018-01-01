using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketsPOC.WebSockets
{
    public abstract class DisposableWebSocketClient : IWebSocketClient
    {
        public abstract Task<bool> InitializeAsync();

        public abstract void Publish<TDataType>(string topic, TDataType data);

        public abstract Task SubscribeAsync(string topic, Action<object> handler);

        public abstract JObject ExtractMessage(object data);

        #region Disposable Pattern

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);

        ~DisposableWebSocketClient()
        {
            Dispose(false);
        }

        #endregion
    }
}
