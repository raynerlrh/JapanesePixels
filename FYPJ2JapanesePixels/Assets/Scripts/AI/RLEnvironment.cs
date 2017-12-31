using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
//using System.IO;

/// <summary>
/// Trains agent with rewards for correct actions
/// </summary>

public struct EnvVars
{
    public float total_correct;
    public int num_things; // how many skills can the ai use? Ignore the name, it is the number that matters
    public int prevAction;
    public int trialsTrained;
    public bool pauseAction;
}

public class RLEnvironment : MonoBehaviour {
    public float act_speed = 0.4f;
    public EnvVars envVars;
    AgentBrain agent;
    public Agent.AgentMemory brainMemory;
    public Coroutine mainRoutine;
    public int orderIndex;
    public int playerIndex = 3;
    public Inventory inventory;

    // Use this for initialization
    void Start () {
        envVars.prevAction = -1;
        envVars.trialsTrained = 0;
        envVars.pauseAction = false;
        orderIndex = 0;
        //BeginLearning();
    }
	
	// Update is called once per frame
	void Update () {
        //if (trialsTrained >= 42)
        //{
        //    WriteString(agent.value_table);
        //    trialsTrained = 0;
        //}
	}

    /// <summary>
    /// Initialize the agent. Can be called by event when button is clicked.
    /// </summary>
    public void BeginLearning()
    {
        envVars.total_correct = 0;
        envVars.num_things = 48; // how many things to learn, in this case is 48 hiragana characters
        bool optimistic = true;
        agent = new AgentBrain(envVars.num_things, optimistic);
        //ReadString(ref agent.value_table);
        brainMemory = new Agent.AgentMemory(2);
        mainRoutine = StartCoroutine(Act());
    }

    /// <summary>
    /// Gets an action from the agent, chooses the punch accordingly,
    /// and updates the agent's value estimates based on the reward.
    /// </summary>
    public IEnumerator Act()
    {
        if (!envVars.pauseAction)
        {
            yield return new WaitForSeconds(act_speed); // wait for actSpeed seconds
            int action = agent.PickAction();
            if (envVars.prevAction == -1)
                envVars.prevAction = action;
            //char chosen = action;
            float reward = GetRewards(action, envVars.prevAction);
            envVars.prevAction = action;
            envVars.total_correct += reward;
            agent.UpdatePolicy(action, reward);
        }
    }

    public void resetagent()
    {
        bool optimistic = true;
        agent.ResetAgent(envVars.num_things, optimistic);
    }

    /// <summary>
    /// Reward ai's action if it is correct
    /// </summary>
    /// <param name="act">The action that AI made</param>
    /// <param return="reward">Reward ranges from (-1, 1)</param>
    public int GetRewards(int act, int previousAction)
    {
        int reward;
        int n = 0;
        if (GameModeManager.instance.languageSystem != null)
            n = GameModeManager.instance.languageSystem.GetRandomIndex(ref orderIndex);
        if (GameModeManager.instance.gameState == GameModeManager.GAME_STATE.IN_GAME)
        {
            envVars.pauseAction = true;
            inventory.OnHandAmount = orderIndex;
        }
        //if (Mathf.Abs(n - act) > Mathf.Abs(n - previousAction))
        //{
        //    act = previousAction;
        //}
        if (brainMemory.getLastAnswer() == n || brainMemory.doesBrainContain(n))
        {
            reward = 1;
            //languageSystem.RefreshQuestion(); // removed for now
            //EnemyMoveController.instance.currentBoss.doAttack(); // removed for now
            //Debug.Log("Guessed correctly 1");
            //env.resetagent();
            ++orderIndex;
            GameModeManager.instance.languageSystem.SetQuestionText(orderIndex, playerIndex);
        }
        else if (n == act)
        {
            reward = 1;
            //languageSystem.RefreshQuestion(); // removed for now
            if (brainMemory.GetSize == brainMemory.newMaxMemory)
            {
                brainMemory.forgetAnswer();
                brainMemory.storeAnswer(act);
            }
            else
                brainMemory.storeAnswer(act);

            ++orderIndex;
            //EnemyMoveController.instance.currentBoss.doAttack(); // removed for now
            //env.resetagent();
            GameModeManager.instance.languageSystem.SetQuestionText(orderIndex, playerIndex);
        }
        /*else if ((act > previousAction) && (n > act)) // reward for taking a step in the right direction
        {
            reward = 1;
            //Debug.Log("+1 confidence");
            ++orderIndex;
        }
        else if ((act < previousAction) && (n < act))
        {
            reward = 1;
            //Debug.Log("+1 confidence");
            ++orderIndex;
        }*/
        else
        {
            reward = -1;
            envVars.trialsTrained++;
            //Debug.Log(act);
        }
        //t_rewards.text = "Total rewards: " + rl_environment.totalRewards.ToString();
        int g = n - act;
        if (g < 0)
            g = -g;
        //if (t_progbar)
          //  t_progbar.sizeDelta = new Vector2(160 - aiprogress.t_progbarwidth * g, t_progbar.sizeDelta.y);
        StartCoroutine(GetComponent<RLEnvironment>().Act());
        //StartCoroutine(delaylearning());
        return reward;
    }

    /*
    [MenuItem("Tools/Write file")]
    static void WriteString(float[] copyarr)
    {
        string path = "Assets/Resources/test.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        for (int i = 0; i < copyarr.Length; ++i)
        {
            writer.WriteLine(copyarr[i].ToString());
        }
        writer.Close();

        //Re-import the file to update the reference in the editor
        AssetDatabase.ImportAsset(path);
        TextAsset asset = (TextAsset)Resources.Load("test");

        //Print the text from the file
        //Debug.Log(asset.text);
    }

    public void eventPressHandle()
    {
        Debug.Log("WOW! " + envVars.trialsTrained);
    }

    [MenuItem("Tools/Read file")]
    static void ReadString(ref float[] writearr)
    {
        string path = "Assets/Resources/test.txt";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        for (int i = 0; i < writearr.Length; ++i)
        {
            writearr[i] = float.Parse(reader.ReadLine());
        }
        reader.Close();
    }*/
}
