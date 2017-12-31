using UnityEngine;
using UnityEngine.UI;

public class ConnectionStatus : MonoBehaviour 
{
    void Start()
    {
        int connectionStatus;

        if (PlayerPrefs.HasKey("Connection_Status"))
            connectionStatus = PlayerPrefs.GetInt("Connection_Status");
        else
            connectionStatus = 1;

        if (connectionStatus == 0)
            GetComponent<Text>().text = "Connection was dropped";
        else
            GetComponent<Text>().text = "";
    }
}
