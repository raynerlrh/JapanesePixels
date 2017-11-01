using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : MonoBehaviour 
{
    public static GameModeManager instance = null;

    private PlayerPawn mainchar;

    public GameGrid gameGrid { get; set; }
    public LanguageSystem question;

    // Only for most foremost operations
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

	void Start () 
    {
        gameGrid = GetComponent<GameGrid>();
        mainchar = PlayerMoveController.instance.GetPawn;

        question = GameObject.Find("Canvas").GetComponent<LanguageSystem>();
        GetComponent<RLEnvironment>().BeginLearning();
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
        if (n == act)
        {
            reward = 1;
            Debug.Log("Guessed correctly");
            question.refreshQuestion();
            GetComponent<RLEnvironment>().resetagent();
        }
        else if ((act > previousAction) && (n > act))
        {
            reward = 1;
        }
        else if ((act < previousAction) && (n < act))
        {
            reward = 1;
        }
        else
        {
            reward = -1;
            //Debug.Log(act);
            //Debug.Log(n);
        }
        //t_rewards.text = "Total rewards: " + rl_environment.totalRewards.ToString();
        StartCoroutine(GetComponent<RLEnvironment>().Act());
        //StartCoroutine(delaylearning());
        return reward;
    }

    void ReceivePlayerChoice(bool wrong)
    {
        if (wrong == false)
        {
            GetComponent<RLEnvironment>().resetagent();
            Debug.Log("shitt");
        }
    }
}
