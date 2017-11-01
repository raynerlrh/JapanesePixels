using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Trains agent with rewards for correct actions
/// </summary>
public class RLEnvironment : MonoBehaviour {
    float total_correct;
    int num_things; // how many skills can the ai use? Ignore the name, it is the number that matters
    float act_speed;
    AgentBrain agent;
    public Agent.AgentMemory brainMemory;
    int prevAction;
    public int trialsTrained;
	// Use this for initialization
	void Start () {
        act_speed = 0.5f;
        prevAction = -1;
        trialsTrained = 0;
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
        total_correct = 0;
        num_things = 48; // how many things to learn, in this case is 48 hiragana characters
        bool optimistic = true;
        agent = new AgentBrain(num_things, optimistic);
        //ReadString(ref agent.value_table);
        brainMemory = new Agent.AgentMemory(5);
        StartCoroutine(Act());
    }

    /// <summary>
    /// Gets an action from the agent, chooses the punch accordingly,
    /// and updates the agent's value estimates based on the reward.
    /// </summary>
    public IEnumerator Act()
    {
        yield return new WaitForSeconds(act_speed); // wait for actSpeed seconds
        int action = agent.PickAction();
        if (prevAction == -1)
            prevAction = action;
        //char chosen = action; // 
        float reward = GameModeManager.instance.GetRewards(action, prevAction);
        prevAction = action;
        total_correct += reward;
        agent.UpdatePolicy(action, reward);
    }

    public void resetagent()
    {
        bool optimistic = true;
        agent.ResetAgent(num_things, optimistic);
    }

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
        Debug.Log("WOW! " + trialsTrained);
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
    }
}
