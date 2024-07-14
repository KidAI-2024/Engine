using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoingBack : MonoBehaviour
{
    // Start is called before the first frame update
    private GlobalAssets.Socket.SocketUDP socketClient; // Socket client for communication
    void Start()
    {
    socketClient = GlobalAssets.Socket.SocketUDP.Instance;
        
    }

    // Update is called once per frame
  
}
