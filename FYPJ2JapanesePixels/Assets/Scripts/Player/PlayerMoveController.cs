using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public struct MovementStats
{
    public float moveSpeed;
    public bool isMoving;
    public Vector3 destinatePos;

    public MovementStats(float movespd = 2f, bool startMoving = false)
    {
        moveSpeed = movespd;
        isMoving = startMoving;
        destinatePos = Vector3.zero;
    }
}

public class PlayerMoveController : MonoBehaviour
{
    public static PlayerMoveController instance = null;
    private DefaultCharacter pawn; // main character, can be an array of pawns for each character
    private GameObject pawn_sprite; // can be an array of sprites for each character
    public DefaultCharacter GetPawn
    {
        get
        {
            return pawn;
        }
    }

    //public const int MAX_MOVES = 5;
    GameGrid gameGrid;

    [SerializeField]
    Sprite availableTileSprite;

    [SerializeField]
    Dpad dpad;

    Tile originalTile;
    Tile availableTile;

    Vector3Int touchPos;
    Vector3Int touchCellPos;
    //Vector3Int selectedCellPos;
    Vector3Int playerCellPos;

    Vector2 playerPos;
    Vector2 targetTilePos;

    Animator p_animator;

    public int numAvailableMoves { get; set; }
    public bool b_answeredCorrectly { get; set; }
    bool b_shownCrossTiles;
    bool b_reachedTarget;
    bool b_touchedScreen;

    public MovementStats moveStat;
    private Rigidbody2D body;

    public enum PlayState
    {
        E_NONCOMBAT,
        E_COMBAT
    }
    public LayerMask masker;

    public PlayState e_playstate;
    public bool isOver;

    // Only for most foremost operations
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        gameGrid = GameModeManager.instance.gameGrid;
        pawn_sprite = GameObject.Find("PlayerHero");
        pawn = pawn_sprite.AddComponent<DefaultCharacter>();
        pawn.InitChar();
        // Set the player at the starting cell
        Vector2 startingPos = gameGrid.GetCellToWorld(new Vector3Int(-3, 0, 0));
        //pawn_sprite.transform.position = new Vector3(startingPos.x, startingPos.y, 0);
        //playerPos = pawn_sprite.transform.position;

