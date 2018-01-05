using System;
using System.Collections.Generic;
using WebSocketSharp;
using Newtonsoft.Json;

namespace MinecraftKnockOff
{
    // using Data = Dictionary<string, dynamic>;
    // using EventHandler = Action<Dictionary<string, dynamic>>;

	using Data = Dictionary<string, string>;
	using EventHandler = Action<Dictionary<string, string>>;

    public class WebSocketClient
    {
        // Properties

        private string Host = "localhost";
        private int Port = 8080;

        private Dictionary<string, EventHandler> Events = new Dictionary<string, EventHandler>();

        private static WebSocketClient instance = null;

        // Tools

        private WebSocket ws;

		
        // Constructors

        public WebSocketClient()
        {
            string url = CreateUrl();
            ws = new WebSocket(url);

            SetupSocket();

            instance = this;
        }

        public WebSocketClient(string host, int port)
        {
            Host = host;
            Port = port;

            string url = CreateUrl();
            ws = new WebSocket(url);

            SetupSocket();

            instance = this;
        }

        public static WebSocketClient getInstance()
        {
            return instance;
        }

        public void RegisterEventHandler(string name, EventHandler callback) {
            Events.Add(name, callback);
        }

        public void UnregisterEventHandler(string name) {
            Events.Remove(name);
        }

        public void Connect() {
            ws.Connect();
        }

        public void Close() {
            ws.Close();
        }

        public void Send(string data) {
            ws.Send(data);
        }

        // Private helpers

        private string CreateUrl() {
            return "ws://" + Host + ":" + Port;
        }

        private void SetupSocket() {
            ws.OnOpen += (sender, e) =>
            {   
                // Notify server
                Console.WriteLine("Connection established");
                Send("Connection established");
            };

            ws.OnMessage += (sender, e) =>
			{
				UnityMainThreadDispatcher.Instance().Enqueue(() => {
            		Console.WriteLine("Message received");
            		Console.WriteLine(e.Data);
            		TriggerEventHandler(e.Data);
				});
            };
        }

        private void TriggerEventHandler(string jsonData) {
            // Get data
            Data data = JsonConvert.DeserializeObject<Data>(jsonData);

            // Call the correct callback function
            string dataType = data["type"];

            if(Events.ContainsKey(dataType))
            {
                EventHandler callback = Events[dataType];
                callback(data);
            }
        }
    }
}
