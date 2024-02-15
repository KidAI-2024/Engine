using SocketIOClient;
using System;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient.Newtonsoft.Json;

namespace GlobalAssets.SocketIO
{
    public class SocketClient : MonoBehaviour
    {
        // Singleton pattern
        public static SocketClient Instance { get; private set; }
        // The socket
        private SocketIOUnity socket;

        void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            var uri = new Uri("http://localhost:5000");
            socket = new SocketIOUnity(uri, new SocketIOOptions
            {
                Query = new Dictionary<string, string> { { "token", "UNITY" } },
                Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
            })
            {
                JsonSerializer = new NewtonsoftJsonSerializer()
            };
            socket.OnConnected += (sender, e) =>
            {
                Debug.Log("Socket Connected Successfully");
            };
            socket.Connect();
        }
        void OnDestroy()
        {
            // Disconnect the socket
            socket.Disconnect();
        }
        public void Emit(string eventName, object data)
        {
            socket.Emit(eventName, data);
        }
        public void OnUnityThread(string eventName, Action<SocketIOResponse> callback)
        {
            socket.OnUnityThread(eventName, callback);
        }
        public void On(string eventName, Action<SocketIOResponse> callback)
        {
            socket.On(eventName, callback);
        }
    }
}