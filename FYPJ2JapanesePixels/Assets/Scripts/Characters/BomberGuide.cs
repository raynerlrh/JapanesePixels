﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class contains information to navigate a CPU character's sprite
 * Extension: Guide can now be given a specific destination to go to
 **/

public class BomberGuide : MonoBehaviour {

    public enum MOVEDIR
    {
        E_LEFT = 0,
        E_RIGHT,
        E_UP,
        E_DOWN,
        E_CENTRE
    }

    MOVEDIR direction;
    Vector3Int destination;
    private Vector3 waypoint;
    public bool canProceed;
    public int cellsFree = 3;
    private int originalCellsFree;
    public bool hasWaypoint = false;
    List<Vector3Int> waypoints;
    public int wpIndex; // waypoint index
    Vector3Int lastValid;
    public GameObject bomberobj;
    public TimerRoutine safetyTimer;

	// Use this for initialization
	void Start () {
        direction = getRandomDir();
        canProceed = false;
        originalCellsFree = cellsFree;
        wpIndex = 0;
        safetyTimer = gameObject.AddComponent<TimerRoutine>();
        safetyTimer.initTimer(3);
        safetyTimer.executedFunction = resetWaypoints;

    }
	
	// Update is called once per frame
	void Update () {
        // if i can move
        if (canProceed)
        {
            // check if the cell in front of me is blocked
            if (!hasWaypoint)
            {
                if (direction == MOVEDIR.E_CENTRE)
                    direction = getRandomDir();

                if (!obstacleCheck(direction, 1))
                {
                    destination = convertDirToVec3(); // not blocked, get next cell to go to depending on my direction
                    moveForward(); // move cell by cell
                    canProceed = false; // until my sprite catches up to me, dont move
                    cellsFree = originalCellsFree; // just resetting a variable
                }
                else
                {
                    MOVEDIR randDir = getRandomDir(); // Blocked, get a random direction
                    if (!obstacleCheck(randDir, cellsFree)) // if i meet with a wall, i will find a new path with preferably as many cells free as the range provided
                    //if (recursionCheck(randDir, 3)) // not guaranteed to work yet, recursion is hard
                    {
                        direction = randDir;
                    }
                    else
                        cellsFree--; // check from max range to 0
                }


            }
            else if (hasWaypoint) // if i have a destination, pathfind
            {
                if (waypoints == null) // pathfind gives waypoint cells to move to
                {
                    GetComponent<AStarPath>().init(GameModeManager.instance.gameGrid.GetWorldFlToCellPos(waypoint));
                    waypoints = GetComponent<AStarPath>().runPathFinding();
                    checkWaypointValidity();
                }
                else // move
                {
                    //hasWaypoint = false;
                    if (wpIndex < waypoints.Count - 1)
                    {
                        wpIndex++;
                        //if (wpIndex == 1)
                        //lastValid = waypoints[1];
                        checkWaypointValidity();
                        //for (int i = 0; i < waypoints.Count; ++i)
                        //print(waypoints[i] + "co");
                        direction = convertVec3ToDir(bomberobj.transform.position, waypoints[wpIndex]);
                        moveToWaypoint();
                        canProceed = false;
                    }
                    else
                    {
                        if (!safetyTimer.hasRun)
                            safetyTimer.executeFunction();
                        direction = MOVEDIR.E_CENTRE;
                    }
                }
                //move
                //if (waypoints != null)
                //{
                //    wpIndex++;
                //    checkWaypointValidity();
                //    for (int i = 0; i < waypoints.Count; ++i)
                //        print(waypoints[i] + "co");
                //    moveToWaypoint();
                //    canProceed = false;
                //    if (wpIndex >= waypoints.Count - 1)
                //    {
                //        waypoints = null;
                //    }

                //}
                //else
                //{
                //    if (bomberobj.GetComponent<Bomber>().bombRef == null)
                //    {
                //        if (!safetyTimer.hasRun)
                //        {
                //            safetyTimer.executeFunction();
                //        }
                //    }
                //}
            }
        }
    }

    public void resetWaypoints()
    {
        wpIndex = 0;
        hasWaypoint = false;
        waypoints = null;
        safetyTimer.hasRun = false;
    }
    
    private int cellsAway(Vector3Int cell1, Vector3Int cell2)
    {
        Vector3Int cellsAway = cell2 - cell1;
        return (int)cellsAway.magnitude;
    }

    private float fCellsAway(Vector3Int cell1, Vector3Int cell2)
    {
        Vector3Int cellsAway = cell2 - cell1;
        return cellsAway.magnitude;
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
    }

    private void moveToWaypoint()
    {
        Vector3 newpos = GameModeManager.instance.gameGrid.GetCellMiddleWPOS(waypoints[wpIndex]);
        transform.position = newpos;
    }

    // if waypoint is more than 1 cell away, sound buzzer
    private bool checkWaypointValidity()
    {
        Vector3Int last = waypoints[0];
        List<Vector3Int> delete = new List<Vector3Int>();
        for (int i = 0; i < waypoints.Count; ++i)
        {
            float dist = fCellsAway(waypoints[i], last);
            //print(waypoints[wpIndex]);
            //print(lastValid);
            if (dist > 1)
            {
                delete.Add(waypoints[i]);
                // waypoints.RemoveAt(i);
                //return false;
            }
            else
                last = waypoints[i];
        }
        List<int> ilist = new List<int>();
        for (int j = 0; j < waypoints.Count; ++ j)
        {
            for (int c = 0; c < delete.Count; ++c)
            {
                if (waypoints[j] == delete[c])
                    ilist.Add(j);
            }
        }

        for (int f = 0; f < ilist.Count; ++f)
        {
            waypoints.RemoveAt(ilist[f] - f);
        }
        return true;

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

    public MOVEDIR convertVec3ToDir(Vector3 startpos, Vector3Int des)
    {
        Vector3Int mycell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(startpos);
        Vector3Int diff = des - mycell;
        if (diff.x == 0)
        {
            if (des.y > mycell.y)
                return MOVEDIR.E_UP;
            else
                return MOVEDIR.E_DOWN;
        }
        else if (diff.y == 0)
        {
            if (des.x > mycell.x)
                return MOVEDIR.E_RIGHT;
            else
                return MOVEDIR.E_LEFT;
        }
        return MOVEDIR.E_CENTRE;
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
                    if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, test) || TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_DESTRUCTIBLE, test))
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
                    if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, test) || TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_DESTRUCTIBLE, test))
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
                    if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, test) || TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_DESTRUCTIBLE, test))
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
                    if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, test) || TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_DESTRUCTIBLE, test))
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

    public void findWaypoint(List<Vector3Int> possible, Vector3 bombpos)
    {
        if (!hasWaypoint)
        {
            List<Vector3Int> pro = null;
            for (int i = 0; i < possible.Count; ++i)
            {
                if (pro == null) // pathfind gives waypoint cells to move to
                {
                    GetComponent<AStarPath>().init(possible[i]);
                    pro = GetComponent<AStarPath>().runPathFinding();
                    if (pro != null)
                    {
                        bool gg = false;
                        for (int w = 1; w < pro.Count; ++w) // 1 so it doesnt count the player's starting point which is actually the bomb cell
                        {
                            Vector3Int bombcell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(bombpos);
                            if (pro[w].Equals(bombcell))
                            {
                                gg = true;
                                break;
                                //waypoints = null;
                            }
                        }
                        if (gg)
                        {
                            continue;
                        }
                        waypoint = GameModeManager.instance.gameGrid.GetCellMiddleWPOS(possible[i]);
                        hasWaypoint = true;
                        break;
                    }
                }

            }
        }
    }
}
