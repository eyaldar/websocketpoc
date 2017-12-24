//using ScClient;
//using SuperSocket.ClientEngine;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace WebSocketsPOC.WebSockets
//{
//    public class SocketClusterClient : DisposableWebSocketClient
//    {
//        private Socket socket;
//        private List<(string Topic, Emitter.Listener Handler)> subscriptions;

//        public SocketClusterClient(string hostname, int port)
//        {
//            socket = new Socket(string.Format($"ws://{hostname}:{port}/"));
//            socket.setListerner(new SampleListener());
//            socket.setReconnectStrategy(new ReconnectStrategy().setMaxAttempts(3));

//            subscriptions = new List<(string Topic, Emitter.Listener Handler)>();
//        }

//        public override Task<bool> InitializeAsync()
//        {
//            socket.connect();
//            return Task.FromResult(true);
//        }

//        public override void Publish<TDataType>(string topic, TDataType data)
//        {
//            socket.publish(topic, data, OnPublishFinished);
//        }

//        public override Task SubscribeAsync(string topic, Action<object> handler)
//        {
//            var listener = new Emitter.Listener((name, data) =>
//            {
//                handler(data);
//            });
//            subscriptions.Add((topic, listener));
//            socket.on(topic, listener);

//            return Task.FromResult(0);
//        }

//        private void OnPublishFinished(string topic, object error, object data)
//        {
//            if (error != null)
//            {
//                Console.WriteLine($"Successfully published message {topic} with data: {data}");
//            }
//            else
//            {
//                Console.WriteLine($"Failed publishing message {topic} with data: {data}, got error: {error}");
//            }
//        }

//        public static IWebSocketClient Create(string hostname, int port)
//        {
//            return new SocketClusterClient(hostname, port);
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                foreach (var topicToHandler in subscriptions)
//                {
//                    socket.unsubscribe(topicToHandler.Topic);
//                }

//                subscriptions.Clear();

//                socket.disconnect();
//            }
//        }

//        private class SampleListener : BasicListener
//        {
//            public void onAuthentication(Socket socket, bool status)
//            {
//            }

//            public void onConnected(Socket socket)
//            {
//                Console.WriteLine($"Successfully connected");
//            }

//            public void onConnectError(Socket socket, ErrorEventArgs e)
//            {
//                Console.WriteLine($"Failed to connect, exception: {e.Exception.Message}");
//            }

//            public void onDisconnected(Socket socket)
//            {
//                Console.WriteLine($"Successfully disconnected");
//            }

//            public void onSetAuthToken(string token, Socket socket)
//            {
//            }
//        }
//    }
//}
