using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

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

public class PlayerMoveController : NetworkBehaviour
{
    public bool b_isHost { get; set; }

    private DefaultCharacter pawn; // main character, can be an array of pawns for each character
    private GameObject pawn_sprite; // can be an array of sprites for each character
    public DefaultCharacter GetPawn
    {
        get
        {
            return pawn;
        }
    }
    private Inventory inventory;
    public Inventory GetInventory{     get {    return inventory;    }    }
    //public const int MAX_MOVES = 5;
    GameGrid gameGrid;
    TileRefManager tileRefManager;

    Dpad dpad;
    GameObject exitRequirements;
    GameObject levelCleared;

    [SyncVar]
    Dpad.MOVE_DIR dpad_moveDir;

    [SyncVar(hook = "OnReqChanged")]
    int level_numKeysRequired;

    Tile originalTile;
    Tile availableTile;

    Vector3Int touchPos;
    Vector3Int touchCellPos;
    //Vector3Int selectedCellPos;

    //[SyncVar]
    public Vector3Int playerCellPos { get; set; }

    Vector3Int prevCellPos;

    Vector2 playerPos;
    Vector2 targetTilePos;

    Animator p_animator;

    public int numAvailableMoves { get; set; }
    public bool b_answeredCorrectly { get; set; }
    bool b_shownCrossTiles;
    bool b_reachedTarget;
    bool b_touchedScreen;

    int numKeys;

    bool b_usedKey;
    bool b_levelCleared;

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
    float levelClearedTimer;
    float checkLevelClearTimer;

    // not being called
    void OnStartLocalPlayer()
    {
        Debug.Log("Connected to server");

        gameGrid = MyNetwork.instance.localGameGrid;
        if (gameGrid == null)
            Debug.Log("NULL");
    }

    bool IsLocalPlayer()
    {
        if (MyNetwork.instance.IsOnlineGame())
        {
            return isLocalPlayer;
        }

        return true;
    }

    public void SetSinglePlayerMode()
    {
        Destroy(GetComponent<SyncTransform>());
    }

    public void ReceiveKey()
    {
        numKeys++;
    }

    void OnReqChanged(int _newNumKeysRequired)
    {
        this.level_numKeysRequired = _newNumKeysRequired;
        //level_numKeysRequired--;
        //Debug.Log("USED KEY " + level_numKeysRequired);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!IsLocalPlayer())
            return;

        if (col.gameObject.layer == 18)
        {
            // Display exit requirements
            Text requirementsText = exitRequirements.transform.GetChild(1).GetComponent<Text>();

            //int keysRemaining = level_numKeysRequired - numKeys;

            int enemiesRemaining = GameObject.FindGameObjectsWithTag("Enemy").Length;

            if (enemiesRemaining > 0)
            {
                //requirementsText.text = "Find keys to exit!    Keys remaining:  " + keysRemaining;
                requirementsText.text = "Kill remaining enemies to exit!";
            }
            else
            {
                requirementsText.text = "Level Cleared!";
                b_levelCleared = true;
                levelCleared.SetActive(true);
            }

            // Use up key
            if (numKeys > 0)
            {
                //numKeys--;
                //level_numKeysRequired--;
                //b_usedKey = true;

                //if (MyNetwork.instance.IsOnlineGame())
                //{
                //    if (isServer)
                //        RpcUpdateKeysRequired(level_numKeysRequired);
                //    else
                //        CmdUpdateKeysRequired(level_numKeysRequired);
                //}
            }

            exitRequirements.SetActive(true);
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (!IsLocalPlayer())
            return;

        if (col.gameObject.layer == 18)
        {
            exitRequirements.SetActive(false);
        }
    }

    public void Start()
    {
        if (!IsLocalPlayer())
            return;

        level_numKeysRequired = 2;

        Transform gameCharacters = GameObject.Find("GameCharacters").transform;
        transform.SetParent(gameCharacters);

        gameGrid = GameObject.Find("Grid").GetComponent<GameGrid>();
        tileRefManager = gameGrid.GetComponent<TileRefManager>();
        dpad = GameObject.Find("Dpad").GetComponent<Dpad>();
        exitRequirements = GameObject.Find("LevelExitRequirements");
        exitRequirements.SetActive(false);
        levelCleared = GameObject.Find("LevelCleared");
        levelCleared.SetActive(false);

        pawn_sprite = this.gameObject;
        pawn = pawn_sprite.AddComponent<DefaultCharacter>();
        inventory = GetComponent<Inventory>();
        
        // Set the player at the starting cell
        //Vector2 startingPos = gameGrid.GetCellToWorld(new Vector3Int(-3, 0, 0));

        originalTile = new Tile();

        moveStat = new MovementStats(5f);
        body = pawn_sprite.GetComponent<Rigidbody2D>();
        body.gravityScale = 0.0f;
        e_playstate = PlayState.E_NONCOMBAT;
        isOver = false;
        p_animator = pawn_sprite.transform.GetChild(1).GetChild(0).GetComponent<Animator>();
        prevCellPos = gameGrid.GetWorldFlToCellPos(pawn_sprite.transform.position);
        
        //tileRefManager.SetTile(TileRefManager.TILEMAP_TYPE.TILEMAP_PLAYER, prevCellPos, TileRefManager.instance.GetTileRef(TileRefManager.TILE_TYPE.TILE_WARNING));

        Debug.Log("START");
    }

    void SyncSpriteAnimations()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<NetworkIdentity>().isLocalPlayer)
                continue;

