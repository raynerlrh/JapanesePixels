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

    void Start()
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

            Debug.Log("WWW Ok!");
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

    void DisplayQuiz()
    {
        // get question ui and give it a hiragana character
        questionText.text = System.Convert.ToString(GetActiveQuestion().symbol);

        // Display randomized options on buttons
        for (int i = 0; i < buttons.transform.childCount; i++)
        {
            // Get unused options
            string theOption = GetRandomizedOption();

            // Set the option text
            buttons.GetChild(i).GetChild(0).GetComponent<Text>().text = theOption; //System.Convert.ToString(theOption);
            
            // Set the button index
            buttons.GetChild(i).GetComponent<LanguageButton>().buttonIndex = GetAnswerIndexFromAnswer(theOption);
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

    int GetAnswerIndexFromAnswer(string ans)
    {
        for (int i = 0; i < GetActiveQuestionGroup().questionData.Length; i++)
        {
            if (GetActiveQuestion().letter == ans)
                return i;
        }

        return 0;
    }

    /// <summary>
    /// Give a new question in game
    /// </summary>
    public void refreshQuestion()
    {
        // Change hiragana group
        if (numPlays >= 5)
        {
            activeQuestionGroupIndex++;
            if (activeQuestionGroupIndex > 2)
                activeQuestionGroupIndex = 0;

            firstOptionShown = false;
            theOptionIndex = 0;
            //for (int i = 0; i < optionIndexTaken.Length; i++)
            //{
            //    optionIndexTaken[i] = false;
            //}

            optionIndexTaken = new bool[GetActiveQuestionGroup().questionData.Length];

            DisplayQuiz();

            numPlays = 0;
        }
        
        int newQuestionIndex = Random.Range(0, GetActiveQuestionGroup().questionData.Length);
        while (activeQuestionIndex != newQuestionIndex)
        {
            numPlays++;
            activeQuestionIndex = newQuestionIndex;

            // get question ui and give it a hiragana character
            questionText.text = System.Convert.ToString(GetActiveQuestion().symbol);
        }
    }
}
