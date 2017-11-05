using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameGrid : MonoBehaviour 
{
    // Ortho cam top right corner coords
    Vector2 camTopRight;

    // Left end x coord of tile map
    public float mapWidthX { get; set; }
    public const int minGridHeight = 0;

    // the grid provided by unity with tilemap
    Grid unityGrid;
    Tilemap tilemap;

    public Sprite testSprite;

    public enum TILEMAP_TYPE
    {
        TILEMAP_ENVIRONMENT,
        TILEMAP_ENEMY,
        TILEMAP_PLAYER,
        TILEMAP_GRIDCELLS,
    }

	void Awake() 
    {
        unityGrid = GameObject.Find("Grid").GetComponent<Grid>();

        camTopRight.x = (Camera.main.orthographicSize) * Camera.main.aspect; 
        camTopRight.y = Camera.main.transform.position.y + (Camera.main.orthographicSize);

        mapWidthX = GetTopRightCell().x - 7;
        mapWidthX = GetCellWPOS(new Vector3Int((int)mapWidthX, GetTopRightCell().y, GetTopRightCell().z)).x;

        // Get the tile map from game scene
        tilemap = unityGrid.transform.GetChild(0).GetComponent<Tilemap>();
	}
	
	void Update() 
    {
        // the actual position of the top right cell
        Vector3 num = GetCellWPOS(GetTopRightCell());
        Vector2 actualPos = GetCellToWorld(new Vector3Int(2, 4, 0));
        //Debug.Log(actualPos);

        // number of cells in a row
        float totalGridWidth = (GetTopRightCell().x + 1) * 2;
        float numCellsInRow = totalGridWidth / unityGrid.cellSize.x;
        //Debug.Log(numCellsInRow);

        // number of cells in a col
        float totalGridHeight = (GetTopRightCell().y + 1) * 2;
        float numCellsInCol = totalGridHeight / unityGrid.cellSize.y;
        //Debug.Log(numCellsInCol);
    }

    public Vector3 GetTopRightGridWPOS() // ? convert and convert again
    {
        Vector3Int cellpos = unityGrid.WorldToCell(camTopRight);
        return unityGrid.CellToWorld(cellpos);
    }

    public Vector3Int GetTopRightCell()
    {
        Vector3Int temppos = unityGrid.WorldToCell(camTopRight);
        return new Vector3Int(temppos.x - 1, temppos.y - 1, temppos.z);
    }

    public Vector3 GetCellWPOS(Vector3Int cellpos)
    {
        return unityGrid.CellToWorld(cellpos);
    }

    public Vector3Int GetWorldIntToCellPos(Vector3Int worldPos)
    {
        return unityGrid.WorldToCell(Camera.main.ScreenToWorldPoint(worldPos));
    }

    public Vector3Int GetWorldFlToCellPos(Vector3 worldPos)
    {
        return unityGrid.WorldToCell(worldPos);
    }

    public Vector2 GetCellToWorld(Vector3Int cellPos)
    {
        Vector3 num = unityGrid.CellToWorld(cellPos);
        return new Vector2(num.x + (unityGrid.cellSize.x / 2), num.y + (unityGrid.cellSize.y / 2));
    }

    public Tile GetTileAtCellPos(GameGrid.TILEMAP_TYPE tileMapType, Vector3Int cellPos)
    {
        return (Tile)GetTilemap(tileMapType).GetTile(cellPos);
    }

    /// <summary>
    /// Gets tile at certain cell in default tilemap containing environment background
    /// </summary>
    /// <param name="cellPos">The cell to set a tile on</param>
    /// <param name="tile">The tile you want to insert in the cell</param>
    public void SetTile(Vector3Int cellPos, Tile tile)
    {
        tilemap.SetTile(cellPos, tile);
    }

    /// <summary>
    /// Gets tile at certain cell in specified tilemap
    /// </summary>
    /// <param name="map">Tilemap to place the tile</param>
    public void SetTile(TILEMAP_TYPE type, Vector3Int cellPos, Tile tile)
    {
        // temp values
        if (cellPos.x < -3 || cellPos.x > 3 || cellPos.y < minGridHeight || cellPos.y > 5)
            return;

        GetTilemap(type).SetTile(cellPos, tile);
    }
    
    /// <summary>
    /// Refreshes tile at certain cell in specified tilemap
    /// </summary>
    /// <param name="map">Tilemap to place the tile</param>
    public void ClearExistingTile(GameObject tileMapObj, Vector3Int cellPos)
    {
        tileMapObj.GetComponent<Tilemap>().RefreshTile(cellPos);
    }

    Tilemap GetTilemap(TILEMAP_TYPE type)
    {
        return unityGrid.transform.GetChild((int)type).GetComponent<Tilemap>();
    }
}
