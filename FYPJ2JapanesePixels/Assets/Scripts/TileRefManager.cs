using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TileRefManager : MonoBehaviour
{
    public static TileRefManager instance = null;
    public Tile[] tileRefs;
    List<PNode> walklist;
    public Dictionary<Vector3Int, GameObject> tile_dict;
    public int destructCount;

    public enum TILE_TYPE
    {
        TILE_GRASS,
        TILE_WARNING,
        TILE_WALL,
    }

    public enum TILEMAP_TYPE
    {
        TILEMAP_ENVIRONMENT,
        TILEMAP_ENEMY,
        TILEMAP_PLAYER,
        TILEMAP_GRIDCELLS,
        TILEMAP_SOLIDWALL,
        TILEMAP_DESTRUCTIBLE
    }

    // Only for most foremost operations
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Turn off player movement grid at the start
        //GetTilemap(TileRefManager.TILEMAP_TYPE.TILEMAP_GRIDCELLS).gameObject.SetActive(false);
        walklist = new List<PNode>();
        tile_dict = new Dictionary<Vector3Int,GameObject>();
        TileBase[] tileBase;
        Tilemap map = GetTilemap(TileRefManager.TILEMAP_TYPE.TILEMAP_ENVIRONMENT);
        map.CompressBounds();
        BoundsInt bound = map.cellBounds;
        tileBase = map.GetTilesBlock(bound);
        int sizehalf = map.size.x / 2;
        int sizehalfy = map.size.y / 2;
        if (map.size.y % 2 != 0) // if tilemap size y is odd, it needs to be offset by 1 row
            sizehalfy += 1;
        for (int x = 0; x < bound.size.x; x++)
        {
            for (int y = 0; y < bound.size.y; y++)
            {
                TileBase tile = tileBase[x + y * bound.size.x];
                if (tile != null)
                {
                    Vector3Int cell = new Vector3Int(x - sizehalf, y - sizehalfy, 0);
                    //TileRefManager.instance.SetTile(TileRefManager.TILEMAP_TYPE.TILEMAP_ENEMY, cell, TileRefManager.instance.GetTileRef(TileRefManager.TILE_TYPE.TILE_GRASS));
                    walklist.Add(new PNode(cell, false));
                }
            }
        }
        map = GetTilemap(TileRefManager.TILEMAP_TYPE.TILEMAP_DESTRUCTIBLE);
        map.CompressBounds();
        bound = map.cellBounds;
        tileBase = map.GetTilesBlock(bound);
        destructCount = tileBase.Length;
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
        return transform.GetChild((int)type).GetComponent<Tilemap>();
    }

    public Tile GetTileAtCellPos(TILEMAP_TYPE tileMapType, Vector3Int cellPos)
    {
        return (Tile)GetTilemap(tileMapType).GetTile(cellPos);
    }

    public int GetNodeIndex(Vector3Int pos)
    {
        for (int i = 0; i < walklist.Count; ++i)
        {
            if (walklist[i].pos == pos)
            {
                return i;
            }
        }
        return -1;
    }

    public PNode GetNode(int key)
    {
        return walklist[key];
    }

    public void plantLanguageTile(int min, int howMany, QuestionData[] data)
    {
        int rint = 0;
        int times = 0;
        if (howMany > destructCount || min > destructCount)
            return;
        while (times < howMany)
        {
            rint = Random.Range(0, walklist.Count);
            Vector3Int randpos = walklist[rint].pos;
            if (GetTileAtCellPos(TILEMAP_TYPE.TILEMAP_DESTRUCTIBLE, randpos) != null)
            {
                if (times < min)
                {
                    if (tile_dict.ContainsKey(randpos))
                        continue;
                    ++times;
                    Vector3 f = GameModeManager.instance.gameGrid.GetCellMiddleWPOS(randpos);
                    f.Set(f.x, f.y + 0.4f, f.z);
                    Text txt = Instantiate(GameModeManager.instance.canvas3d.transform.GetChild(0).gameObject, f, GameModeManager.instance.languageSystem.symbolTxt.rectTransform.rotation, GameModeManager.instance.canvas3d.transform).GetComponent<Text>();
                    txt.fontSize = 100;
                    txt.gameObject.SetActive(true);
                    tile_dict.Add(randpos, txt.gameObject);
                }
                else
                {
                    if (tile_dict.ContainsKey(randpos))
                        continue;
                    int r_idx = Random.Range(0, data.Length);
                    if (data[r_idx].symbol.Equals(GameModeManager.instance.languageSystem.symbolTxt.text))
                        continue;
                    ++times;
                    Vector3 f = GameModeManager.instance.gameGrid.GetCellMiddleWPOS(randpos);
                    f.Set(f.x, f.y + 0.4f, f.z);
                    Text txt = Instantiate(GameModeManager.instance.canvas3d.transform.GetChild(0).gameObject, f, GameModeManager.instance.languageSystem.symbolTxt.rectTransform.rotation, GameModeManager.instance.canvas3d.transform).GetComponent<Text>();
                    txt.fontSize = 100;
                    txt.text = data[r_idx].symbol;
                    txt.gameObject.SetActive(true);
                    tile_dict.Add(randpos, txt.gameObject);
                }
            }
        }
    }

    public void removeWordTile(Vector3Int pos)
    {
        if (tile_dict.ContainsKey(pos))
        {
            if (tile_dict[pos].GetComponent<Text>().text.Equals(GameModeManager.instance.languageSystem.symbolTxt.text))
            {
                GameModeManager.instance.num_event--;
            }
            Destroy(tile_dict[pos]);
            tile_dict.Remove(pos);
            destructCount--;
        }
    }

    public void deleteTileDict()
    {
        GameObject[] items = new GameObject[tile_dict.Count];
        tile_dict.Values.CopyTo(items, 0);
        tile_dict.Clear();
    }
}
