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
    int minGridHeight;

    // the grid provided by unity with tilemap
    Grid unityGrid;
    public Grid getGrid
    {
        get { return unityGrid; }
    }

    Tilemap tilemap;

    public Sprite testSprite;

	void Start() 
    {
        unityGrid = GameObject.Find("Grid").GetComponent<Grid>();

        camTopRight.x = (Camera.main.orthographicSize) * Camera.main.aspect; 
        camTopRight.y = Camera.main.transform.position.y + (Camera.main.orthographicSize);

        mapWidthX = GetTopRightCell().x - 7;
        mapWidthX = GetCellWPOS(new Vector3Int((int)mapWidthX, GetTopRightCell().y, GetTopRightCell().z)).x;

        minGridHeight = 0;

        // Get the tile map from game scene
        tilemap = unityGrid.transform.GetChild(0).GetComponent<Tilemap>();
	}
	
	void Update() 
    {
        //Vector3 num = GetCellWPOS(GetTopRightCell());
        // the actual position of the top right cell
        //Debug.Log("x: " + (num.x - (world_grid.cellSize.x / 2)) + " y: " + (num.y + (world_grid.cellSize.y / 2)));

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

    public Vector3 GetCellMiddleWPOS(Vector3Int cellpos)
    {
        return unityGrid.GetCellCenterWorld(cellpos);
    }

    public Vector3Int GetWorldToCellPos(Vector3Int worldPos)
    {
        return unityGrid.WorldToCell(Camera.main.ScreenToWorldPoint(worldPos));
    }

    public Vector3Int GetWorldToCellPos(Vector3 worldPos)
    {
        return unityGrid.WorldToCell(worldPos);
    }

    public Tile GetTileAtCellPos(Vector3Int cellPos)
    {
        return (Tile)tilemap.GetTile(cellPos);
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
    /// Refreshes tile at certain cell in specified tilemap
    /// </summary>
    /// <param name="map">Tilemap to place the tile</param>
    public void ClearExistingTile(GameObject tileMapObj, Vector3Int cellPos)
    {
        tileMapObj.GetComponent<Tilemap>().RefreshTile(cellPos);
    }

    // Doesn't work
    public void SetSpriteAtCellPos(Vector3Int cellPos, Sprite sprite)
    {
        GetTileAtCellPos(cellPos).sprite = sprite;
    }

    // Doesn't work
    public Sprite GetSpriteAtCellPos(Vector3Int cellPos)
    {
        return GetTileAtCellPos(cellPos).sprite;
    }

    public int GetMinHeight()
    {
        return minGridHeight;
    }

    public Vector3Int[] GetPerimeter(int radius)
    {
        int width = radius * 2 + 1;
        Vector3Int[] cells = new Vector3Int[width * width]; // because there are 8 cells surrounding 1 cell in middle
        Vector3Int startcell = GetWorldToCellPos(EnemyMoveController.instance.GetBossObj.transform.position);
        startcell = new Vector3Int(startcell.x - radius, startcell.y - radius, startcell.z);
        int count = 0;
        for (int w = 0; w < width; ++w)
        {
            for (int h = 0; h < width; ++h)
            {
                cells[count] = new Vector3Int(startcell.x + w, startcell.y + h, startcell.z);
                count++;
            }
        }
        return cells;
    }

    public int GetCellsAwaySquared(Vector3Int pos, Vector3Int des)
    {
        Vector3Int cellsAway = des - pos;
        return cellsAway.sqrMagnitude;
    }
}
