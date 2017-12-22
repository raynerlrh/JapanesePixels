using UnityEngine;
using UnityEngine.UI;

public class LanguageButton : MonoBehaviour 
{
    public bool b_answer { get; set; }
    public float ShowAnswerHowLong = 5;
    private TimerRoutine resetColourTime;
    private Color originalColour;

    void Start()
    {
        originalColour = GetComponent<Image>().color;
        resetColourTime = gameObject.AddComponent<TimerRoutine>();
        resetColourTime.initTimer(0.8f);
        resetColourTime.executedFunction = resetColour;
    }

    void OnDisable()
    {
        resetColour();
    }

    void resetColour()
    {
        GetComponent<Image>().color = originalColour;
    }

    public void highlightCorrect()
    {
        GetComponent<Image>().color = Color.green;
        resetColourTime.executeFunction();
    }

    public void highlightWrong()
    {
        GetComponent<Image>().color = Color.red;
        resetColourTime.executeFunction();
    }
}
