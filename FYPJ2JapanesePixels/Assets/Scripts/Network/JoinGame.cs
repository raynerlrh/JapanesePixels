using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class JoinGame : MonoBehaviour 
{
    private NetworkManager networkManager;

    List<GameObject> roomList = new List<GameObject>();

    [SerializeField]
    Text status;

    [SerializeField]
    GameObject roomListItemPrefab;

    [SerializeField]
    Transform roomListParent;

    void Start()
    {
        networkManager = NetworkManager.singleton;

        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        RefreshRoomList();
    }

    public void RefreshRoomList()
    {
        ClearRoomList();
        networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
        status.text = "Loading...";
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)﻿
    {
        status.text = "";

        if (!success || matches == null)
        {
            status.text = "Cannot find rooms";
            return;
        }

        for (int i = 0; i < matches.Count; i++)
        {
            GameObject roomListItemGO = Instantiate(roomListItemPrefab);
            roomListItemGO.transform.SetParent(roomListParent);
            roomListItemGO.transform.localScale = new Vector3(1, 1, 1);

            RoomListItem roomListItem = roomListItemGO.GetComponent<RoomListItem>();

            if (roomListItem != null)
            {
                roomListItem.Setup(matches[i], JoinRoom);
            }

            roomList.Add(roomListItemGO);
        }

        if (roomList.Count == 0)
        {
            status.text = "No rooms found";
        }
    }

    void ClearRoomList()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }

        roomList.Clear();
    }

    public void JoinRoom(MatchInfoSnapshot _match)
    {
        PlayerPrefs.SetInt("Connection_Status", 1);

        networkManager.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        ClearRoomList();
        status.text = "Joining...";
    }
}
