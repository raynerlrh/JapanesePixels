﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileRefManager : MonoBehaviour 
{
    public static TileRefManager instance = null;
    public Tile[] tileRefs;

    public enum TILE_TYPE
    {
        TILE_GRASS,
        TILE_WARNING,
    }

    public enum TILEMAP_TYPE
    {
        TILEMAP_ENVIRONMENT,
        TILEMAP_ENEMY,
        TILEMAP_PLAYER,
        TILEMAP_GRIDCELLS,
    }

    // Only for most foremost operations
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public Tile GetTileRef(TILE_TYPE tileType)
    {
        if (tileRefs[(int)tileType] != null)
            return tileRefs[(int)tileType];

        return tileRefs[0]; // if tile cannot be found return default tile
    }

    public void EraseTiles(Vector3Int[] tileList)
    {
        for (int i = 0; i < tileList.Length; ++i)
        {
            SetTile(TILEMAP_TYPE.TILEMAP_ENEMY, tileList[i], null);
        }
    }

    /// <summary>
    /// Gets tile at certain cell in specified tilemap
    /// </summary>
    /// <param name="map">Tilemap to place the tile</param>
    public void SetTile(TILEMAP_TYPE type, Vector3Int cellPos, Tile tile)
    {
        GetTilemap(type).SetTile(cellPos, tile);
    }

    public Tilemap GetTilemap(TILEMAP_TYPE type)
    {
        return GameModeManager.instance.gameGrid.getGrid.transform.GetChild((int)type).GetComponent<Tilemap>();
    }

    public Tile GetTileAtCellPos(TILEMAP_TYPE tileMapType, Vector3Int cellPos)
    {
        return (Tile)GetTilemap(tileMapType).GetTile(cellPos);
    }
}
