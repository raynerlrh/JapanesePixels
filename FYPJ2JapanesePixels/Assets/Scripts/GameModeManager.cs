using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public struct UIProp
{
    public float t_progbarwidth;
}

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager instance = null;

    public GameGrid gameGrid { get; set; }
    public LanguageSystem languageSystem;

    private RectTransform t_progbar;

    UIProp aiprogress;

    [SerializeField]
    public GameObject DeathscreenUI;
    [SerializeField]
    public GameObject WinscreenUI;
    public ItemSpawner itemSpawner;
    public Text enemyLeftTxt;
    public bool isEventStarted;
    public bool isEventFailed;
    public int num_event;
    public TimerRoutine event_timer;
    public Text event_timeText;
    public int event_timeLimit;
    public GameObject eventui_panel;
    public Canvas canvas3d;
    public AudioSource event_music;

    public enum GAME_STATE
    {
        PRE_GAME,
        IN_GAME,
        PAUSED_GAME,
        GAME_OVER,
    }

    public GAME_STATE gameState { get; set; }

    // Only for most foremost operations
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //DontDestroyOnLoad(gameObject);

        gameGrid = GameObject.Find("Grid").GetComponent<GameGrid>();

        gameState = GAME_STATE.PRE_GAME;
    }

    void Start()
    {
        GameObject progressBar = GameObject.Find("AIProgressBar");
        if (progressBar)
        {
            t_progbar = progressBar.GetComponent<RectTransform>();
            //if (aiprogress)
            aiprogress.t_progbarwidth = t_progbar.sizeDelta.x / 100;
        }
        int enemiesRemaining = GameObject.FindGameObjectsWithTag("Enemy").Length;
        enemyLeftTxt.text = enemiesRemaining.ToString();
        isEventStarted = false;
        isEventFailed = false;
        num_event = 0;
        event_timer = gameObject.AddComponent<TimerRoutine>();
        event_timeLimit = 120;
        event_timer.initTimer(event_timeLimit);
        event_timer.executedFunction = ResetEvent;
        canvas3d = GameObject.FindGameObjectWithTag("3DCanvas").GetComponent<Canvas>();
    }

    void Update()
    {
        if (event_timer.hasRun)
        {
            int left = (int)(event_timeLimit - (Time.time - event_timer.secStarted));
            event_timeText.text = left.ToString();
        }
    }

    //void ReceivePlayerChoice(bool wrong)
    //{
    //    if (wrong == false)
    //    {
    //        //GetComponent<RLEnvironment>().resetagent();
    //        //question.text.fontSize = 0;
    //        Camera.main.GetComponent<CameraController>().switchMode(CamSettings.CamMode.E_OFFSET);
    //    }
    //}

    public void RestartLevel()
    {
        if (MyNetwork.instance.IsOnlineGame())
            NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackMainMenu()
    {
        if (MyNetwork.instance.IsOnlineGame())
            NetworkManager.singleton.ServerChangeScene("MainMenu");
        else
            SceneManager.LoadScene(SceneManager.GetSceneByName("MainMenu").buildIndex);
    }

    public void ReturnLobby()
    {
        Destroy(MyNetwork.instance.localPlayer);
        Destroy(NetworkManager.singleton.gameObject);
        SceneManager.LoadScene("Lobby");

    }

    public string getSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public void showDeathScreen()
    {
        DeathscreenUI.SetActive(true);
        //if (Time.timeScale == 1)
        {
            //Time.timeScale = 0.25f;
            //Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }

    public void showWinScreen()
    {
        WinscreenUI.SetActive(true);
        //Time.timeScale = 0.25f;
        //Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    public int getEnemiesLeft()
    {
        return itemSpawner.gameCharacters.childCount - 1; // minus the player
    }

    public void ResetEvent()
    {
        if (num_event > 0)
        {
            isEventFailed = true;
            GameObject[] items = new GameObject[TileRefManager.instance.tile_dict.Count];
            TileRefManager.instance.tile_dict.Values.CopyTo(items, 0);

            TileRefManager.instance.deleteTileDict(); // must be called after values copyto
            for (int i = 0; i < items.Length; ++i)
            {
                if (items[i].GetComponent<Text>().text.Equals(languageSystem.symbolTxt.text))
                {
                    TileRefManager.instance.SetTile(TileRefManager.TILEMAP_TYPE.TILEMAP_DESTRUCTIBLE, gameGrid.GetWorldFlToCellPos(items[i].transform.position), null);
                    SpawnBomb("Prefabs/Bomb", items[i].transform.position, Quaternion.identity);
                }
                Destroy(items[i]);
            }
        }
        else
        {
            GameObject[] items = new GameObject[TileRefManager.instance.tile_dict.Count];
            TileRefManager.instance.tile_dict.Values.CopyTo(items, 0);
            TileRefManager.instance.deleteTileDict();
            for (int i = 0; i < items.Length; ++i)
                Destroy(items[i]);
        }
        num_event = 0;
        isEventStarted = false;
        isEventFailed = false;
        event_timer.hasRun = false;
        eventui_panel.SetActive(false);
        event_music.Stop();
    }

    void SpawnBomb(string _prefabsPath, Vector3 _pos, Quaternion _rot)
    {
        PlayerMoveController moveController = MyNetwork.instance.localPlayer.GetComponent<PlayerMoveController>();

        GameObject spawn = null;
        if (moveController.isServer)
            moveController.RpcSpawn(_prefabsPath, _pos, _rot, 10, true, 250, true);
        else
            moveController.CmdSpawn(_prefabsPath, _pos, _rot, 10, true, 250, true);

        if (spawn)
        {
            spawn.GetComponent<Bomb>().effectRange = 10;
            spawn.GetComponent<Bomb>().unstoppable = true;
        }
    }
}
