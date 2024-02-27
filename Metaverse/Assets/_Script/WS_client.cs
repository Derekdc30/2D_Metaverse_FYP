using WebSocketSharp;
using UnityEngine;

public class WS_client : MonoBehaviour
{
    WebSocket ws;
    void Start()
    {
        ws = new WebSocket("ws://localhost:3000");
        ws.OnMessage += (sender, e)=>{
            Debug.Log("Message received from "+((WebSocket)sender).Url+", Data : "+e.Data);
        };
        ws.Connect();
    }

    void Update()
    {
        if(ws==null){
            return;
        }
        if(Input.GetKeyDown(KeyCode.T)){
            ws.Send("Hello");
        }
    }
}
