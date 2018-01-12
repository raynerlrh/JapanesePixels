using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPath : MonoBehaviour {
    PNode startnode;
    PNode endnode;
    List<Vector3Int> taken_list;
    List<PNode> neighbours;

    List<Vector3Int> pathfind(PNode start, PNode goal)
    {
        //start.gCost = 0;
        //start.hCost = GetDistHeuristic(start, goal);
        List<PNode> open_list; // list
        List<Vector3Int> close_list; // list
        List<Vector3Int> want_list; // list
        open_list = new List<PNode>();
        close_list = new List<Vector3Int>();
        want_list = new List<Vector3Int>();
        open_list.Add(start);
        int temp = 0; // temp is for debugging if this algo crashes becus of the While
        while (open_list.Count > 0)
        {
            temp++;
            PNode curr = open_list[0];
            for (int i = 1; i < open_list.Count; ++i)// 1 because doesnt count start node
            {
                if (open_list[i].fcost < curr.fcost || open_list[i].fcost == curr.fcost && open_list[i].hCost < curr.hCost)
                {
                    curr = open_list[i];
                }
            }

            if (curr.pos == goal.pos)
            {
                //print("ff");
                want_list.Add(goal.pos);
                return want_list;
                //return RetracePath(start, goal, want_list); ; // mission accomplished
            }
            else if (temp > 200)
            {
                //print("break");
                return null;
            }

            open_list.Remove(curr);
            close_list.Add(curr.pos);

            curr.GetNeighbours(ref neighbours); // list
            //print(neighbours.Count);
            for (int j = 0; j < neighbours.Count; ++j)
            {
                if (!neighbours[j].passable || close_list.Contains(neighbours[j].pos))
                {
                    //print(neighbours.Count);
                    continue;
                }
                int curr2Neighbour = curr.gCost + GetDistHeuristic(curr, neighbours[j]);
                int start2Neighbour = neighbours[j].gCost;
                // if cost of moving from my curr node to neighbour is more than start to neightbour or if ne
                if (curr2Neighbour < start2Neighbour || !open_list.Contains(neighbours[j]))
                {
                    // we want this node
                    neighbours[j].gCost = curr2Neighbour;
                    neighbours[j].hCost = GetDistHeuristic(neighbours[j], goal);
                    neighbours[j].parent = curr; // isnt actually used
                    if (!want_list.Contains(curr.pos))
                        want_list.Add(curr.pos);
                    open_list.Add(neighbours[j]);
                }
            }
        }
        return null;
    }

    List<Vector3Int> RetracePath(PNode start, PNode target, List<Vector3Int> list = null)
    {
        //List<PNode> path = new List<PNode>();
        //PNode current = target;
        //while(current != start)
        //{
        //    path.Add(current);
        //    current = current.parent;
        //}
        //path.Reverse();
        return list;
    }

    int GetDistHeuristic(PNode anode, PNode bnode) // manhattan distance heuristics
    {
        int xdist = Mathf.Abs(anode.pos.x - bnode.pos.x); // when a node is subtracted from another, we get the x/y/hori/vert number of nodes to reach
        int ydist = Mathf.Abs(anode.pos.y - bnode.pos.y);
        //return (xdist + ydist) * 10;
        // research heuristics
        if (xdist > ydist) // the node requires more horizontal node movement to get to than vertical nodes
        {
            // since we get how many nodes in x/y direction, we can just multiply them with our cost value, 10, normally got 14 cost for diagonal but our game has no diagonal
            return 14 * ydist + 10 * (xdist - ydist);
        }
        return 14 * xdist + 10 * (ydist - xdist);
    }

    public List<Vector3Int> runPathFinding()
    {
        //if (!once)
        {
            taken_list = pathfind(startnode, endnode);
            //once = true;
        }
        /*else
        {
            if (taken_list != null)
            {
                //for (int i = 0; i < taken_list.Count; ++i)
                //    print(taken_list[i]);
                once = false;
            }
        }*/
        return taken_list;
    }

    public void init(Vector3Int endcell)
    {
        startnode = new PNode(GameModeManager.instance.gameGrid.GetWorldFlToCellPos(GetComponent<BomberGuide>().bomberobj.transform.position), true);
        endnode = new PNode(endcell, true);
        taken_list = null;
        //open_list.Clear();
        //close_list.Clear();
        //want_list.Clear();
        //open_list.TrimExcess();
        //close_list.TrimExcess();
        //want_list.TrimExcess();
    }
}
