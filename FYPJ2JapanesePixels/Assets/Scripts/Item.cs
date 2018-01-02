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

    public enum EFFECT_TYPE
    {
        E_HEALTH,
        E_EXPLOSION,
        E_SPEEDINCREASE
    }

    public ITEM_TYPE itemType = ITEM_TYPE.SKILL;
    public EFFECT_TYPE effectType = EFFECT_TYPE.E_EXPLOSION;

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
        if (col.tag == "Player" && itemType == ITEM_TYPE.QUESTION)
        {
            player = col.GetComponent<NetworkIdentity>();
            UseItem();
            col.GetComponent<Inventory>().pendingReward = true;
        }
        else if(col.tag == "Player")
            UseItem();
        else if (col.gameObject.layer == 14)
        {
            UseItemCPU(col.gameObject);
        }
    }

    void UseItem()
    {
        switch (itemType)
        {
            case ITEM_TYPE.SKILL:
                switch (effectType)
                {
                    case EFFECT_TYPE.E_EXPLOSION:
                        Inventory inv = MyNetwork.instance.localPlayer.GetComponent<Inventory>();
                        inv.OnHandRange++;
                        inv.UpdateUI();
                        break;
                    case EFFECT_TYPE.E_HEALTH:
                        CharacterStats stats = MyNetwork.instance.localPlayer.GetComponent<CharacterStats>();
                        stats.increaseHealth(stats.hpSys.MAX_HEALTH);
                        break;
                }
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

    void UseItemCPU(GameObject go)
    {
        switch (itemType)
        {
            case ITEM_TYPE.SKILL:
                switch (effectType)
                {
                    case EFFECT_TYPE.E_EXPLOSION:
                        go.GetComponent<Inventory>().OnHandRange++;
                        break;
                    case EFFECT_TYPE.E_HEALTH:
                        go.GetComponent<CharacterStats>().increaseHealth(go.GetComponent<CharacterStats>().hpSys.MAX_HEALTH);
                        break;
                }
                // give a randomized skill? or already randomized at the start and just give a skill
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
