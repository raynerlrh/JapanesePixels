using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMoveController : MonoBehaviour
{
    public static PlayerMoveController instance = null;
    private PlayerPawn pawn; // main character, can be an array of pawns for each character
    private GameObject pawn_sprite; // can be an array of sprites for each character
    public PlayerPawn GetPawn
    {
        get
        {
            return pawn;
        }
    }

    GameModeManager gameManager;

    [SerializeField]
    Sprite availableTileSprite;

    Tile[] originalTiles; // Order: left, right, up, down
    Tile availableTile;
    Vector3Int touchPos;
    Vector3Int touchCellPos;
    Vector3Int selectedCellPos;

    // Only for most foremost operations
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        gameManager = GameModeManager.instance;
        pawn = new PlayerPawn();
        pawn.InitChar();
        pawn_sprite = GameObject.Find("PlayerHero");

        originalTiles = new Tile[4];
        availableTile = new Tile();
        availableTile.sprite = availableTileSprite;
    }

    void Update()
    {
        if (pawn_sprite.activeSelf)
        {
            if (pawn.checkIfDead())
            {
                pawn_sprite.SetActive(false);
            }

            UpdateInput();
        }
    }

    void UpdateInput()
    {
#if UNITY_EDITOR
        touchPos = new Vector3Int((int)Input.mousePosition.x, (int)Input.mousePosition.y, (int)Input.mousePosition.z);
#elif UNITY_ANDROID
        touchPos = new Vector3Int((int)Input.GetTouch(0).position.x, (int)Input.GetTouch(0).position.y, (int)Input.GetTouch(0).position.z);
#endif

        // Convert the touch position to cell position
        touchCellPos = gameManager.gameGrid.GetWorldToCellPos(touchPos);

        // Limit player movement to game area
        if (touchCellPos.y < gameManager.gameGrid.GetMinHeight())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            // Set the selected cell position
            selectedCellPos = touchCellPos;

            // Get the original sprite
            originalTiles[0] = gameManager.gameGrid.GetTileAtCellPos(GetCrossTilesPosInOrder(0));
            originalTiles[1] = gameManager.gameGrid.GetTileAtCellPos(GetCrossTilesPosInOrder(1));
            originalTiles[2] = gameManager.gameGrid.GetTileAtCellPos(GetCrossTilesPosInOrder(2));
            originalTiles[3] = gameManager.gameGrid.GetTileAtCellPos(GetCrossTilesPosInOrder(3));

            // Set the available tile sprites
            gameManager.gameGrid.SetTile(GetCrossTilesPosInOrder(0), availableTile);
            gameManager.gameGrid.SetTile(GetCrossTilesPosInOrder(1), availableTile);
            gameManager.gameGrid.SetTile(GetCrossTilesPosInOrder(2), availableTile);
            gameManager.gameGrid.SetTile(GetCrossTilesPosInOrder(3), availableTile);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Reset tiles if player does not move
            if (touchCellPos == selectedCellPos)
            {
                gameManager.gameGrid.SetTile(GetCrossTilesPosInOrder(0), originalTiles[0]);
                gameManager.gameGrid.SetTile(GetCrossTilesPosInOrder(1), originalTiles[1]);
                gameManager.gameGrid.SetTile(GetCrossTilesPosInOrder(2), originalTiles[2]);
                gameManager.gameGrid.SetTile(GetCrossTilesPosInOrder(3), originalTiles[3]);
            }
            else
            {
                for (int i = 0; i < originalTiles.Length; i++)
                {
                    if (touchCellPos == GetCrossTilesPosInOrder(i))
                    {
                        gameManager.gameGrid.SetTile(GetCrossTilesPosInOrder(i), availableTile);
                        continue;
                    }

                    gameManager.gameGrid.SetTile(GetCrossTilesPosInOrder(i), originalTiles[i]);
                }
            }
        }
    }

    Vector3Int GetCrossTilesPosInOrder(int orderNum)
    {
        switch (orderNum)
        {
            case 0: // left
                return new Vector3Int(selectedCellPos.x - 1, selectedCellPos.y, selectedCellPos.z);
            case 1: // right
                return new Vector3Int(selectedCellPos.x + 1, selectedCellPos.y, selectedCellPos.z);
            case 2: // up
                return new Vector3Int(selectedCellPos.x, selectedCellPos.y + 1, selectedCellPos.z);
            case 3: // down
                return new Vector3Int(selectedCellPos.x, selectedCellPos.y - 1, selectedCellPos.z);
            default:
                return new Vector3Int(0, 0, 0);
        }
    }

    // why all these here
    private IEnumerator healthcoroutine(float seconds, float damage)
    {
        pawn.decreaseHealth(damage);
        pawn.isHurt = true;
        yield return new WaitForSeconds(seconds);
        pawn.isHurt = false;
    }

    public void decreasehealthbytime(float secs, float dmg)
    {
        if (!pawn.isHurt)
            StartCoroutine(healthcoroutine(secs, dmg));
    }

    public Vector3Int GetPlayerCellPos
    {
        get
        {
            return gameManager.gameGrid.GetWorldToCellPos(pawn_sprite.transform.position);
        }
    }
}
