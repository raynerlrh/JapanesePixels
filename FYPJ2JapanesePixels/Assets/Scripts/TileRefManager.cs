using System.Collections;
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
}
