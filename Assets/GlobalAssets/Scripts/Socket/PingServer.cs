using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalAssets.Socket;
using System;


namespace GlobalAssets.Socket
{
    public class PingServer : MonoBehaviour
    {
        public GameObject warningPanel;
        private SocketUDP socketClient;
        void Start()
        {
            socketClient = SocketUDP.Instance;
            Ping();   
        }

        void Update()
        {
            try
            {
                if (socketClient.isDataAvailable())
                {
                    Debug.Log("Server Alive");
                    Dictionary<string, string> response = socketClient.ReceiveDictMessage();
                    warningPanel.SetActive(false);

                }
            }
            catch (Exception e)
            {
                Debug.Log("Server is not available");
                warningPanel.SetActive(true);
            }
        }
        public void Ping()
        {
            Debug.Log("Sending Ping request to server");
            Dictionary<string, string> message = new Dictionary<string, string>
            {
                { "event", "ping" }
            };
            socketClient.SendMessage(message);
        }
    }

}