using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Collections.Generic;
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
        private int chunkSize = 60000; // Size of each chunk in bytes
        private byte[] receiveBuffer;
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
            // set receive buffer size to 2MB
            udp.Client.ReceiveBufferSize = 2 * 1024 * 1024;
        }

        public void SendMessage(Dictionary<string, string> message)
        {
            string message_string = SerializeDictionary(message);
            // Debug.Log("Sending: " + message_string);
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message_string);
            SendMessageBytes(messageBytes);
        }
        private void SendMessageBytes(byte[] messageBytes)
        {
            // Debug.Log(System.Text.Encoding.UTF8.GetString(messageBytes));
            // Debug.Log("messageBytes.Length: " + messageBytes.Length);
            for (int i = 0; i < messageBytes.Length; i += chunkSize)
            {
                // Debug.Log("Sending chunk " + i / chunkSize);
                byte[] chunk = new byte[Mathf.Min(chunkSize, messageBytes.Length - i)];
                Array.Copy(messageBytes, i, chunk, 0, chunk.Length);
                udp.Send(chunk, chunk.Length, host, port);
            }
        }
        [Obsolete("ReceiveMessage is deprecated, please use ReceiveDictMessage instead.")]
        public string ReceiveMessage()
        {
            byte[] data = ReceiveCompleteMessageBytes();
            string message = ReceiveStringMessage();
            // try to convert the message to a dictionary
            try
            {
                Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
                return dict["prediction"];
            }
            catch (Exception)
            {
                return message;
            }
        }
        public Dictionary<string, string> ReceiveDictMessage()
        {
            string message = ReceiveStringMessage();
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
        }
        private string ReceiveStringMessage()
        {
            byte[] data = ReceiveCompleteMessageBytes();
            return System.Text.Encoding.UTF8.GetString(data);
        }
        public bool isDataAvailable()
        {
            return udp.Available > 0;
        }

        private string SerializeDictionary(Dictionary<string, string> message)
        {
            return JsonConvert.SerializeObject(message);
        }
        private byte[] ReceiveCompleteMessageBytes()
        {
            byte[] data = udp.Receive(ref remoteEP);
            return data;
        }
        void OnApplicationQuit()
        {
            Dictionary<string, string> message = new Dictionary<string, string>
                {
                    { "event", "stop_feed_hand_pose" }
                };
            SendMessage(message);
            udp.Close();
            remoteEP = null;
        }
    }
}