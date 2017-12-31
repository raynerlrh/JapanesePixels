using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DefaultAttack : PlayerSkill
{
    public bool b_needsUpdate { get; set; }
    public int numMoves { get; set; }
    public int cellsAffected { get; set; }
    public float damage { get; set; }
    public string skillName { get; set; }
    private GameObject bombRef;

    public DefaultAttack()
    {
        skillName = "Default Attack: Drop bomb";
        numMoves = 0;
        b_needsUpdate = false;
    }

    public void ExecuteSkill()
    {
        PlayerMoveController moveController = MyNetwork.instance.localPlayer.GetComponent<PlayerMoveController>();
        if (moveController.GetInventory.OnHandAmount <= 0)
            return;
        damage = moveController.GetPawn.GetComponent<CharacterStats>().attackVal;
        Transform playerTrans = moveController.GetPawn.gameObject.transform;
        Vector3Int playerPos = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(playerTrans.position);
        if (bombRef != null)
            if (GameModeManager.instance.gameGrid.GetWorldFlToCellPos(bombRef.transform.position) == playerPos)
                return;

        bool passThrough = true;
        Vector3 spawnPos = GameModeManager.instance.gameGrid.GetCellMiddleWPOS(playerPos);

        GameObject bomb = GameObject.Instantiate(moveController.GetInventory.OnHandItem, spawnPos, playerTrans.rotation);
        if (moveController.GetInventory.OnHandAmount > 0)
            moveController.GetInventory.OnHandAmount--;
        if (passThrough)
            bomb.GetComponent<Collider2D>().isTrigger = true;

        bomb.GetComponent<Bomb>().effectRange = moveController.GetInventory.OnHandRange;

        bomb.SetActive(true);
        bombRef = bomb;
        if (MyNetwork.instance.IsOnlineGame())
        {
            //bomb.AddComponent<SyncTransform>();
            //bomb.GetComponent<SyncTransform>().b_isEnemy = true;

            //Debug.Log("DEFAULT");

            //CmdSpawnBomb(bomb);
            //MyNetwork.instance.CmdSpawnObject(bomb);

            GameObject.Find("ItemSpawner").GetComponent<ItemSpawner>().CmdSpawnObject(bomb);
        }
    }

    //[Command]
    //void CmdSpawnBomb(GameObject _gameObject)
    //{
    //    NetworkServer.Spawn(_gameObject);
    //}

    public void Update()
    {
        if (!b_needsUpdate)
            return;
    }
}
