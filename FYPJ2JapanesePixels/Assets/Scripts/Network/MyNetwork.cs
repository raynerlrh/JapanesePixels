using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetwork : MonoBehaviour 
{
    public static MyNetwork instance = null;

    public GameObject localPlayer { get; set; }
    public GameGrid localGameGrid { get; set; }

    public bool b_foundLocalPlayer { get; set; }

    public enum GAME_TYPE
    {
        SINGLE_PLAYER,
        ONLINE_VS,
        ONINE_COOP,
    }

    public GAME_TYPE gameType { get; set; }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        // here temporarily cos lazy change scene
        PlayerPrefs.SetString("Game_Mode", "Single_Player");

        if (PlayerPrefs.GetString("Game_Mode") == "Single_Player")
        {
            gameType = GAME_TYPE.SINGLE_PLAYER;
        }
        else if (PlayerPrefs.GetString("Game_Mode") == "Online_Versus")
        {
            gameType = GAME_TYPE.ONLINE_VS;
        }
        else if (PlayerPrefs.GetString("Game_Mode") == "Online_Coop")
        {
            gameType = GAME_TYPE.ONINE_COOP;
        }
    }

    void Start()
    {
        if (PlayerPrefs.GetString("Game_Mode") == "Single_Player")
        {
            localPlayer = GameObject.Instantiate(GetComponent<NetworkManager>().playerPrefab);
            localPlayer.GetComponent<PlayerMoveController>().SetSinglePlayerMode();
            Destroy(GetComponent<NetworkManagerHUD>());
            Destroy(GetComponent<NetworkManager>());

            gameType = GAME_TYPE.SINGLE_PLAYER;
        }
    }

    void Update()
    {
        if (gameType == GAME_TYPE.SINGLE_PLAYER)
            return;

        if (b_foundLocalPlayer)
            return;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                localPlayer = players[i];
                localGameGrid = GameObject.Find("Grid").GetComponent<GameGrid>();
                b_foundLocalPlayer = true;
                return;
            }
        }
    }

    public bool IsOnlineGame()
    {
        return !(gameType == GAME_TYPE.SINGLE_PLAYER);
    }
}
