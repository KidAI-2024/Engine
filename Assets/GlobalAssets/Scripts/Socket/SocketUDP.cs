using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Collections.Generic;
// using System.Linq;
using Newtonsoft.Json;
namespace GlobalAssets.Socket
{

    public class SocketUDP : MonoBehaviour
    {
        // Singleton pattern
        public static SocketUDP Instance { get; private set; }

        // UDP client
        private UdpClient udp;
        private IPEndPoint remoteEP;
        private string host = "localhost";
        private int port = 5065;
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
            udp = new UdpClient();
            remoteEP = new IPEndPoint(IPAddress.Any, 0);
        }
        private void SendMessageBytes(byte[] messageBytes)
        {
            // Debug.Log(System.Text.Encoding.UTF8.GetString(messageBytes));
            int chunkSize = 60000; // Size of each chunk in bytes
            for (int i = 0; i < messageBytes.Length; i += chunkSize)
            {
                // Debug.Log("messageBytes.Length: " + messageBytes.Length);
                // Debug.Log("Sending chunk " + i / chunkSize);
                byte[] chunk = new byte[Mathf.Min(chunkSize, messageBytes.Length - i)];
                Array.Copy(messageBytes, i, chunk, 0, chunk.Length);
                udp.Send(chunk, chunk.Length, host, port);
            }
        }
        public void SendMessage(Dictionary<string, string> message)
        {
            string message_string = SerializeDictionary(message);
            // Debug.Log("Sending: " + message_string);
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message_string);
            SendMessageBytes(messageBytes);
        }
        private string SerializeDictionary(Dictionary<string, string> message)
        {
            return JsonConvert.SerializeObject(message);
        }
        public byte[] ReceiveData()
        {
            byte[] data = udp.Receive(ref remoteEP);
            return data;
        }
        public bool isDataAvailable()
        {
            return udp.Available > 0;
        }
        void OnApplicationQuit()
        {
            udp.Close();
            remoteEP = null;
        }
    }
}