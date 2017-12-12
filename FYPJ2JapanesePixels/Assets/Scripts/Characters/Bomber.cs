using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : MonoBehaviour {
    private bool canDrop;
    public GameObject guide;
    MovementData movement;
    TimerRoutine delay;
    public Vector3Int dropSpot;
    private int bombRange;
    public int sightRange;
	// Use this for initialization
	void Start () {
        canDrop = true;
        movement = new MovementData(true);
        {
            delay = gameObject.AddComponent<TimerRoutine>();
            delay.initTimer(5);
            delay.executedFunction = resetBomb;
        }
        bombRange = 2;
        sightRange = bombRange * 2;
        BoxCollider2D col = gameObject.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.3f * sightRange, 0.3f);
        col.isTrigger = true;
        col = gameObject.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.3f, 0.3f * sightRange);
        col.isTrigger = true;
        transform.parent.GetComponent<DefaultCharacter>().InitChar();
	}
	
	// Update is called once per frame
	void Update () {
        float dist = Vector2.Distance(transform.parent.position, guide.transform.position);
        float speed = (movement.speed * Time.deltaTime);
        if (dist > speed)
        {
            Vector2 direction = (guide.transform.position - transform.parent.position).normalized;
            transform.parent.GetComponent<Rigidbody2D>().MovePosition((Vector2)transform.parent.position + direction * speed);
        }
        else
            guide.GetComponent<BomberGuide>().canProceed = true;

        if (transform.parent.GetComponent<DefaultCharacter>().checkIfDead())
        {
            Destroy(transform.parent.gameObject);
        }

	}

    void OnTriggerStay2D(Collider2D obj)
    {
        if (!obj.Equals(transform.parent.gameObject))
        {
            Vector3Int otherBomber = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(obj.transform.position);
            if (detectPlayerApprox(otherBomber, sightRange)) // check for collided obj
            {
                if (xyAxisCheck(otherBomber))
                {
                    if (obj.gameObject.layer == 14)
                    {
                        //if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, findPlayerDir()) == null)
                        
                        if (XYObstacleCheck(otherBomber, 3) == false)
                            if (canDrop)
                            {
                                dropBomb();
                                delay.executeFunction();
                                canDrop = false;
                            }
                        guide.GetComponent<BomberGuide>().setDirection(findNextDirection(otherBomber));
                    }
                }
            }
        }
        avoidBomb(obj);
    }

    private void dropBomb()
    {
        Vector3Int mycell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.parent.position);
        Vector3 spawnPos = GameModeManager.instance.gameGrid.GetCellMiddleWPOS(mycell); //guide.GetComponent<BomberGuide>().convertDirToVec3()
        GameObject bomb = GameObject.Instantiate(EnemyMoveController.instance.enemyPrefabs[2], spawnPos, transform.parent.localRotation);
        //bool passThrough = false;
        //if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, playerPos))
        //{
        //    playerPos = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(playerTrans.position);
        //    passThrough = true;
        //}
        //if (passThrough)
        bomb.GetComponent<Bomb>().effectRange = bombRange;
        bomb.GetComponent<Collider2D>().isTrigger = true;
        bomb.SetActive(true);
    }
    
    private void resetBomb()
    {
        canDrop = true;
    }

    // Approximately detect if player is in bomb range
    private bool detectPlayerApprox(Vector3Int targetcell, int range = 1)
    {
        Vector3Int mycell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.parent.position);
        Vector3Int cellsAway = targetcell - mycell;
        if (cellsAway.magnitude <= range)
            return true;
        return false;
    }

    private bool xyAxisCheck(Vector3Int targetcell)
    {
        Vector3Int mycell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.parent.position);
        if (targetcell.x == mycell.x || targetcell.y == mycell.y)
        {
            return true;
        }
        return false;
    }

    // closest cell to the player is not a wall
    public Vector3Int findPlayerDir()
    {
        Vector3Int mycell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.parent.position);
        Vector3Int modified = mycell;
        Vector3Int targetcell = PlayerMoveController.instance.GetPlayerCellPos;

        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
                modified.Set(mycell.x - 1, modified.y, modified.z);
            if (i == 1)
                modified.Set(mycell.x + 1, mycell.y, mycell.z);
            if (i == 2)
                modified.Set(mycell.x, mycell.y + 1, mycell.z);
            if (i == 3)
                modified.Set(mycell.x, mycell.y - 1, mycell.z);
            if (cellsAway(modified, targetcell) < cellsAway(mycell, targetcell))
            {
                break;
            }
        }

        return modified;
    }

    public int findNextDirection(Vector3Int targetcell)
    {

        Vector3Int mycell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.parent.position);
        Vector3Int modified = mycell;
        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
                modified.Set(mycell.x - 1, modified.y, modified.z);
            if (i == 1)
                modified.Set(mycell.x + 1, mycell.y, mycell.z);
            if (i == 2)
                modified.Set(mycell.x, mycell.y + 1, mycell.z);
            if (i == 3)
                modified.Set(mycell.x, mycell.y - 1, mycell.z);
            if (cellsAway(modified, targetcell) < cellsAway(mycell, targetcell))
            {
                return i;
            }
        }
        return guide.GetComponent<BomberGuide>().getDirection();
    }

    private int cellsAway(Vector3Int cell1, Vector3Int cell2)
    {
        Vector3Int cellsAway = cell2 - cell1;
        return (int)cellsAway.magnitude;
    }

    private bool XYObstacleCheck(Vector3Int targetcell, int range)
    {
        Vector3Int mycell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.parent.position);
        Vector3Int modified;
        Vector3Int des = targetcell - mycell;
        bool block = false;
        for (int i = 0; i <= range; ++i)
        {
            mycell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.parent.position);
            modified = mycell;
            if (des.x < 0)
                modified.Set(mycell.x - i, mycell.y, mycell.z);
            else if (des.x > 0)
                modified.Set(mycell.x + i, mycell.y, mycell.z);
            else if (des.y < 0)
                modified.Set(mycell.x, mycell.y - i, mycell.z);
            else if (des.y > 0)
                modified.Set(mycell.x, mycell.y + i, mycell.z);
            if (targetcell == modified)
                break;
            if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, modified))
            {
                block = true;
                break;
            }
        }
        return block;
    }

    void avoidBomb(Collider2D obj)
    {
        if (obj.gameObject.layer == 10 && obj.gameObject.CompareTag("Interactable"))
        {
            Vector3Int bombcell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(obj.transform.position);
            switch (findNextDirection(bombcell))
            {
                case 0:
                    guide.GetComponent<BomberGuide>().setDirection(1);
                    break;
                case 1:
                    guide.GetComponent<BomberGuide>().setDirection(0);
                    break;
                case 2:
                    guide.GetComponent<BomberGuide>().setDirection(3);
                    break;
                case 3:
                    guide.GetComponent<BomberGuide>().setDirection(2);
                    break;
            }
        }
    }
}
