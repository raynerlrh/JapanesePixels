using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeManager : MonoBehaviour 
{
    public static GameModeManager instance = null;

    private PlayerPawn mainchar;

    public GameGrid gameGrid { get; set; }
    public LanguageSystem question;

    private RectTransform t_progbar;
    private float t_progbarwidth;

    // Only for most foremost operations
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        gameGrid = GetComponent<GameGrid>();
    }

	void Start () 
    {
        mainchar = PlayerMoveController.instance.GetPawn;

        question = GameObject.Find("Canvas").GetComponent<LanguageSystem>();
        GetComponent<RLEnvironment>().BeginLearning();

        t_progbar = GameObject.Find("AIProgressBar").GetComponent<RectTransform>();
        t_progbarwidth = t_progbar.sizeDelta.x / 100;
	}
	
	void Update () 
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
            env.trialsTrained++;
            //Debug.Log(act);
        }
        //t_rewards.text = "Total rewards: " + rl_environment.totalRewards.ToString();
        int g = n - act;
        if (g < 0)
            g = -g;
        t_progbar.sizeDelta = new Vector2(160 - t_progbarwidth * g, t_progbar.sizeDelta.y);
        StartCoroutine(GetComponent<RLEnvironment>().Act());
        //StartCoroutine(delaylearning());
        return reward;
    }

    void ReceivePlayerChoice(bool wrong)
    {
        if (wrong == false)
        {
            GetComponent<RLEnvironment>().resetagent();
        }
    }
}
