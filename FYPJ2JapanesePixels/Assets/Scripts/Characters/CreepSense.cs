using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepSense : MonoBehaviour {

    private Minions m_info;
    public bool canDmg = true;

    private void Start()
    {
        m_info = transform.parent.GetComponent<Minions>();
        charstat = transform.parent.GetComponent<CharacterStats>();
        if (!canDmg)
        {
            BoxCollider2D col = gameObject.AddComponent<BoxCollider2D>();
            col.size = new Vector2(m_info.cellsFree, m_info.cellsFree);
            col.isTrigger = true;
        }
    }
    bool once = false;
    CharacterStats charstat;

    private void LateUpdate()
    {
        if (once)
            once = false;
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (!obj.Equals(gameObject))
        {
            if (obj.gameObject.layer == 14 && !once && canDmg)
            {
                obj.GetComponent<CharacterStats>().decreaseHealth(charstat.attackVal);
                once = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D obj)
    {
        if (!obj.Equals(gameObject))
        {
            if (obj.gameObject.layer == 14 && m_info.minionState == Minions.MinionState.E_NEUTRAL)
            {
                Vector3Int otherBomber = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(obj.transform.position);
                if (xyAxisCheck(otherBomber))
                {
                    if (XYObstacleCheck(otherBomber, m_info.cellsFree) == false)
                    {
                        m_info.direction = (BomberGuide.MOVEDIR)findNextDirection(otherBomber);
                    }
                }
            }

            if (obj.gameObject.layer == 10 && obj.gameObject.CompareTag("Interactable"))
            {
                m_info.minionState = Minions.MinionState.E_AVOID;
                avoidBomb(obj);
            }
            else
            {
                m_info.minionState = Minions.MinionState.E_NEUTRAL;
            }
        }
    }

    void avoidBomb(Collider2D obj)
    {
        Vector3Int bombcell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(obj.transform.position);
        switch (findNextDirection(bombcell))
        {
            case 0:
                m_info.direction = (BomberGuide.MOVEDIR)1;
                break;
            case 1:
                m_info.direction = (BomberGuide.MOVEDIR)0;
                break;
            case 2:
                m_info.direction = (BomberGuide.MOVEDIR)3;
                break;
            case 3:
                m_info.direction = (BomberGuide.MOVEDIR)2;
                break;
        }
    }

    private bool xyAxisCheck(Vector3Int targetcell)
    {
        Vector3Int mycell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.position);
        if (targetcell.x == mycell.x || targetcell.y == mycell.y)
        {
            return true;
        }
        return false;
    }

    private int cellsAway(Vector3Int cell1, Vector3Int cell2)
    {
        Vector3Int cellsAway = cell2 - cell1;
        return (int)cellsAway.magnitude;
    }

    public int findNextDirection(Vector3Int targetcell)
    {

        Vector3Int mycell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.position);
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
        return (int)m_info.direction;
    }

    private bool XYObstacleCheck(Vector3Int targetcell, int range)
    {
        Vector3Int mycell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.position);
        Vector3Int modified = mycell;
        Vector3Int des = targetcell - mycell;
        bool block = false;
        for (int i = 0; i <= range; ++i)
        {
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
            if (TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_SOLIDWALL, modified) || TileRefManager.instance.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_DESTRUCTIBLE, modified))
            {
                block = true;
                break;
            }
        }
        return block;
    }
}
