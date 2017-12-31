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

        if (prevBombPos != Vector3.zero)
        {
            if (GameModeManager.instance.gameGrid.GetWorldFlToCellPos(prevBombPos) == playerPos)
                return;
        }
        
        Vector3 spawnPos = GameModeManager.instance.gameGrid.GetCellMiddleWPOS(playerPos);

        if (moveController.GetInventory.OnHandAmount > 0)
            moveController.GetInventory.OnHandAmount--;

        if (MyNetwork.instance.IsOnlineGame())
        {
            prevBombPos = spawnPos;
            SpawnBomb("Prefabs/Bomb", spawnPos, playerTrans.rotation);
        }
        else
        {
            GameObject _bomb = Resources.Load("Prefabs/Bomb") as GameObject;
            GameObject.Instantiate(_bomb, spawnPos, playerTrans.rotation);
        }
    }

    void SpawnBomb(string _prefabsPath, Vector3 _pos, Quaternion _rot)
    {
        PlayerMoveController moveController = MyNetwork.instance.localPlayer.GetComponent<PlayerMoveController>();

        GameObject spawned = null;
        if (moveController.isServer)
            moveController.RpcSpawn(_prefabsPath, _pos, _rot);
        else
            moveController.CmdSpawn(_prefabsPath, _pos, _rot);
        if (spawned)
            spawned.GetComponent<Bomb>().effectRange = moveController.GetInventory.OnHandRange;
    }

    public void Update()
    {
        if (!b_needsUpdate)
            return;
    }
}
