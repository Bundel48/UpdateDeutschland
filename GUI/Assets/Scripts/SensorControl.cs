using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WatsonWebsocket;
using System;
using System.Text;

public class SensorControl : MonoBehaviour
{
    WatsonWsClient client;
    // Start is called before the first frame update

    public bool pressedUp = false;
    public bool pressedDown = false;
    public bool pressedOk = false;

    void Start()
    {
        client = new WatsonWsClient("localhost", 8818, false);
        client.ServerConnected += ServerConnected;
        client.ServerDisconnected += ServerDisconnected;
        client.MessageReceived += MessageReceived;

        client.Start();
    }

    void MessageReceived(object sender, MessageReceivedEventArgs args) 
    {
        Debug.Log("Message from server: " + Encoding.UTF8.GetString(args.Data));
        
        if (Encoding.UTF8.GetString(args.Data) == "UP") {
            this.pressedUp = true;
        } else if (Encoding.UTF8.GetString(args.Data) == "DOWN") {
            this.pressedDown = true;
        } else if (Encoding.UTF8.GetString(args.Data) == "OK") {
            this.pressedOk = true;
        }

    }

    void ServerConnected(object sender, EventArgs args) 
    {
        Debug.Log("Server connected");
    }

    void ServerDisconnected(object sender, EventArgs args) 
    {
        Debug.Log("Server disconnected");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy() {
        client.Stop();
    }
}
