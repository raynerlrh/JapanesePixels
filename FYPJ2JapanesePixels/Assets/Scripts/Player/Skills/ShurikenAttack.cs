using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A simple horizontal shuriken throw
public class ShurikenAttack : PlayerSkill 
{
    public bool b_needsUpdate { get; set; }
    public string skillName { get; set; }

    GameObject shurikenPrefab;
    GameObject[] shurikenClone;

    int attackCellDist = 3;
    int totalAttackDist;
    int startingPos;

    public ShurikenAttack()
    {
        skillName = "Shuriken Attack";
        shurikenPrefab = (GameObject)Resources.Load("Prefabs/Shuriken", typeof(GameObject));
        shurikenClone = new GameObject[PlayerMoveController.MAX_MOVES];
    }

    public void ExecuteSkill()
    {
        Debug.Log("Executed Skill: " + skillName);

        b_needsUpdate = true;

        Vector2 playerPos = GameModeManager.instance.gameGrid.GetCellToWorld(PlayerMoveController.instance.GetPlayerCellPos);
        startingPos = (int)playerPos.x;
        totalAttackDist = startingPos + attackCellDist;

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

        for (int i = 0; i < shurikenClone.Length; i++)
        {
            if (shurikenClone[i] == null)
                continue;

            shurikenClone[i].transform.position += Vector3.right * Time.deltaTime;

            Vector3Int shurikenCellPos = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(shurikenClone[i].transform.position);

            if (shurikenCellPos.x >= totalAttackDist)
                GameObject.Destroy(shurikenClone[i]);
        }
    }

    void InstantiateInactiveShuriken(Vector2 playerPos)
    {
        for (int i = 0; i < shurikenClone.Length; i++)
        {
            if (shurikenClone[i] == null)
            {
                shurikenClone[i] = GameObject.Instantiate(shurikenPrefab, playerPos, Quaternion.identity);
                return;
            }
        }
    }

    bool NoShurikensActive()
    {
        for (int i = 0; i < shurikenClone.Length; i++)
        {
            if (shurikenClone[i] != null)
                return false;
        }

        return true;
    }
}
