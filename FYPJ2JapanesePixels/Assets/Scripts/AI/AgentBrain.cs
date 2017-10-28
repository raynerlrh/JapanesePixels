using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AgentBrain {
    // The vector containing the cost function values for each possible choice/hypothesis, j(action)
    public float[] value_table;
    // The rate at which to update the value estimates given a reward. It is small so that it does not jump over the best(least) cost parameter for performing a state/action
    public float learning_rate = 0.1f;//0.0005f; // Learning speed depends on learning rate

    /// <summary>
	/// Initializes a new instance of the <see cref="Agent"/> class.
	/// </summary>
	/// <param name="actionSize">Number of possible actions possible in the environment.</param>
	/// <param name="optimistic">If set to <c>true</c>, then initialize the value estimates as optimistically.</param>
	public AgentBrain(int actionSize, bool optimistic) {
		value_table = new float[actionSize]; // init vector rows
        for (int j = 0; j < actionSize; j++)
        {
            if (!optimistic)
            {
                value_table[j] = 0.0f; // access and modify matrix values
            }
            else
            {
                value_table[j] = 1.0f;
            }
        }
	}

    /// <summary>
    /// Update cost value of each action.
    /// </summary>
    /// <param name="action">The action choosen by the agent.</param>
    /// <param name="reward">The reward recieved by the agent from the environment for it's action.</param>
    // linear variate/one variable cost function is V(a) = V(a) + alpha * (r - V(a))
    // Notationally, will use Q(s, a) instead of just V(a) if there is 2 variables
    public void UpdatePolicy(int action, float reward)
    {
        value_table[action] += learning_rate * (reward - value_table[action]);
        //Debug.Log(value_table[action]);
    }

    public void UpdateGradientDescent(int action, float reward)
    {
        float temp1 = value_table[0] - learning_rate * (1 / 2 * value_table.Length) * Mathf.Pow((value_table[action] - reward), 2);
        float temp2 = value_table[1] - learning_rate * (1 / 2 * value_table.Length) * Mathf.Pow((value_table[action] - reward), 2);
        value_table[0] = temp1;
        value_table[1] = temp2;
        //Debug.Log(value_table[action]);
    }

    /// <summary>
    /// Picks an action to take given a state.
    /// </summary>
    /// <returns>The action choosen by the agent's policy</returns>
    /// <param name="state">The current environment state</param>
    public int PickAction()
    {
        float[] probs = softmax(value_table, 0.5f); // normalize between  0, 1 using softmax function
        float cumulative = 0.0f;
        int selectedElement = 0;
        float diceRoll = Random.Range(0f, 1f);
        for (int i = 0; i < value_table.Length; i++) // iterate through the possible actions AI can take
        {
            cumulative += probs[i]; /// the abs is for making the normalized value positive
            if (diceRoll < cumulative)
            {
                selectedElement = i;
                break;
            }
        }
        //Debug.Log(selectedElement);
        return selectedElement;
    }

    /// <summary>
    /// Average out with feature scaling and mean normalization.
    /// </summary>
    /// <param name="values">The values to be normalized using softmax.</param>
    /// <returns name="normal_values">A set of normalized values which sum to 1.</returns>
    // range from −0.5 ≤ x(i) ≤ 0.5
    // xi = xi−μ(mean) / number of values
    float[] mean_normalize(float[] values)
    {
        int length = values.Length; // this is actually max - min but since max is 1 and min is 0, there's no point
        float mean = 0f;
        for (int i = 0; i < values.Length; ++i)
        {
            mean += values[i]; // calculate sum of values
        }
        mean = mean / values.Length; // calculate mean
        float[] normal_values = new float[values.Length];

        for (int i = 0; i < values.Length; i++)
        {
            normal_values[i] = normal_values[i] - mean / length;
            //Debug.Log("mean: " + normal_values[i]);
        }
        return normal_values;
    }

    //We can speed up learning/gradient descent by having each of our input values in roughly the same range. 
    //This is because v(a)/θ will descend quickly on small ranges and slowly on large ranges, and so will oscillate inefficiently down to the optimum when the variables are very uneven.

    float[] softmax(float[] values, float temp)
    {
        float[] softmax_values = new float[values.Length]; // how many columns are there in each row
        float[] exp_values = new float[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            exp_values[i] = Mathf.Exp(values[i] / temp); // e is the euler's number
        }

        for (int i = 0; i < values.Length; i++)
        {
            softmax_values[i] = exp_values[i] / exp_values.Sum();
            //Debug.Log("softmax: " + softmax_values[i]);

        }
        return softmax_values;
    }

    public void ResetAgent(int actionSize, bool optimistic)
    {
        for (int j = 0; j < actionSize; j++)
        {
            if (!optimistic)
            {
                value_table[j] = 0.0f; // access and modify matrix values
            }
            else
            {
                value_table[j] = 1.0f;
            }
        }
    }
}
