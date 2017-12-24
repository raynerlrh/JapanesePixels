using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PNode {

    public Vector3Int pos;
    public bool passable;

    public int gCost;
    public int hCost;
    public int fcost
    {
        get { return gCost + hCost; }
    }
    public PNode parent;

    public PNode(Vector3Int position, bool pass)
    {
        pos = position;
        passable = pass;
        
    }

    public List<PNode> GetNeighbours()
    {
        List<PNode> neighbours = new List<PNode>();
        for (int x = -1; x <= 1; ++x)
            for (int y = -1; y <= 1; ++y)
            {
                if (x == 0 && y == 0)
                    continue;
                else if (Mathf.Abs(x) == Mathf.Abs(y)) // xy axis check preventing 1,1 scenario, no diagonals
                    continue;
                int checkx = pos.x + x;
                int checky = pos.y + y;
                // whether can pass depends on whether its a wall
                Vector3Int closeCell = new Vector3Int(checkx, checky, pos.z);
                if (WallCheck(closeCell))
                    neighbours.Add(new PNode(closeCell, true));
                //else
                    //neighbours.Add(new PNode(closeCell, false));
            }
        return neighbours;
    }

    public bool WallCheck(Vector3Int cell)
    {
        if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, cell) || TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_DESTRUCTIBLE, cell))
        {
            return false;
        }
        return true;
    }
}
