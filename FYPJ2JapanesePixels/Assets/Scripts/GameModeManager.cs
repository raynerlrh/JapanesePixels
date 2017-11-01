using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeManager : MonoBehaviour {
    public static GameModeManager instance = null;
    private PlayerPawn mainchar;
    private Grid world_grid;
    public Vector2 camTopRight;
    public LanguageSystem question;
    public float mapWidthX;
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
    }
	// Use this for initialization
	void Start () {
        mainchar = PlayerMoveController.instance.GetPawn;
        world_grid = GameObject.Find("Grid").GetComponent<Grid>();
        camTopRight.y = Camera.main.transform.position.y + (Camera.main.orthographicSize);
        camTopRight.x = (Camera.main.orthographicSize) * Camera.main.aspect;
        question = GameObject.Find("Canvas").GetComponent<LanguageSystem>();
        GetComponent<RLEnvironment>().BeginLearning();
        mapWidthX = GetTopRightCell().x - 7;
        mapWidthX = GetCellWPOS(new Vector3Int((int)mapWidthX, GetTopRightCell().y, GetTopRightCell().z)).x;
        t_progbar = GameObject.Find("AIProgressBar").GetComponent<RectTransform>();
        t_progbarwidth = t_progbar.sizeDelta.x / 100;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Vector3 GetTopRightGridWPOS()
    {
        Vector3Int cellpos = world_grid.WorldToCell(camTopRight);
        return world_grid.CellToWorld(cellpos);
    }

    public Vector3Int GetTopRightCell()
    {
        Vector3Int temppos = world_grid.WorldToCell(camTopRight);
        return new Vector3Int(temppos.x, temppos.y - 1, temppos.z);
    }

    public Vector3 GetCellWPOS(Vector3Int cellpos)
    {
        return world_grid.CellToWorld(cellpos);
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
        if (env.brainMemory.getLastAnswer() == n)
        {
            reward = 1;
            question.refreshQuestion();
            //Debug.Log("Guessed correctly 1");
            //env.resetagent();
        }
        else if (env.brainMemory.doesBrainContain(n))
        {
            reward = 1;
            question.refreshQuestion();
            //Debug.Log("Guessed correctly 2");
            //env.resetagent();
        }
        else if (n == act)
        {
            reward = 1;
            //Debug.Log("Guessed correctly");
            question.refreshQuestion();
            if (env.brainMemory.GetSize == env.brainMemory.newMaxMemory)
            {
                env.brainMemory.forgetAnswer();
                env.brainMemory.storeAnswer(act);
            }
            else
                env.brainMemory.storeAnswer(act);
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
