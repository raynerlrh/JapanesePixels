﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct UIProp
{
    public float t_progbarwidth;
}

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager instance = null;

    public GameGrid gameGrid { get; set; }
    public LanguageSystem question;

    private RectTransform t_progbar;

    UIProp aiprogress;

    [SerializeField]
    public GameObject DeathscreenUI;
    [SerializeField]
    public GameObject WinscreenUI;

    // Only for most foremost operations
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //DontDestroyOnLoad(gameObject);

        gameGrid = GameObject.Find("Grid").GetComponent<GameGrid>();
    }

    void Start()
    {

        question = GameObject.Find("Canvas").GetComponent<LanguageSystem>();
        GetComponent<RLEnvironment>().BeginLearning();
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

    /// <summary>
    /// Reward ai's action if it is correct
    /// </summary>
    /// <param name="act">The action that AI made</param>
    /// <param return="reward">Reward ranges from (-1, 1)</param>
    public int GetRewards(int act, int previousAction)
    {
        int reward;
        int n = question.theLetterIndex + 1;
        RLEnvironment env = GetComponent<RLEnvironment>();
        //if (Mathf.Abs(n - act) > Mathf.Abs(n - previousAction))
        //{
        //    act = previousAction;
        //}
        if (env.brainMemory.getLastAnswer() == n || env.brainMemory.doesBrainContain(n))
        {
            reward = 1;
            question.refreshQuestion();
            EnemyMoveController.instance.currentBoss.doAttack();
            //Debug.Log("Guessed correctly 1");
            //env.resetagent();
        }
        else if (n == act)
        {
            reward = 1;
            question.refreshQuestion();
            if (env.brainMemory.GetSize == env.brainMemory.newMaxMemory)
            {
                env.brainMemory.forgetAnswer();
                env.brainMemory.storeAnswer(act);
            }
            else
                env.brainMemory.storeAnswer(act);

            EnemyMoveController.instance.currentBoss.doAttack();
            //env.resetagent();
        }
        else if ((act > previousAction) && (n > act))
        {
            reward = 1;
            //Debug.Log("+1 confidence");
        }
        else if ((act < previousAction) && (n < act))
        {
            reward = 1;
            //Debug.Log("+1 confidence");
        }
        else
        {
            reward = -1;
            env.envVars.trialsTrained++;
            //Debug.Log(act);
        }
        //t_rewards.text = "Total rewards: " + rl_environment.totalRewards.ToString();
        int g = n - act;
        if (g < 0)
            g = -g;
        if (t_progbar)
            t_progbar.sizeDelta = new Vector2(160 - aiprogress.t_progbarwidth * g, t_progbar.sizeDelta.y);
        StartCoroutine(GetComponent<RLEnvironment>().Act());
        //StartCoroutine(delaylearning());
        return reward;
    }

    void ReceivePlayerChoice(bool wrong)
    {
        if (wrong == false)
        {
            //GetComponent<RLEnvironment>().resetagent();
            question.text.fontSize = 0;
            Camera.main.GetComponent<CameraController>().switchMode(CamSettings.CamMode.E_OFFSET);
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
