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
        // chunk size for sending messages
        private int chunkSize = 60 * 1024;
        private byte[] receiveBuffer;
        private int receiveBufferSize = 2 * 1024 * 1024; // 2MB
        private int sendBufferSize = 2 * 1024 * 1024; // 2MB
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
            remoteEP = new IPEndPoint(IPAddress.Loopback, port);
            udp = new UdpClient();
            // set receive buffer size to 2MB
            udp.Client.ReceiveBufferSize = receiveBufferSize;
            // set send buffer size to 2MB
            udp.Client.SendBufferSize = sendBufferSize;
            Debug.Log("UDP client created");
            Debug.Log("Receive buffer size: " + udp.Client.ReceiveBufferSize);
            Debug.Log("Send buffer size: " + udp.Client.SendBufferSize);

        }

        public void SendMessage(Dictionary<string, string> message)
        {
            byte[] messageBytes = DictToBytes(message);
            SendMessageBytesInChuncks(messageBytes);
        }
        public void SendMessageSync(Dictionary<string, string> message)
        {
            byte[] messageBytes = DictToBytes(message);
            SendMessageBytesInChuncksSync(messageBytes);
        }
        private byte[] DictToBytes(Dictionary<string, string> message)
        {
            string message_string = SerializeDictionary(message);
            // Debug.Log("Sending: " + message_string);
            return System.Text.Encoding.UTF8.GetBytes(message_string);
        }
        private void SendMessageBytesInChuncks(byte[] messageBytes)
        {
            // Debug.Log(System.Text.Encoding.UTF8.GetString(messageBytes));
            for (int i = 0; i < messageBytes.Length; i += chunkSize)
            {
                // Debug.Log("Sending chunk " + i / chunkSize);
                byte[] chunk = new byte[Mathf.Min(chunkSize, messageBytes.Length - i)];
                Array.Copy(messageBytes, i, chunk, 0, chunk.Length);
                SendMessageBytesCompletely(chunk);
            }
        }
        private void SendMessageBytesInChuncksSync(byte[] messageBytes)
        {
            // Debug.Log(System.Text.Encoding.UTF8.GetString(messageBytes));
            for (int i = 0; i < messageBytes.Length; i += chunkSize)
            {
                // Debug.Log("Sending chunk " + i / chunkSize);
                byte[] chunk = new byte[Mathf.Min(chunkSize, messageBytes.Length - i)];
                Array.Copy(messageBytes, i, chunk, 0, chunk.Length);
                SendMessageBytesCompletelySync(chunk);
            }
        }
        private void SendMessageBytesCompletely(byte[] messageBytes)
        {
            udp.SendAsync(messageBytes, messageBytes.Length, remoteEP);
        }
        private void SendMessageBytesCompletelySync(byte[] messageBytes)
        {
            udp.Send(messageBytes, messageBytes.Length, remoteEP);
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
            SendMessageSync(message);
            udp.Close();
            remoteEP = null;
        }
    }
}