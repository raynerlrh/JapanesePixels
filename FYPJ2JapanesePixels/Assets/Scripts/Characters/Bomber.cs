using System.Collections.Generic;
using UnityEngine;

/**
 * This class is for CPU character interactions and assumes that it's attached to a parent gameobject
 * Follows a bomber guide for movement
 */

public struct MovementData
{
    public float speed;
    public MovementData(float speedVal = 2f)
    {
        speed = speedVal;
    }
}

public class Bomber : MonoBehaviour {
    private bool canDrop;
    public BomberGuide guide;
    MovementData movement;
    TimerRoutine delay;
    public Vector3Int dropSpot;
    private int bombRange;
    public int sightRange;
    private GameObject dmgBody;
    enum BomberState
    {
        E_PATROL,
        E_ALERT,
        E_AVOID
    }
    BomberState e_state;
    public GameObject bombRef;
    Vector3Int myCellPos;
    Vector3Int prevCellPos;

    Rigidbody2D physBody;
    int priorityRange;
    Animator anim;

    // Use this for initialization
    void Start () {
        canDrop = true;
        movement = new MovementData(2);
        {
            delay = gameObject.AddComponent<TimerRoutine>();
            delay.initTimer(6f);
            delay.executedFunction = resetBomb;
        }
        bombRange = 1;
        sightRange = bombRange + 4;
        BoxCollider2D col = gameObject.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1f * sightRange, 1f);
        col.isTrigger = true;
        col = gameObject.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1f, 1f * sightRange);
        col.isTrigger = true;
        dmgBody = transform.parent.GetChild(2).gameObject;
        dmgBody.GetComponent<DefaultCharacter>().InitChar();
        e_state = BomberState.E_PATROL;
        bombRef = null;
        prevCellPos = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.position);
        physBody = transform.parent.GetComponent<Rigidbody2D>();
        priorityRange = 0;
        anim = transform.parent.GetChild(3).GetChild(0).GetComponent<Animator>();
        guide = transform.parent.GetComponent<BomberGuide>();
    }
	
	// Update is called once per frame
	void Update () {
        // Moving towards guide
        float dist = Vector2.Distance(transform.position, guide.bomberdes);
        float speed = (movement.speed * Time.deltaTime);
        if (dist > speed)
        {
            Vector2 direction = (guide.bomberdes - transform.position).normalized;
            physBody.MovePosition((Vector2)transform.position + direction * speed);
        }
        else
        {
            guide.canProceed = true; // tell guide that i reached
        }

        if (dmgBody.GetComponent<DefaultCharacter>().checkIfDead()) // no more health, destroy myself
        {
            Destroy(transform.parent.gameObject);
        }

        myCellPos = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.position);
        if (myCellPos != prevCellPos)
        {
            TileRefManager.instance.SetTile(TileRefManager.TILEMAP_TYPE.TILEMAP_PLAYER, prevCellPos, null);
            prevCellPos = myCellPos;
            TileRefManager.instance.SetTile(TileRefManager.TILEMAP_TYPE.TILEMAP_PLAYER, myCellPos, TileRefManager.instance.GetTileRef(TileRefManager.TILE_TYPE.TILE_WARNING));
        }
        animate();
        //switch ()

        //if (direction == Dpad.MOVE_DIR.LEFT)
        //{
        //    p_animator.transform.localScale = new Vector3(1, 1, 1);
        //}
        //else if (dpad.moveDir == Dpad.MOVE_DIR.RIGHT)
        //{
        //    p_animator.transform.localScale = new Vector3(-1, 1, 1);
        //}
    }

    void OnTriggerStay2D(Collider2D obj)
    {
        // if its not me
                    /*if (obj.gameObject.layer == 14)
            {
                // get player cell pos
                Vector3Int otherBomber = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(obj.transform.position);
                //if (detectPlayerApprox(otherBomber, sightRange)) // check if this object is within my sight
                if (xyAxisCheck(otherBomber)) // is this object on either of my  4 direction
                {
                    //if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, findPlayerDir()) == null)
                    if (XYObstacleCheck(otherBomber, bombRange) == false) // if there is nothing blocking my path to bomb the object
                        if (canDrop) // if i can drop a bomb right now
                        {
                            // Before i drop, i should do some thinking first, is there a cell i can hide? should i drop it in a deliberate place?
                            dropBomb(); // drop bomb
                            delay.executeFunction(); // cool down for bomb drop
                            canDrop = false;
                        }
                    //guide.GetComponent<BomberGuide>().setDirection(findNextDirection(otherBomber)); // chase a player or destructable
                }
            }*/
        if (!obj.gameObject.Equals(transform.parent.gameObject) && !obj.gameObject.Equals(dmgBody))
        {
            if (obj.gameObject.layer == 14 || obj.gameObject.layer == 17) // is this a object i care about
            {
                if (canDrop && dmgBody.GetComponent<Inventory>().OnHandAmount > 0) // if i can drop a bomb right now
                {
                    // Before i drop, i should do some thinking first, is there a cell i can hide? should i drop it in a deliberate place?
                    if (XYValidDrop(bombRange))
                    {
                        List<Vector3Int> cells = findHidingPlace(GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.position), bombRange);
                        guide.findWaypoint(cells, transform.position);
                        if (guide.hasWaypoint)
                            dropBomb(); // drop bomb
                        delay.executeFunction(); // cool down for bomb drop
                        canDrop = false;
                        dmgBody.GetComponent<Inventory>().OnHandAmount--;
                    }
                }
            }
            // the following block should not be called if it is the bomber's own bomb
            if (obj.gameObject.layer == 10 && !obj.gameObject.Equals(bombRef) && obj.gameObject.CompareTag("Interactable")) // is this a solidobject? Ignore my own bomb and do not detect flames which are also solidobjs
            {
                if (bombRef == null)
                    priorityRange = 0;
                int bombRange = obj.gameObject.GetComponent<Bomb>().effectRange;

                //if (bombRange > priorityRange)
                {
                    bombRef = obj.gameObject;
                    priorityRange = bombRange;
                    List<Vector3Int> cells = findHidingPlace(GameModeManager.instance.gameGrid.GetWorldFlToCellPos(obj.transform.position), obj.gameObject.GetComponent<Bomb>().effectRange);
                    guide.resetWaypoints();
                    guide.findWaypoint(cells, obj.transform.position);
                }
            }
            //else if (obj.gameObject.layer == 10 && obj.gameObject.CompareTag("InventoryItem"))
            //{

            //}
        }
        //avoidBomb(obj);
    }

    //void OnCollisionStay2D(Collision2D obj)
    //{
    //    print("ff");
    //    if (obj.gameObject.layer == 10)
    //    {
    //        List<Vector3Int> cells = findHidingPlace();
    //        guide.GetComponent<BomberGuide>().findWaypoint(cells);
    //    }
    //}

    private void dropBomb()
    {
        Vector3Int mycell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.position);
        Vector3 spawnPos = GameModeManager.instance.gameGrid.GetCellMiddleWPOS(mycell); //guide.GetComponent<BomberGuide>().convertDirToVec3()
        bombRef = GameObject.Instantiate(dmgBody.GetComponent<Inventory>().OnHandItem, spawnPos, transform.parent.localRotation);
        bombRef.GetComponent<Bomb>().effectRange = dmgBody.GetComponent<Inventory>().OnHandRange;
        setRange(dmgBody.GetComponent<Inventory>().OnHandRange);
        bombRef.GetComponent<Collider2D>().isTrigger = true;
        bombRef.SetActive(true);
    }
    
    private void resetBomb()
    {
        canDrop = true;
    }

    private int cellsAway(Vector3Int cell1, Vector3Int cell2)
    {
        Vector3Int cellsAway = cell2 - cell1;
        return (int)cellsAway.magnitude;
    }

    private bool XYValidDrop(int range)
    {
        Vector3Int mycell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.position);
        Vector3Int modified = mycell;

        for (int x = 1; x <= range; ++x)
        {
            modified.Set(mycell.x + x, mycell.y, mycell.z);
            if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_DESTRUCTIBLE, modified) )
            {
                    return true;
            }
            else if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, modified))
            {
                break;
            }
            //else
                //TileRefManager.instance.SetTile(TileRefManager.TILEMAP_TYPE.TILEMAP_GRIDCELLS, modified, TileRefManager.instance.GetTileRef(TileRefManager.TILE_TYPE.TILE_WARNING));
        }
        for (int x = -1; x >= -range; --x)
        {
            modified.Set(mycell.x + x, mycell.y, mycell.z);
            if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_DESTRUCTIBLE, modified))
            {
                    return true;
            }
            else if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, modified))
            {
                break;
            }
        }
        for (int y = 1; y <= range; ++y)
        {
            modified.Set(mycell.x, mycell.y + y, mycell.z);
            if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_DESTRUCTIBLE, modified))
            {
                return true;
            }
            else if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, modified))
            {
                break;
            }
        }
        for (int y = -1; y >= -range; --y)
        {
            modified.Set(mycell.x, mycell.y + y, mycell.z);
            if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_DESTRUCTIBLE, modified) /*|| TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_PLAYER, modified)*/)
            {
                return true;
            }
            else if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, modified))
            {
                break;
            }
        }
        return false;
    }

    public List<Vector3Int> findHidingPlace(Vector3Int bombcell, int range)
    {
        Vector3Int mycell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.position);
        Vector3Int modified = mycell;
        List<Vector3Int> danger = GetCrossCells(range, bombcell);
        List<Vector3Int> safe = new List<Vector3Int>();
        int hiderange = sightRange;
        if (range > sightRange)
            hiderange = range;
        for (int x = -hiderange; x <= hiderange; ++x)
        {
            for (int y = -hiderange; y <= hiderange; ++y)
            {
                modified.Set(mycell.x + x, mycell.y + y, mycell.z);
                if (danger.Contains(modified))
                    continue;
                if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, modified) || TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_DESTRUCTIBLE, modified))
                    continue;
                //if (modified.x == mycell.x || modified.y == mycell.y)
                    //continue;
                if (cellsAway(mycell, modified) <= sightRange)
                {
                    safe.Add(modified);
                }
            }
        }
        //for (int i = 0; i < safe.Count; ++i)
        //    TileRefManager.instance.SetTile(TileRefManager.TILEMAP_TYPE.TILEMAP_ENEMY, safe[i], TileRefManager.instance.GetTileRef(TileRefManager.TILE_TYPE.TILE_DEBUG));
        return safe;
    }

    private List<Vector3Int> GetCrossCells(int range, Vector3Int targetcell)
    {
        Vector3Int modified = targetcell;
        List<Vector3Int> vectors = new List<Vector3Int>();
        for (int x = -range; x <= range; ++x)
        {
            modified.Set(targetcell.x + x, targetcell.y, targetcell.z);
            vectors.Add(modified);
            modified.Set(targetcell.x, targetcell.y + x, targetcell.z);
            vectors.Add(modified);
        }
        return vectors;
    }

    private void animate()
    {
        int dir = guide.getDirection();
        //print(dir);
        switch((BomberGuide.MOVEDIR)dir)
        {
            case BomberGuide.MOVEDIR.E_LEFT:
                anim.transform.localScale = new Vector3(1, 1, 1);
                break;
            case BomberGuide.MOVEDIR.E_RIGHT:
                anim.transform.localScale = new Vector3(-1, 1, 1);
                break;
        }
        anim.SetFloat("NormalizeSpd", dir);
    }

    public void setRange(int range)
    {
        bombRange = range;
        sightRange = bombRange + 4;
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        col.size = new Vector2(1f * sightRange, 1f);
        BoxCollider2D[] boxColliders = GetComponents<BoxCollider2D>();
        for (int i = 0; i < boxColliders.Length; ++i)
            if (!boxColliders[i].Equals(col))
            {
                col = boxColliders[i];
                break;
            }
        col.size = new Vector2(1f, 1f * sightRange);
    }
}
