﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : ObjectStats {
    TimerRoutine explode_time;
    bool hasExploded;
    bool hasMoved;
    public int effectRange = 4;
    Vector3Int startPos;
    Vector3Int[] effectPos;
	// Use this for initialization
	void Start () {
        explode_time = gameObject.AddComponent<TimerRoutine>();
        explode_time.initTimer(3);
        explode_time.executedFunction = Explode;
        hasExploded = false;
        effectPos = new Vector3Int[effectRange * 4 + 1];
        startPos = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.position);
	}
	
	// Update is called once per frame
	void Update () {
        if (hasExploded == false)
        {
            explode_time.executeFunction();
            hasExploded = true;
        }
	}

    void Explode()
    {
        Destroy(this.gameObject);
        spawnFlames();
    }

    void spawnFlames()
    {
        for (int x = 0; x <= effectRange; ++x)
        {
            Vector3Int spawnCell = Vector3Int.zero;
            spawnCell.Set(startPos.x + x, startPos.y, startPos.z);
            if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, spawnCell))
            {
                //Debug.Log(x);
                break;
            }
            //if (tileobj.gameObject.layer == 13)
            //{

            //}
            Vector3 spawnPos = GameModeManager.instance.gameGrid.GetCellMiddleWPOS(spawnCell);
            GameObject bomb = GameObject.Instantiate(EnemyMoveController.instance.enemyPrefabs[3], spawnPos, transform.localRotation);
            bomb.SetActive(true);
        }

        for (int x = -1; x >= -effectRange; --x)
        {
            Vector3Int spawnCell = Vector3Int.zero;
            spawnCell.Set(startPos.x + x, startPos.y, startPos.z);
            if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, spawnCell))
            {
                break;
            }
            Vector3 spawnPos = GameModeManager.instance.gameGrid.GetCellMiddleWPOS(spawnCell);
            GameObject bomb = GameObject.Instantiate(EnemyMoveController.instance.enemyPrefabs[3], spawnPos, transform.localRotation);
            bomb.SetActive(true);
        }

        for (int y = 0; y <= effectRange; ++y)
        {
            Vector3Int spawnCell = Vector3Int.zero;
            spawnCell.Set(startPos.x, startPos.y + y, startPos.z);
            if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, spawnCell))
            {
                break;
            }
            Vector3 spawnPos = GameModeManager.instance.gameGrid.GetCellMiddleWPOS(spawnCell);
            GameObject bomb = GameObject.Instantiate(EnemyMoveController.instance.enemyPrefabs[3], spawnPos, transform.localRotation);
            bomb.SetActive(true);
        }

        for (int y = -1; y >= -effectRange; --y)
        {
            Vector3Int spawnCell = Vector3Int.zero;
            spawnCell.Set(startPos.x, startPos.y + y, startPos.z);
            if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, spawnCell))
                break;
            Vector3 spawnPos = GameModeManager.instance.gameGrid.GetCellMiddleWPOS(spawnCell);
            GameObject bomb = GameObject.Instantiate(EnemyMoveController.instance.enemyPrefabs[3], spawnPos, transform.localRotation);
            bomb.SetActive(true);
        }
    }

    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.gameObject.layer == 10 && hasMoved)
            GetComponent<Collider2D>().isTrigger = false;
        else
            hasMoved = false;
    }

    void OnTriggerExit2D(Collider2D obj)
    {
        hasMoved = true;
    }
}
