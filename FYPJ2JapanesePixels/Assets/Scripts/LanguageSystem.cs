using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class Quiz
{
    public LanguageData languageData;
}

[System.Serializable]
public class LanguageData
{
    public Question[] questions;
}

[System.Serializable]
public class Question
{
    public QuestionData[] questionData;
}

[System.Serializable]
public class QuestionData
{
    public string symbol;
    public string letter;
}

public class LanguageSystem : MonoBehaviour 
{
    Quiz _quiz;

    int activeQuestionGroupIndex;
    int activeQuestionIndex;

    int numPlays; // a temp condition for prototype

    public Text questionText;
    public Transform buttons;

    bool[] optionIndexTaken;

    bool firstOptionShown;
    int theOptionIndex;

    void Awake()
    {
        string url = "http://fyp2-japanese-pixels.appspot.com/jp_hiragana";
        WWW www = new WWW(url);
        StartCoroutine(WaitForRequest(www));
    }

    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            _quiz = JsonUtility.FromJson<Quiz>(www.text);

            activeQuestionGroupIndex = 0; // start from aiueo
            activeQuestionIndex = Random.Range(0, GetActiveQuestionGroup().questionData.Length);
            optionIndexTaken = new bool[GetActiveQuestionGroup().questionData.Length];

            DisplayQuiz();

            Debug.Log("WWW OK!");
            //gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }

    Question GetActiveQuestionGroup()
    {
        return _quiz.languageData.questions[activeQuestionGroupIndex];
    }

    QuestionData GetActiveQuestion()
    {
        return _quiz.languageData.questions[activeQuestionGroupIndex].questionData[activeQuestionIndex];
    }

    public int GetQuestionIndex()
    {
        return activeQuestionIndex;
    }

    void DisplayQuiz(bool b_changedQuestionGroup = true)
    {
        // get question ui and give it a hiragana character
        questionText.text = System.Convert.ToString(GetActiveQuestion().symbol);

        // Display randomized options on buttons
        for (int i = 0; i < buttons.transform.childCount; i++)
        {
            if (b_changedQuestionGroup)
            {
                // Get unused options
                string theOption = GetRandomizedOption();

                // Set the option text on buttons
                buttons.GetChild(i).GetChild(0).GetComponent<Text>().text = theOption;
            }

            // Set the button with the correct answer
            SetAnswerButton(GetActiveQuestion().letter);
        }
    }

    string GetRandomizedOption()
    {
        string option = "";

        while (!firstOptionShown || optionIndexTaken[theOptionIndex])
        {
            firstOptionShown = true;
            theOptionIndex = Random.Range(0, GetActiveQuestionGroup().questionData.Length);
            option = GetActiveQuestionGroup().questionData[theOptionIndex].letter;
        }

        optionIndexTaken[theOptionIndex] = true;
        return option;
    }

    void SetAnswerButton(string ans)
    {
        for (int i = 0; i < GetActiveQuestionGroup().questionData.Length; i++)
        {
            if (buttons.GetChild(i).GetChild(0).GetComponent<Text>().text == ans)
            {
                buttons.GetChild(i).GetComponent<LanguageButton>().b_answer = true;
                return;
            }
        }
    }

    void ResetAnswerButton()
    {
        for (int i = 0; i < GetActiveQuestionGroup().questionData.Length; i++)
        {
            buttons.GetChild(i).GetComponent<LanguageButton>().b_answer = false;
        }
    }

    /// <summary>
    /// Give a new question in game
    /// </summary>
    public void refreshQuestion()
    {
        // Change hiragana group
        if (numPlays >= 15)
        {
            activeQuestionGroupIndex++;
            if (activeQuestionGroupIndex > 2)
                activeQuestionGroupIndex = 0;

            firstOptionShown = false;
            theOptionIndex = 0;
            optionIndexTaken = new bool[GetActiveQuestionGroup().questionData.Length];

            DisplayQuiz();
            numPlays = 0;
        }
        
        int newQuestionIndex = Random.Range(0, GetActiveQuestionGroup().questionData.Length);
        while (activeQuestionIndex != newQuestionIndex)
        {
            numPlays++;
            activeQuestionIndex = newQuestionIndex;

            // Reset bools
            ResetAnswerButton();

            DisplayQuiz(false);
        }
    }
}
