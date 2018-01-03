using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

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
    }

    void Update()
    {

    }

    void ReceivePlayerChoice(bool wrong)
    {
        if (wrong == false)
        {
            //GetComponent<RLEnvironment>().resetagent();
            //question.text.fontSize = 0;
            Camera.main.GetComponent<CameraController>().switchMode(CamSettings.CamMode.E_OFFSET);
        }
    }

    public void RestartLevel()
    {
        if (MyNetwork.instance.IsOnlineGame())
            NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

}
