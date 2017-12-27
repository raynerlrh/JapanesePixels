using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A simple horizontal shuriken throw
public class ShurikenAttack : PlayerSkill 
{
    public bool b_needsUpdate { get; set; }
    public int numMoves { get; set; }
    public int cellsAffected { get; set; }
    public float damage { get; set; }
    public string skillName { get; set; }

    GameObject shurikenPrefab;
    //GameObject[] shurikenClone;
    List<GameObject> shurikenClone;

    int totalAttackDist;
    int startingPos;
    float shurikenSpeed = 3f;

    public ShurikenAttack()
    {
        skillName = "Shuriken Attack";
        numMoves = 1;
        damage = 10;

        shurikenClone = new List<GameObject>();
        shurikenPrefab = (GameObject)Resources.Load("Prefabs/Shuriken", typeof(GameObject));
        shurikenPrefab.GetComponent<ObjectStats>().damage = damage;
        cellsAffected = 3;
    }

    public void ExecuteSkill()
    {
        Debug.Log("Executed Skill: " + skillName);

        b_needsUpdate = true;

        PlayerMoveController moveController = MyNetwork.instance.localPlayer.GetComponent<PlayerMoveController>();

        Vector2 playerPos = GameModeManager.instance.gameGrid.GetCellToWorld(moveController.GetPlayerCellPos);
        startingPos = (int)playerPos.x;
        totalAttackDist = startingPos + cellsAffected;

        InstantiateInactiveShuriken(playerPos);
    }

    public void Update()
    {
        if (!b_needsUpdate)
            return;

        UpdateShurikens();
    }

    void UpdateShurikens()
    {
        if (NoShurikensActive())
            b_needsUpdate = false;

        for (int i = 0; i < shurikenClone.Count; i++)
        {
            if (shurikenClone[i] == null)
                continue;
            
            shurikenClone[i].transform.position += Vector3.right * shurikenSpeed *  Time.deltaTime;

            Vector3Int shurikenCellPos = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(shurikenClone[i].transform.position);

            if (shurikenCellPos.x >= totalAttackDist)
                GameObject.Destroy(shurikenClone[i]);
        }
    }

    void InstantiateInactiveShuriken(Vector2 playerPos)
    {
        GameObject _shuriken = GameObject.Instantiate(shurikenPrefab, playerPos, Quaternion.identity);

        shurikenClone.Add(_shuriken);
    }

    bool NoShurikensActive()
    {
        for (int i = 0; i < shurikenClone.Count; i++)
        {
            if (shurikenClone[i] != null)
                return false;
        }

        return true;
    }
}