            Animator _animator = players[i].transform.GetChild(1).GetChild(0).GetComponent<Animator>();
            Dpad.MOVE_DIR _dir = players[i].GetComponent<PlayerMoveController>().dpad_moveDir;

            players[i].GetComponent<PlayerMoveController>().RenderOtherPlayersAnim(_animator, _dir);
        }
    }

    void SyncNumKeysRequired()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<NetworkIdentity>().isLocalPlayer)
                continue;

            int _numKeysReq = players[i].GetComponent<PlayerMoveController>().level_numKeysRequired;

            players[i].GetComponent<PlayerMoveController>().SetOtherPlayersNumKeysReq(_numKeysReq);
        }
    }

    bool NoEnemiesRemaining()
    {
        int enemiesRemaining = GameObject.FindGameObjectsWithTag("Enemy").Length;

        return enemiesRemaining == 0;
    }

    bool CheckIfLevelCleared()
    {
        checkLevelClearTimer += Time.deltaTime;

        float checkAfterNumSec = 1f;

        if (checkLevelClearTimer > checkAfterNumSec)
        {
            // perform check after time has passed
            if (NoEnemiesRemaining())
                return true;

            // reset timer
            checkLevelClearTimer = 0f;
        }

        return false;
    }

    void Update()
    {
        if (!IsLocalPlayer())
            return;

        if (!pawn_sprite.activeSelf)
            return;

        if (!b_levelCleared)
        {
            if (CheckIfLevelCleared())
            {
                b_levelCleared = true;
                levelCleared.SetActive(true);
            }
        }
        else
        {
            levelClearedTimer += Time.deltaTime;

            if (levelClearedTimer < 1f)
                return;

            Destroy(this.gameObject);

            if (MyNetwork.instance.IsOnlineGame())
                NetworkManager.singleton.ServerChangeScene("Level2Rayner_Multiplayer");
            else
                SceneManager.LoadScene("Level2Rayner_Multiplayer");
        }

        //if (isServer)
        //{
        //    b_usedKey = true;
        //    if (b_usedKey)
        //    {
        //        level_numKeysRequired--;

        //        if (MyNetwork.instance.IsOnlineGame())
        //        {
        //            level_numKeysRequired = 3;
        //            RpcUpdateKeysRequired(level_numKeysRequired);
        //        }

        //        b_usedKey = false;
        //    }
        //}

        //Debug.Log(level_numKeysRequired);

        if (pawn.checkIfDead())
        {
            GameModeManager.instance.showDeathScreen();
        }

        //if (e_playstate.Equals(PlayState.E_COMBAT))
        {
            //if (b_answeredCorrectly)
                // UpdateInput();

            //if (isServer)
                //RpcSyncSA();
            //else
                //CmdSyncSA();

            if (MyNetwork.instance.IsOnlineGame())
            {
                //SyncNumKeysRequired();
                SyncSpriteAnimations();
            }

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

    [Command]
    void CmdUpdateMoveDir(Dpad.MOVE_DIR _dir)
    {
        dpad_moveDir = _dir;
    }

    [Command]
    void CmdUpdateKeysRequired(int newReq)
    {
        RpcUpdateKeysRequired(newReq);
    }

    [ClientRpc]
    void RpcUpdateKeysRequired(int newReq)
    {
        //level_numKeysRequired--;
        this.level_numKeysRequired = newReq;
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

        if (prevCellPos != playerCellPos)
        {
            tileRefManager.SetTile(TileRefManager.TILEMAP_TYPE.TILEMAP_PLAYER, prevCellPos, null);
            prevCellPos = playerCellPos;
            if (tileRefManager.GetTileAtCellPos(TileRefManager.TILEMAP_TYPE.TILEMAP_PLAYER, playerCellPos) != tileRefManager.GetTileRef(TileRefManager.TILE_TYPE.TILE_WARNING))
                tileRefManager.SetTile(TileRefManager.TILEMAP_TYPE.TILEMAP_PLAYER, playerCellPos, tileRefManager.GetTileRef(TileRefManager.TILE_TYPE.TILE_WARNING));
        }

        if (MyNetwork.instance.IsOnlineGame())
        {
            if (!isServer)
                CmdUpdateMoveDir(dpad.moveDir);
            else
                dpad_moveDir = dpad.moveDir;
        }

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

    void SetOtherPlayersNumKeysReq(int _numKeysRequired)
    {
        //if (_numKeysRequired > level_numKeysRequired)
            _numKeysRequired = level_numKeysRequired;
    }

    void RenderOtherPlayersAnim(Animator _animator, Dpad.MOVE_DIR _dir)
    {
        if (_dir == Dpad.MOVE_DIR.LEFT)
        {
            _animator.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (_dir == Dpad.MOVE_DIR.RIGHT)
        {
            _animator.transform.localScale = new Vector3(-1, 1, 1);
        }

        _animator.SetFloat("NormalizeSpd", (int)_dir);
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

#elif UNITY_STANDALONE_WIN
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

        return Vector3.zero;
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

#elif UNITY_STANDALONE_WIN
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

    [Command]
    public void CmdSpawn(string _prefabsPath, Vector3 _pos, Quaternion _rot)
    {
        RpcSpawn(_prefabsPath, _pos, _rot);
    }

    [ClientRpc]
    public void RpcSpawn(string _prefabsPath, Vector3 _pos, Quaternion _rot)
    {
        GameObject _bomb = Resources.Load(_prefabsPath) as GameObject;
        GameObject _gameObject = GameObject.Instantiate(_bomb, _pos, _rot);
    }
}