        originalTile = new Tile();
        availableTile = new Tile();
        availableTile.sprite = availableTileSprite;
        moveStat = new MovementStats(5f);
        body = pawn_sprite.GetComponent<Rigidbody2D>();
        body.gravityScale = 0.0f;
        e_playstate = PlayState.E_NONCOMBAT;
        isOver = false;
        p_animator = pawn_sprite.transform.GetChild(2).GetChild(0).GetComponent<Animator>();
    }

    void Update()
    {
        if (!pawn_sprite.activeSelf)
            return;

        if (pawn.checkIfDead())
        {
            GameModeManager.instance.showDeathScreen();
        }

        //if (e_playstate.Equals(PlayState.E_COMBAT))
        {
            //if (b_answeredCorrectly)
                // UpdateInput();

            UpdateMovement();
        }
        //else
        //{
        //    if (hasInteract())
        //    {
        //        moveStat.destinatePos = GetTouchWPos();
        //        moveStat.isMoving = true;
        //    }
        //}
    }

    void FixedUpdate()
    {
        //return;
        //if (moveStat.isMoving)
        //{
        //    bool reached = UpdateFreeMovement(moveStat.destinatePos);
        //    if (reached)
        //    {
        //        moveStat.isMoving = false;
        //        //b_answeredCorrectly = false;
        //    }
        //}
    }

    void UpdateMovement()
    {
        playerPos = pawn_sprite.transform.position;
        playerCellPos = gameGrid.GetWorldFlToCellPos(playerPos);

        Move(dpad.moveDir);
        RenderAnim();

        //if (targetTilePos != Vector2.zero)
        //{
        //    if (playerPos != targetTilePos)
        //    {
        //        pawn_sprite.transform.position = Vector2.MoveTowards(playerPos, targetTilePos, Time.deltaTime);
        //    }
        //    else
        //    {
        //        if (!b_reachedTarget)
        //        {
        //            gameGrid.SetTile(TileRefManager.TILEMAP_TYPE.TILEMAP_PLAYER, gameGrid.GetWorldFlToCellPos(playerPos), originalTile);
        //            b_shownCrossTiles = false;
        //            b_reachedTarget = true;
        //        }
        //    }
        //}
    }

    void RenderAnim()
    {
        if (dpad.moveDir == Dpad.MOVE_DIR.LEFT)
        {
            p_animator.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (dpad.moveDir == Dpad.MOVE_DIR.RIGHT)
        {
            p_animator.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (dpad.moveDir == Dpad.MOVE_DIR.DOWN)
        {
        }

        p_animator.SetFloat("NormalizeSpd", (int)dpad.moveDir);
    }

    void Move(Dpad.MOVE_DIR _moveDir)
    {
        switch (_moveDir)
        {
            case Dpad.MOVE_DIR.UP:
                targetTilePos = gameGrid.GetCellToWorld(new Vector3Int(playerCellPos.x, playerCellPos.y + 1, playerCellPos.z));
                break;
            case Dpad.MOVE_DIR.DOWN:
                targetTilePos = gameGrid.GetCellToWorld(new Vector3Int(playerCellPos.x, playerCellPos.y - 1, playerCellPos.z));
                break;
            case Dpad.MOVE_DIR.LEFT:
                targetTilePos = gameGrid.GetCellToWorld(new Vector3Int(playerCellPos.x - 1, playerCellPos.y, playerCellPos.z));
                break;
            case Dpad.MOVE_DIR.RIGHT:
                targetTilePos = gameGrid.GetCellToWorld(new Vector3Int(playerCellPos.x + 1, playerCellPos.y, playerCellPos.z));
                break;
        }

        // Move to target cell
        if (_moveDir != Dpad.MOVE_DIR.NONE)
        {
            Vector2 direction = (targetTilePos - playerPos).normalized;

            body.MovePosition((Vector2)pawn_sprite.transform.position + direction * (moveStat.moveSpeed * Time.deltaTime));
        }
    }

    bool UpdateFreeMovement(Vector3 destination)
    {
        Vector3 wpos = pawn_sprite.transform.position;
        float dist = Vector2.Distance(wpos, destination);
        float speed = (moveStat.moveSpeed * (1 / dist) * Time.deltaTime);
        if (dist > speed * 0.5f)
        {
            Vector2 direction = (destination - wpos).normalized;
            body.MovePosition((Vector2)pawn_sprite.transform.position + direction * speed);
        }
        else
            return true;
        return false;
    }

    public Vector3 GetTouchWPos()
    {
#if UNITY_EDITOR
        Vector3 temp = new Vector3(Input.mousePosition.x, Input.mousePosition.y, pawn_sprite.transform.position.z);
        return Camera.main.ScreenToWorldPoint(temp);
#elif UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase.Equals(TouchPhase.Began))
            {
                return Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            }
        }
#endif
    }

    public bool hasInteract()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) && !isOver)
        {
            //if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1) == false) // doesnt work
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // = LayerMask.NameToLayer("Tilemap");
                Vector3 t = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 fword = new Vector3(t.x, t.y, 5);
                Vector3 origin = new Vector3(t.x, t.y, -10);
                Vector3 dir = (fword - origin).normalized;
                if (Physics2D.Raycast(origin, dir, Mathf.Infinity, 1 << LayerMask.NameToLayer("Tilemap")))
                {
                    //, Mathf.Infinity, LayerMask.NameToLayer("Tilemap")
                    Debug.DrawRay(origin, 100 * dir, Color.blue, Mathf.Infinity);
                    return true;
                }
            }
        }

#elif UNITY_ANDROID
        if (Input.touchCount > 0)
        {
        return true;
        }
#endif
        return false;
    }

    public void disableInteraction()
    {
        //Debug.Log("tt");
        isOver = true;
    }

    public void enableInteraction()
    {

        //Debug.Log("ttf");
        isOver = false;
    }

    void UpdateInput()
    {
        if (NoMovesLeft())
        {
            SetCrossTiles(originalTile);
            TileRefManager.instance.GetTilemap(TileRefManager.TILEMAP_TYPE.TILEMAP_GRIDCELLS).gameObject.SetActive(false);
            b_shownCrossTiles = false;
            b_answeredCorrectly = false;
            return;
        }

#if UNITY_EDITOR
        touchPos = new Vector3Int((int)Input.mousePosition.x, (int)Input.mousePosition.y, (int)Input.mousePosition.z);
        b_touchedScreen = Input.GetMouseButtonDown(0);
        
#elif UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            //Debug.Log(Input.touchCount);
            touchPos = new Vector3Int((int)Input.GetTouch(0).position.x, (int)Input.GetTouch(0).position.y, 0);

            b_touchedScreen = (Input.GetTouch(0).tapCount == 1);
        }
        else
            b_touchedScreen = false;
