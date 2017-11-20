using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    public const int MAX_MOVES = 5;
    GameGrid gameGrid;

    [SerializeField]
    Sprite availableTileSprite;

    Tile originalTile;
    Tile availableTile;

    Vector3Int touchPos;
    Vector3Int touchCellPos;
    //Vector3Int selectedCellPos;
    Vector3Int playerCellPos;

    Vector2 playerPos;
    Vector2 targetTilePos;

    public int numMovesMade { get; set; }
    public bool b_answeredCorrectly { get; set; }
    bool b_shownCrossTiles;
    bool b_reachedTarget;
    bool b_touchedScreen;

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
        pawn_sprite.transform.position = new Vector3(startingPos.x, startingPos.y, 0);
        playerPos = pawn_sprite.transform.position;
        numMovesMade = 0;

        originalTile = new Tile();
        availableTile = new Tile();
        availableTile.sprite = availableTileSprite;
    }

    void Update()
    {
        if (pawn_sprite.activeSelf)
        {
            if (pawn.checkIfDead())
            {
                GameModeManager.instance.showDeathScreen();
            }

            if (b_answeredCorrectly)
                UpdateInput();

            UpdateMovement();
        }
    }

    void UpdateMovement()
    {
        playerPos = pawn_sprite.transform.position;

        if (targetTilePos != Vector2.zero)
        {
            if (playerPos != targetTilePos)
            {
                pawn_sprite.transform.position = Vector2.MoveTowards(playerPos, targetTilePos, Time.deltaTime);
            }
            else
            {
                if (!b_reachedTarget)
                {
                    gameGrid.SetTile(TileRefManager.TILEMAP_TYPE.TILEMAP_PLAYER, gameGrid.GetWorldFlToCellPos(playerPos), originalTile);
                    b_shownCrossTiles = false;
                    b_reachedTarget = true;
                }
            }
        }
    }

    void UpdateInput()
    {
        //Debug.Log(numMovesMade);

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
                                numMovesMade = 1;
                                gameGrid.SetTile(TileRefManager.TILEMAP_TYPE.TILEMAP_PLAYER, tilePos, availableTile);
                                b_reachedTarget = false;
                                //Debug.Log(numMovesMade);
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
        return (numMovesMade >= MAX_MOVES);
    }

    public void ResetNumMovesWhenNoneLeft()
    {
        if (NoMovesLeft())
            numMovesMade = 0;
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
            return GameModeManager.instance.gameGrid.GetWorldFlToCellPos(pawn_sprite.transform.position);
        }
    }
}
