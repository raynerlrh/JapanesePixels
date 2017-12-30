using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetwork : MonoBehaviour 
{
    public static MyNetwork instance = null;

    public GameObject gameHost { get; set; }
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
        PlayerPrefs.SetString("Game_Mode", "Online_Versus");

        if (PlayerPrefs.GetString("Game_Mode") == "Single_Player")
        {
            //b_foundLocalPlayer = true;
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
        }
    }

    void FindHost(GameObject[] _players)
    {
        for (int i = 0; i < _players.Length; i++)
        {
            if (_players[i].GetComponent<NetworkIdentity>().playerControllerId == -1)
            {
                gameHost = _players[i];
                Debug.Log("FOUND HOST");
                return;
            }
        }
    }

    void Update()
    {
        if (gameType == GAME_TYPE.SINGLE_PLAYER)
            return;
       
        if (b_foundLocalPlayer)
            return;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        FindHost(players);

        for (int i = 0; i < players.Length; i++)
        {
            NetworkIdentity _player = players[i].GetComponent<NetworkIdentity>();

            if (_player.isLocalPlayer)
            {
                if (_player.isServer)
                    _player.GetComponent<PlayerMoveController>().b_isHost = true;

                localPlayer = _player.gameObject;

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
