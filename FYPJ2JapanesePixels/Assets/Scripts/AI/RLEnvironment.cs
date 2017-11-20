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
    public float act_speed;
    public int prevAction;
    public int trialsTrained;
    public bool pauseAction;
}

public class RLEnvironment : MonoBehaviour {
    public EnvVars envVars;
    AgentBrain agent;
    public Agent.AgentMemory brainMemory;
    public Coroutine mainRoutine;

	// Use this for initialization
	void Start () {
        envVars.act_speed = 0.4f;
        envVars.prevAction = -1;
        envVars.trialsTrained = 0;
        envVars.pauseAction = false;
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
        brainMemory = new Agent.AgentMemory(5);
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
            yield return new WaitForSeconds(envVars.act_speed); // wait for actSpeed seconds
            int action = agent.PickAction();
            if (envVars.prevAction == -1)
                envVars.prevAction = action;
            //char chosen = action; // 
            float reward = GameModeManager.instance.GetRewards(action, envVars.prevAction);
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
