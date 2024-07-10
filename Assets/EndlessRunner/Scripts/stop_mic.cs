using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stop_mic : MonoBehaviour
{

    private GlobalAssets.Socket.SocketUDP socketClient;
    // Start is called before the first frame update
    void Start()
    {
        socketClient = GlobalAssets.Socket.SocketUDP.Instance;
        Dictionary<string, string> message = new Dictionary<string, string>
        {

            { "event", "stop_prediction" }
        };

        // Send the message to the server
        socketClient.SendMessage(message);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
