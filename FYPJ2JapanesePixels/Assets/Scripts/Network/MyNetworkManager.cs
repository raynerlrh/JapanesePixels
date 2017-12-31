using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkManager : NetworkManager 
{
    // called when disconnected from a server
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        PlayerPrefs.SetInt("Connection_Status", 0);

        StopClient();
    }
}
