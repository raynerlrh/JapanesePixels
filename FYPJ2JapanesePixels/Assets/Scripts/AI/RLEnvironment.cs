using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Trains agent with rewards for correct actions
/// </summary>
public class RLEnvironment : MonoBehaviour {
    float total_correct;
    int num_things; // how many skills can the ai use? Ignore the name, it is the number that matters
    float act_speed;
    AgentBrain agent;
    int prevAction;
	// Use this for initialization
	void Start () {
        act_speed = 0.5f;
        prevAction = -1;
	}
	
	// Update is called once per frame
	void Update () {
		
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
}
