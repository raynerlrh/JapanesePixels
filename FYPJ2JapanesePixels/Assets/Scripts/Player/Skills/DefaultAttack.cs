﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultAttack : PlayerSkill {

    public bool b_needsUpdate { get; set; }
    public int numMoves { get; set; }
    public int cellsAffected { get; set; }
    public float damage { get; set; }
    public string skillName { get; set; }

    public DefaultAttack()
    {
        skillName = "Default Attack: Drop bomb";
        numMoves = 1;
        b_needsUpdate = false;
    }

    public void ExecuteSkill()
    {
        damage = PlayerMoveController.instance.GetPawn.GetComponent<CharacterStats>().attackVal;
        Transform playerTrans = PlayerMoveController.instance.GetPawn.gameObject.transform;
        Vector3Int playerPos = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(playerTrans.position);
        bool passThrough = true;
        Vector3 spawnPos = GameModeManager.instance.gameGrid.GetCellMiddleWPOS(playerPos);
        GameObject bomb = GameObject.Instantiate(EnemyMoveController.instance.enemyPrefabs[2], spawnPos, playerTrans.rotation);
        if (passThrough)
            bomb.GetComponent<Collider2D>().isTrigger = true;
        bomb.SetActive(true);
    }

    public void Update()
    {
        if (!b_needsUpdate)
            return;
    }
}
