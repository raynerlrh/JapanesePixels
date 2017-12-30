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
    public LanguageButton theAnswerButton { get; set; }
    public Text preGameTimerText;

    int activeQuestionGroupIndex;
    int activeQuestionIndex;
    List<int> questionIndexList;

    int numCorrect;

    public Transform questionText;
    public Transform buttons;

    bool[] questionIndexTaken;
    bool[] optionIndexTaken;

    bool firstQuestionIndex;
    bool firstOptionShown;
    bool b_changedQuestionGroup;

    int newQuestionIndex;
    int theOptionIndex;

    const int preGameTime = 1;
    float preGameTimer;

    Vector2[] questionPosArr; // prev, curr, next

    void Awake()
    {
        string url = "http://fyp2-japanese-pixels.appspot.com/jp_hiragana";
        WWW www = new WWW(url);
        StartCoroutine(WaitForRequest(www));
    }

    void Start()
    {
        preGameTimer = preGameTime + 1;
        b_changedQuestionGroup = true;
        GetComponent<QuizAnim>().enabled = false;
        questionIndexList = new List<int>();
        questionPosArr = new Vector2[questionText.childCount];

        // Store question positions;
        for (int i = 0; i < questionText.childCount; i++)
        {
            questionPosArr[i] = questionText.GetChild(i).localPosition;
        }
    }

    public void Enable()
    {
        switch (GameModeManager.instance.gameState)
        {
            case GameModeManager.GAME_STATE.PRE_GAME:

                break;
            case GameModeManager.GAME_STATE.IN_GAME:

                break;
        }

        // Get number of questions
        int numQuestions = GetActiveQuestionGroup().questionData.Length;

        // Reset values
        numCorrect = 0;
        activeQuestionIndex = 0;
        firstQuestionIndex = false;
        questionIndexTaken = new bool[numQuestions];
        questionIndexList.Clear();
        ResetAnswerButton();
        
        // Set and store randomized questions
        newQuestionIndex = Random.Range(0, numQuestions);

        while (questionIndexList.Count < numQuestions)
        {
            while (!firstQuestionIndex || questionIndexTaken[newQuestionIndex])
            {
                firstQuestionIndex = true;
                newQuestionIndex = Random.Range(0, numQuestions);
            }

            questionIndexTaken[newQuestionIndex] = true;
            questionIndexList.Add(newQuestionIndex);
        }

        DisplayQuiz();

        if (GameModeManager.instance.gameState == GameModeManager.GAME_STATE.IN_GAME)
            GetComponent<QuizAnim>().enabled = true;
    }

    void OnDisable()
    {
        GetComponent<QuizAnim>().enabled = true;
    }

    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            _quiz = JsonUtility.FromJson<Quiz>(www.text);

            activeQuestionGroupIndex = 0; // start from aiueo
            activeQuestionIndex = 0;
            questionIndexTaken = new bool[GetActiveQuestionGroup().questionData.Length];
            optionIndexTaken = new bool[questionIndexTaken.Length];
            newQuestionIndex = Random.Range(0, GetActiveQuestionGroup().questionData.Length);

            if (GameModeManager.instance.gameState == GameModeManager.GAME_STATE.PRE_GAME)
                Enable();

            Debug.Log("WWW OK!");
            //gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }

    void Update()
    {
        if (!MyNetwork.instance.b_foundLocalPlayer)
            return;

        if (GameModeManager.instance.gameState == GameModeManager.GAME_STATE.PRE_GAME)
        {
            preGameTimer -= Time.deltaTime;

            if (preGameTimer < 1f)
            {
                GameModeManager.instance.gameState = GameModeManager.GAME_STATE.IN_GAME;
                GetComponent<QuizAnim>().b_showRewards = true;
                this.enabled = false;
            }
            else
                preGameTimerText.text = ((int)preGameTimer).ToString() + "s";
        }
    }

    Question GetActiveQuestionGroup()
    {
        return _quiz.languageData.questions[activeQuestionGroupIndex];
    }

    QuestionData GetActiveQuestion(int offset = 0)
    {
        return _quiz.languageData.questions[activeQuestionGroupIndex].questionData[questionIndexList[activeQuestionIndex + offset]];
    }

    public int GetQuestionIndex()
    {
        return activeQuestionIndex;
    }

    Text GetQuestionText(int index)
    {
        if (index >= questionText.childCount)
            return null;

        return questionText.transform.GetChild(index).GetComponent<Text>();
    }

    void SetQuestion()
    {
        Vector2 targetPos;

        //if (numPlays == 0)
        //{
        //    GetQuestionText(1).text = System.Convert.ToString(GetActiveQuestion().symbol);
        //    GetQuestionText(2).text = System.Convert.ToString(GetActiveQuestion(1).symbol);
        //}
        //else
        //{
        //    targetPos = questionPosArr[1];
        //}

        if (activeQuestionIndex != 0)
            GetQuestionText(0).text = System.Convert.ToString(GetActiveQuestion(-1).symbol);
        else
            GetQuestionText(0).text = "";

        GetQuestionText(1).text = System.Convert.ToString(GetActiveQuestion().symbol);

        if (activeQuestionIndex + 1 < questionIndexList.Count)
            GetQuestionText(2).text = System.Convert.ToString(GetActiveQuestion(1).symbol);
        else
            GetQuestionText(2).text = "";
    }

    void DisplayQuiz()
    {
        // Set question
        SetQuestion();

        // Display randomized options on buttons
        if (b_changedQuestionGroup)
        {
            for (int i = 0; i < buttons.transform.childCount; i++)
            {
                // Get unused options
                string theOption = GetRandomizedOption();

                // Set the option text on buttons
                buttons.GetChild(i).GetChild(0).GetComponent<Text>().text = theOption;
            }

            b_changedQuestionGroup = false;
        }

        // Set the button with the correct answer
        SetAnswerButton(GetActiveQuestion().letter);
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
                theAnswerButton = buttons.GetChild(i).GetComponent<LanguageButton>();
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

        theAnswerButton = null;
    }

    void ChangeQuestionGroup(bool _stopQuiz)
    {
        b_changedQuestionGroup = true;
        questionIndexList.Clear();

        activeQuestionGroupIndex++;
        if (activeQuestionGroupIndex > 2)
            activeQuestionGroupIndex = 0;

        firstQuestionIndex = false;
        questionIndexTaken = new bool[GetActiveQuestionGroup().questionData.Length];
        firstOptionShown = false;
        optionIndexTaken = new bool[GetActiveQuestionGroup().questionData.Length];
        theOptionIndex = 0;

        numCorrect = 0;

        if (_stopQuiz)
            this.enabled = false;
    }

    /// <summary>
    /// Give a new question in game
    /// </summary>
    public void RefreshQuestion(bool _correct)
    {
        if (_correct)
            numCorrect++;

        // Change questions
        if (activeQuestionIndex + 1 < questionIndexList.Count)
        {
            activeQuestionIndex++;
        }
        else
        {
            if (GameModeManager.instance.gameState == GameModeManager.GAME_STATE.PRE_GAME)
            {
                ChangeQuestionGroup(false);
                Enable();
            }
            else if (GameModeManager.instance.gameState == GameModeManager.GAME_STATE.IN_GAME)
            {
                this.enabled = false;
                return;
            }
        }
        
        ResetAnswerButton();
        DisplayQuiz();

        if (GameModeManager.instance.gameState == GameModeManager.GAME_STATE.PRE_GAME)
        {
            if (numCorrect == 3)
                ChangeQuestionGroup(false);
        }
        else if (GameModeManager.instance.gameState == GameModeManager.GAME_STATE.IN_GAME)
        {
            // Stop quiz and change hiragana group
            if (numCorrect == 3)
                ChangeQuestionGroup(true);
        }
    }
}
