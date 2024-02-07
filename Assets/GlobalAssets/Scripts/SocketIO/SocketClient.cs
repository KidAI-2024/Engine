using SocketIOClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient.Newtonsoft.Json;

namespace GlobalAssets.SocketIO
{
    public class SocketClient : MonoBehaviour
    {
        public static SocketClient Instance { get; private set; }

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
            });

            socket.JsonSerializer = new NewtonsoftJsonSerializer();

            socket.OnConnected += (sender, e) =>
            {
                Debug.Log("socket.OnConnected");
            };
            socket.On("result", (response) =>
            {
                // Handle the result here
                Debug.Log("Received result from server:");
                Debug.Log(response.GetValue<string>());
            });

            socket.Connect();
        }
        void Start()
        {

        }

        void Update()
        {
            // Emit a frame to the server
            if (Input.GetKeyDown(KeyCode.Space))
            {
                socket.Emit("frame", "Your frame data here");
            }
        }

        void OnDestroy()
        {
            socket.Disconnect();
        }
    }

}