#endif
        // Convert the touch position to cell position
        touchCellPos = gameGrid.GetWorldIntToCellPos(touchPos);

        if (!b_shownCrossTiles)
        {
            // Set the selected cell position
            playerCellPos = gameGrid.GetWorldFlToCellPos(playerPos);

            // Set the available tile sprites
            SetCrossTiles(availableTile);

            b_shownCrossTiles = true;
        }
        else
        {
            // Limit player movement to game area
            if (touchCellPos.y < GameGrid.minGridHeight)
                return;

            if (b_touchedScreen)
            {
                // Reset tiles if player does not move
                if (touchCellPos == playerCellPos)
                {
                    //SetCrossTiles(originalTile);
                }
                else if (TouchedAvailableTiles())
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Vector3Int tilePos = GetCrossTilesPosInOrder(i);

                        if (touchCellPos == tilePos)
                        {
                            if (!NoMovesLeft())
                            {
                                targetTilePos = gameGrid.GetCellToWorld(tilePos);
                                numAvailableMoves--;
                                gameGrid.SetTile(TileRefManager.TILEMAP_TYPE.TILEMAP_PLAYER, tilePos, availableTile);
                                b_reachedTarget = false;
                                continue;
                            }
                        }

                        gameGrid.SetTile(TileRefManager.TILEMAP_TYPE.TILEMAP_PLAYER, GetCrossTilesPosInOrder(i), originalTile);
                    }
                }
            }
        }
    }

    void SetCrossTiles(Tile _tile)
    {
        gameGrid.SetTile(TileRefManager.TILEMAP_TYPE.TILEMAP_PLAYER, GetCrossTilesPosInOrder(0), _tile);
        gameGrid.SetTile(TileRefManager.TILEMAP_TYPE.TILEMAP_PLAYER, GetCrossTilesPosInOrder(1), _tile);
        gameGrid.SetTile(TileRefManager.TILEMAP_TYPE.TILEMAP_PLAYER, GetCrossTilesPosInOrder(2), _tile);
        gameGrid.SetTile(TileRefManager.TILEMAP_TYPE.TILEMAP_PLAYER, GetCrossTilesPosInOrder(3), _tile);
    }

    bool TouchedAvailableTiles()
    {
        for (int i = 0; i < 4; i++)
        {
            if (touchCellPos == GetCrossTilesPosInOrder(i))
                return true;
        }

        return false;
    }

    Vector3Int GetCrossTilesPosInOrder(int orderNum)
    {
        switch (orderNum)
        {
            case 0: // left
                return new Vector3Int(playerCellPos.x - 1, playerCellPos.y, playerCellPos.z);
            case 1: // right
                return new Vector3Int(playerCellPos.x + 1, playerCellPos.y, playerCellPos.z);
            case 2: // up
                return new Vector3Int(playerCellPos.x, playerCellPos.y + 1, playerCellPos.z);
            case 3: // down
                return new Vector3Int(playerCellPos.x, playerCellPos.y - 1, playerCellPos.z);
            default:
                return new Vector3Int(0, 0, 0);
        }
    }

    public bool NoMovesLeft()
    {
        return (numAvailableMoves > 0);
    }

    // why all these here
    private IEnumerator healthcoroutine(float seconds, float damage)
    {
        pawn.charStat.decreaseHealth(damage);
        pawn.charStat.hpSys.isHurt = true;
        yield return new WaitForSeconds(seconds);
        pawn.charStat.hpSys.isHurt = false;
    }

    public void decreasehealthbytime(float secs, float dmg)
    {
        if (!pawn.charStat.hpSys.isHurt)
            StartCoroutine(healthcoroutine(secs, dmg));
    }

    public Vector3Int GetPlayerCellPos
    {
        get
        {
            return gameGrid.GetWorldFlToCellPos(playerPos);
        }
    }
}
