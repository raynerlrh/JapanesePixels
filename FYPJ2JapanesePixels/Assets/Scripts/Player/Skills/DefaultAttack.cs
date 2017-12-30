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

    public DefaultAttack()
    {
        skillName = "Default Attack: Drop bomb";
        numMoves = 0;
        b_needsUpdate = false;
    }

    public void ExecuteSkill()
    {
        PlayerMoveController moveController = MyNetwork.instance.localPlayer.GetComponent<PlayerMoveController>();

        damage = moveController.GetPawn.GetComponent<CharacterStats>().attackVal;
        Transform playerTrans = moveController.GetPawn.gameObject.transform;
        Vector3Int playerPos = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(playerTrans.position);
        Vector3 spawnPos = GameModeManager.instance.gameGrid.GetCellMiddleWPOS(playerPos);

        //GameObject bomb = GameObject.Instantiate(EnemyMoveController.instance.enemyPrefabs[2], spawnPos, playerTrans.rotation);

        //bool passThrough = true;
        //if (passThrough)
            //bomb.GetComponent<Collider2D>().isTrigger = true;

        if (MyNetwork.instance.IsOnlineGame())
        {
            //bomb.AddComponent<SyncTransform>();
            //bomb.GetComponent<SyncTransform>().b_isEnemy = true;

            SpawnBomb("Prefabs/Bomb", spawnPos, playerTrans.rotation);
        }
    }

    void SpawnBomb(string _prefabsPath, Vector3 _pos, Quaternion _rot)
    {
        PlayerMoveController moveController = MyNetwork.instance.localPlayer.GetComponent<PlayerMoveController>();

        if (moveController.isServer)
            moveController.RpcSpawn(_prefabsPath, _pos, _rot);
        else
            moveController.CmdSpawn(_prefabsPath, _pos, _rot);
    }

    public void Update()
    {
        if (!b_needsUpdate)
            return;
    }
}
