using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MovementData
{
    public bool reach;
    public float speed;
    public MovementData(bool reached = true)
    {
        reach = reached;
        speed = 2f;
    }

}

public class BomberGuide : MonoBehaviour {
    TimerRoutine delay; // for testing movement

    enum MOVEDIR
    {
        E_LEFT = 0,
        E_RIGHT,
        E_UP,
        E_DOWN,
        E_CENTRE
    }

    MOVEDIR direction;
    Vector3Int destination;
    MovementData movement;
    public bool canProceed;
    public int cellsFree = 3;
    private int originalCellsFree;

	// Use this for initialization
	void Start () {
        direction = MOVEDIR.E_CENTRE;
        direction = getRandomDir();
        //prevDir = direction;
        {
            delay = gameObject.AddComponent<TimerRoutine>();
            delay.initTimer(2);
            delay.executedFunction = moveForward;
        }
        movement = new MovementData(true);
        canProceed = false;
        originalCellsFree = cellsFree;
	}
	
	// Update is called once per frame
	void Update () {
        //if (movement.reach)
        if (canProceed)
        {
            // if sees player
            //chooseDir();
            if (!obstacleCheck(direction, 1))
            {
                destination = convertDirToVec3();
                //delay.executeFunction();
                moveForward();
                canProceed = false;
                //movement.reach = false;
                cellsFree = originalCellsFree;
            }
            else
            {
                MOVEDIR randDir = getRandomDir();
                if (!obstacleCheck(randDir, cellsFree)) // if i meet with a wall, i will find a new path with preferably as many cells free as the range u provide
                //if (recursionCheck(randDir, 3)) // not guaranteed to work yet, recursion is hard
                {
                    direction = randDir;
                }
                else
                    cellsFree--;
            }
        }
	}
    
    private int cellsAway(Vector3Int cell1, Vector3Int cell2)
    {
        Vector3Int cellsAway = cell2 - cell1;
        return (int)cellsAway.magnitude;
    }

    private Vector3Int chooseDir(int redo = 1, MOVEDIR lockDir = MOVEDIR.E_CENTRE)
    {
        if (redo == -1)
        {
            return destination;
        }

        Vector3Int mycell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.position);
        Vector3Int modified = mycell;
        Vector3Int targetcell = PlayerMoveController.instance.GetPlayerCellPos; // target can be anyone, not just the player
        //int locked = (int)lockDir;
        for (int i = 0; i < 4; i++)
        {
            //if (i == locked)
                //continue;
            if (i == 0)
                modified.Set(mycell.x - redo, modified.y, modified.z);
            if (i == 1)
                modified.Set(mycell.x + redo, mycell.y, mycell.z);
            if (i == 2)
                modified.Set(mycell.x, mycell.y + redo, mycell.z);
            if (i == 3)
                modified.Set(mycell.x, mycell.y - redo, mycell.z);
            if (cellsAway(modified, targetcell) < cellsAway(mycell, targetcell))
            {
                if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, modified))
                {
                    // raycast/collide few blocks ahead to see if there is a wall ahead
                    //continue;
                    //if (redo == -1)
                    //chooseDir(redo + 1); // for recursion
                    //lockDir = (MOVEDIR)i; // if removed, object skips block
                    break;
                }
                //redo = -1; // for recursion
                destination = modified;
                direction = (MOVEDIR)i;
                break;
            }
        }
        return destination;
    }

    private MOVEDIR getRandomDir()
    {
        int randnum = Random.Range(0, 100);
        if (randnum < 25)
            return MOVEDIR.E_LEFT;
        else if (randnum >= 25 && randnum < 50)
            return MOVEDIR.E_RIGHT;
        else if (randnum >= 50 && randnum < 75)
            return MOVEDIR.E_UP;
        else
            return MOVEDIR.E_DOWN;
    }

    private void moveForward()
    {
        Vector3 newpos = GameModeManager.instance.gameGrid.GetCellMiddleWPOS(destination);
        transform.position = newpos;
        movement.reach = true;
    }

    public Vector3Int convertDirToVec3()
    {
        Vector3Int mycell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.position);
        switch (direction)
        {
            case MOVEDIR.E_LEFT:
                return new Vector3Int(mycell.x - 1, mycell.y, mycell.z);
            case MOVEDIR.E_RIGHT:
                return new Vector3Int(mycell.x + 1, mycell.y, mycell.z);
            case MOVEDIR.E_UP:
                return new Vector3Int(mycell.x, mycell.y + 1, mycell.z);
            case MOVEDIR.E_DOWN:
                return new Vector3Int(mycell.x, mycell.y - 1, mycell.z);
            default:
                return mycell;
        }
    }

    private Vector3Int convertDirToVec3(MOVEDIR desired)
    {
        Vector3Int mycell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.position);
        switch (desired)
        {
            case MOVEDIR.E_LEFT:
                return new Vector3Int(mycell.x - 1, mycell.y, mycell.z);
            case MOVEDIR.E_RIGHT:
                return new Vector3Int(mycell.x + 1, mycell.y, mycell.z);
            case MOVEDIR.E_UP:
                return new Vector3Int(mycell.x, mycell.y + 1, mycell.z);
            case MOVEDIR.E_DOWN:
                return new Vector3Int(mycell.x, mycell.y - 1, mycell.z);
            default:
                return mycell;
        }
    }

    private bool obstacleCheck(MOVEDIR dir, int range)
    {
        Vector3Int mycell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.position);
        Vector3Int test = Vector3Int.zero;
        bool fail = false;
        switch (dir)
        {
            case MOVEDIR.E_LEFT:
                for (int i = 0; i <= range; ++i)
                {
                    test.Set(mycell.x - i, mycell.y, mycell.z);
                    if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, test))
                    {
                        fail = true;
                        break;
                    }
                }
                break;
            case MOVEDIR.E_RIGHT:
                for (int i = 0; i <= range; ++i)
                {
                    test.Set(mycell.x + i, mycell.y, mycell.z);
                    if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, test))
                    {
                        fail = true;
                        break;
                    }
                }
                break;
            case MOVEDIR.E_UP:
                for (int i = 0; i <= range; ++i)
                {
                    test.Set(mycell.x, mycell.y + i, mycell.z);
                    if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, test))
                    {
                        fail = true;
                        break;
                    }
                }
                break;
            case MOVEDIR.E_DOWN:
                for (int i = 0; i <= range; ++i)
                {
                    test.Set(mycell.x, mycell.y - i, mycell.z);
                    if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, test))
                    {
                        fail = true;
                        break;
                    }
                }
                break;
        }
        return fail;
    }

    public void setDirection(int dir)
    {
        direction = (MOVEDIR)dir;
    }

    public int getDirection()
    {
        return (int)direction;
    }

    private bool recursionCheck(MOVEDIR dir, int range)
    {
        if (!obstacleCheck(dir, range))
            return true;
        else
        {
            --range;
            if (!obstacleCheck(dir, range))
                return true;
            return false;
        }
    }
}
