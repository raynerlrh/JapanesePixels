using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerRoutine : MonoBehaviour {

    float waitSeconds;
    public void initTimer(float waitSec)
    {
        waitSeconds = waitSec;
    }
    public bool hasRun = false;
    public float secStarted;

    /// <summary>
    /// Delegatable function that can be passed to timer
    /// </summary>
    /// <param name="param">Passed to delegate to use</param>
    public delegate void Delegated();

    public Delegated executedFunction;

    private IEnumerator run()
    {
        yield return new WaitForSeconds(waitSeconds); // wait for seconds
        executedFunction();
    }

    public void executeFunction()
    {
        hasRun = true;
        secStarted = Time.time;
        StartCoroutine(run());
    }
}
