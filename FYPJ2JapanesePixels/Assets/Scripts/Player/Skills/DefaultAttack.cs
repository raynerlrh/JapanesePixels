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

    private Vector3 prevBombPos = Vector3.zero;

    float timeTillReset;

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

        if (prevBombPos != Vector3.zero)
        {
            if (GameModeManager.instance.gameGrid.GetWorldFlToCellPos(prevBombPos) == playerPos)
                return;
        }
        
        Vector3 spawnPos = GameModeManager.instance.gameGrid.GetCellMiddleWPOS(playerPos);

        if (moveController.GetInventory.OnHandAmount > 0)
            moveController.GetInventory.OnHandAmount--;
        moveController.GetInventory.UpdateUI();

        if (MyNetwork.instance.IsOnlineGame())
        {
            prevBombPos = spawnPos;
            SpawnBomb("Prefabs/Bomb", spawnPos, playerTrans.rotation);
        }
        else
        {
            
            GameObject bombPrefab = Resources.Load("Prefabs/Bomb") as GameObject;
            GameObject _bomb = GameObject.Instantiate(bombPrefab, spawnPos, playerTrans.rotation);
            _bomb.GetComponent<Bomb>().effectRange = moveController.GetInventory.OnHandRange;
        }

        b_needsUpdate = true;
    }

    void SpawnBomb(string _prefabsPath, Vector3 _pos, Quaternion _rot)
    {
        PlayerMoveController moveController = MyNetwork.instance.localPlayer.GetComponent<PlayerMoveController>();

        GameObject spawn = null;

        int bombRange = moveController.GetInventory.OnHandRange;

        if (moveController.isServer)
            moveController.RpcSpawn(_prefabsPath, _pos, _rot, bombRange, false, 100);
        else
            moveController.CmdSpawn(_prefabsPath, _pos, _rot, bombRange, false, 100);

        if (spawn)
            spawn.GetComponent<Bomb>().effectRange = moveController.GetInventory.OnHandRange;
    }

    public void Update()
    {
        if (!b_needsUpdate)
            return;

        timeTillReset += Time.deltaTime;

        if (timeTillReset > 3f)
        {
            prevBombPos = Vector3.zero;
            timeTillReset = 0f;
            b_needsUpdate = false;
        }
    }
}
