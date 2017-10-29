using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : MonoBehaviour {
    public static GameModeManager instance = null;
    private PlayerPawn mainchar;
    private Grid world_grid;
    public Vector2 camTopRight;
    public LanguageSystem question;
    public float mapWidthX;

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
