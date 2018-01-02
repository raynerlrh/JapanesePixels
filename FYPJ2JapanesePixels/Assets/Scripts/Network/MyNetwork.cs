using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

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

    void OnApplicationQuit()
    {
        if (!IsOnlineGame())
            return;

        NetworkManager networkManager = NetworkManager.singleton;

        MatchInfo matchInfo = networkManager.matchInfo;

        if (matchInfo == null)
            return;

        networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);
        networkManager.StopHost();

        PlayerPrefs.SetInt("Connection_Status", 1);

        Debug.Log("Application ending after " + Time.time + " seconds");
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log("Clean up after player " + player);
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        if (Network.isServer)
            Debug.Log("Local server connection disconnected");
        else
            if (info == NetworkDisconnection.LostConnection)
                Debug.Log("Lost connection to the server");
            else
                Debug.Log("Successfully diconnected from the server");
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        // here temporarily cos lazy change scene
        // proper place should be when player selects single player or multiplayer
        PlayerPrefs.SetString("Game_Mode", "Online_Coop");

        if (PlayerPrefs.GetString("Game_Mode") == "Single_Player")
        {
            b_foundLocalPlayer = true;
            gameType = GAME_TYPE.SINGLE_PLAYER;

            GameObject playerPrefab = Resources.Load("Prefabs/PlayerHero") as GameObject;
            localPlayer = GameObject.Instantiate(playerPrefab);
            Debug.Log("DDD");
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
            localPlayer.GetComponent<PlayerMoveController>().Start();

            if (localPlayer.GetComponent<PlayerMoveController>().GetPawn == null)
                Debug.Log("NULL");

            localPlayer.GetComponent<PlayerMoveController>().GetPawn.InitChar();
            localPlayer.GetComponent<PlayerMoveController>().SetSinglePlayerMode();

            // Don't need to destroy because NetworkManager is brought over from lobby, single player does not activate from lobby
            //Destroy(GetComponent<NetworkManagerHUD>());
            //Destroy(GetComponent<MyNetworkManager>());
        }
    }

    // Other players are always -1, only local player is 0
    // This function does not work
    // Don't think any scripts require the host anyway
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
        //print(b_foundLocalPlayer);
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
                localPlayer.GetComponent<PlayerMoveController>().GetPawn.InitChar();
                
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
