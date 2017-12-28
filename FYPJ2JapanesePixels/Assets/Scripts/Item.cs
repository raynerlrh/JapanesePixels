using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Item : NetworkBehaviour 
{
    public enum ITEM_TYPE
    {
        SKILL,
        QUESTION,
    }

    public ITEM_TYPE itemType;

    Vector3Int itemCellPos;
    NetworkIdentity player;

    void Start()
    {
        GameGrid gameGrid = GameObject.Find("Grid").GetComponent<GameGrid>();


        itemCellPos = gameGrid.GetWorldFlToCellPos(transform.localPosition);
    }

    void Update()
    {
        //if (!MyNetwork.instance.b_foundLocalPlayer)
        //    return;

        //if (MyNetwork.instance.localPlayer.GetComponent<PlayerMoveController>().playerCellPos == itemCellPos)
        //{
        //    UseItem();
        //}
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            player = col.GetComponent<NetworkIdentity>();
            UseItem();
        }
    }

    void UseItem()
    {
        switch (itemType)
        {
            case ITEM_TYPE.SKILL:
                // give a randomized skill? or already randomized at the start and just give a skill

                break;
            case ITEM_TYPE.QUESTION:
                // enables quiz menu
                EnableQuiz();
                break;
        }

        // Remove item
        GameObject.Destroy(gameObject);
    }

    void EnableQuiz()
    {
        if (MyNetwork.instance.IsOnlineGame())
        {
            if (!player.isLocalPlayer)
                return;
        }

        GameModeManager.instance.languageSystem.gameObject.SetActive(true);
        GameModeManager.instance.languageSystem.Enable();
    }
}
