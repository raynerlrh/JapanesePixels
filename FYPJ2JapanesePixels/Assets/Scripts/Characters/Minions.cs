using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIRS = BomberGuide.MOVEDIR;

public class Minions : MonoBehaviour
{
    public DIRS direction;
    private Vector3 bodyDestination;
    private bool canProceed;
    public int cellsFree = 3;
    private int originalCellsFree;
    private MovementData movement;
    Rigidbody2D physBody;
    DefaultCharacter character;
    Animator animator;

    public enum MinionState
    {
        E_NEUTRAL,
        E_AVOID
    }

    public enum MinionType
    {
        E_CREEP1,
        E_CREEP2,
        E_CREEP3
    }

    public MinionType m_MinionType;
    public MinionState minionState;

    void Start()
    {
        m_MinionType = MinionType.E_CREEP1;
        direction = BomberGuide.getRandomDir();
        canProceed = true;
        originalCellsFree = cellsFree;
        movement = new MovementData(3);
        physBody = transform.GetComponent<Rigidbody2D>();
        bodyDestination = transform.position;
        GetComponent<DefaultCharacter>().InitChar();
        character = GetComponent<DefaultCharacter>();
        animator = transform.GetChild(4).GetChild(0).GetComponent<Animator>();
        minionState = MinionState.E_NEUTRAL;
    }

    void Update()
    {
        if (canProceed)
        {
            // check if the cell in front of me is blocked
            if (!ObstacleCheck(direction, 1))
            {
                Vector3Int destination = convertDirToVec3(); // not blocked, get next cell to go to depending on my direction
                SetDestination(destination); // move cell by cell
                canProceed = false; // until my sprite catches up to me, dont move
                cellsFree = originalCellsFree; // just resetting a variable
            }
            else
            {
                DIRS randDir = BomberGuide.getRandomDir(); // Blocked, get a random direction
                if (!ObstacleCheck(randDir, cellsFree)) // if i meet with a wall, i will find a new path with preferably as many cells free as the range provided
                    direction = randDir;
                else
                    cellsFree--; // check from max range to 0
            }
        }
        // Moving towards guide
        float dist = Vector2.Distance(transform.position, bodyDestination);
        float speed = (movement.speed * Time.deltaTime);
        if (dist > speed)
        {
            Vector2 direction = (bodyDestination - transform.position).normalized;
            physBody.MovePosition((Vector2)transform.position + direction * speed);
        }
        else
        {
            canProceed = true; // tell guide that i reached
        }

        if (character.checkIfDead()) // no more health, destroy myself
        {
            int enemiesRemaining = GameModeManager.instance.getEnemiesLeft();
            enemiesRemaining -= 1;
            GameModeManager.instance.enemyLeftTxt.text = enemiesRemaining.ToString();
            //if (GameModeManager.instance.getSceneName().Equals("Level2"))
            //{
            //    if (enemiesRemaining < 2)
            //    {
            //        GameModeManager.instance.itemSpawner.spawnAnotherEnemy();
            //    }
            //}
            Destroy(this.gameObject);
        }

        animate();
    }

    bool ObstacleCheck(DIRS dir, int range)
    {
        Vector3Int mycell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.position);
        Vector3Int test = Vector3Int.zero;
        bool fail = false;
        switch (dir)
        {
            case DIRS.E_LEFT:
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
            case DIRS.E_RIGHT:
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
            case DIRS.E_UP:
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
            case DIRS.E_DOWN:
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

    public Vector3Int convertDirToVec3()
    {
        Vector3Int mycell = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.position);
        switch (direction)
        {
            case DIRS.E_LEFT:
                return new Vector3Int(mycell.x - 1, mycell.y, mycell.z);
            case DIRS.E_RIGHT:
                return new Vector3Int(mycell.x + 1, mycell.y, mycell.z);
            case DIRS.E_UP:
                return new Vector3Int(mycell.x, mycell.y + 1, mycell.z);
            case DIRS.E_DOWN:
                return new Vector3Int(mycell.x, mycell.y - 1, mycell.z);
            default:
                return mycell;
        }
    }

    private void SetDestination(Vector3Int descell)
    {
        Vector3 newpos = GameModeManager.instance.gameGrid.GetCellMiddleWPOS(descell);
        bodyDestination = newpos;
    }

    void animate()
    {
        int dir = (int)direction;
        switch (direction)
        {
            case BomberGuide.MOVEDIR.E_LEFT:
                animator.transform.localScale = new Vector3(1, 1, 1);
                break;
            case BomberGuide.MOVEDIR.E_RIGHT:
                animator.transform.localScale = new Vector3(-1, 1, 1);
                break;
        }
        animator.SetFloat("NormalizeSpd", dir);
    }
}