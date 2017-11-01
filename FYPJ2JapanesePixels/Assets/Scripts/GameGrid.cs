using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameGrid : MonoBehaviour 
{
    // From previous file
    Vector2 camTopRight;
    public float mapWidthX { get; set; }
    int minGridHeight;

    Grid unityGrid;
    Tilemap tileMap;

    public Sprite testSprite;

	void Start() 
    {
        unityGrid = GameObject.Find("Grid").GetComponent<Grid>();

        camTopRight.x = (Camera.main.orthographicSize) * Camera.main.aspect;
        camTopRight.y = Camera.main.transform.position.y + (Camera.main.orthographicSize);

        mapWidthX = GetTopRightCell().x - 7;
        mapWidthX = GetCellWPOS(new Vector3Int((int)mapWidthX, GetTopRightCell().y, GetTopRightCell().z)).x;

        minGridHeight = 0;

        // Get the tile map
        tileMap = unityGrid.transform.GetChild(0).GetComponent<Tilemap>();
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

    public Vector3Int GetWorldToCellPos(Vector3Int worldPos)
    {
        return unityGrid.WorldToCell(Camera.main.ScreenToWorldPoint(worldPos));
    }

    public Tile GetTileAtCellPos(Vector3Int cellPos)
    {
        return (Tile)tileMap.GetTile(cellPos);
    }

    public void SetTile(Vector3Int cellPos, Tile tile)
    {
        tileMap.SetTile(cellPos, tile);
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
}
