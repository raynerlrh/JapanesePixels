using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileRefManager : MonoBehaviour {
    public static TileRefManager instance = null;
    public Tile[] tileRefs;

    // Only for most foremost operations
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public Tile GetTileRef(int index)
    {
        if (tileRefs[index] != null)
            return tileRefs[index];
        return tileRefs[0]; // if tile cannot be found return default tile
    }
}
