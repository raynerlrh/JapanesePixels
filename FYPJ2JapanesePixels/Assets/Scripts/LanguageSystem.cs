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
    Quiz activeQuiz;
    Quiz quiz_hiragana;
    Quiz quiz_katakana;

    [SerializeField]
    AudioPlayer audioPlayer;

    public LanguageButton theAnswerButton { get; set; }
    public Text preGameTimerText;

    int activeQuestionGroupIndex;
    int activeQuestionIndex;
    List<int> questionIndexList;

    // Players have to answer 3 correctly before question is changed
    int numCorrect;
    public int preGameNumCorrect { get; set; }

    public Transform questionText;
    public Transform buttons;

    bool[] questionIndexTaken;
    bool[] optionIndexTaken;

    bool firstQuestionIndex;
    bool firstOptionShown;
    bool b_changedQuestionGroup;
    bool b_OnSecondTake;

    int newQuestionIndex;
    int theOptionIndex;

    public int preGameTime = 1;
    float preGameTimer;

    Vector2[] questionPosArr; // prev, curr, next
    public Text symbolTxt;

    void Awake()
    {
        // Set up hiragana and katakana quizzes
        string url_hiragana = "http://fyp2-japanese-pixels.appspot.com/jp_hiragana";
        WWW www_hiragana = new WWW(url_hiragana);
        StartCoroutine(WaitForRequest(www_hiragana, true));

        string url_katakana = "http://fyp2-japanese-pixels.appspot.com/jp_katakana";
        WWW www_katakana = new WWW(url_katakana);
        StartCoroutine(WaitForRequest(www_katakana, false));
    }

    void Start()
    {
        // temp value
        preGameTime = 10;

        preGameTimer = preGameTime + 1;
        b_changedQuestionGroup = true;
        GetComponent<QuizAnim>().Init();
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
        if (!b_OnSecondTake)
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
        {
            MyNetwork.instance.localPlayer.GetComponent<PlayerMoveController>().b_inQuiz = true;
            GetComponent<QuizAnim>().enabled = true;
        }
    }

    void OnDisable()
    {
        MyNetwork.instance.localPlayer.GetComponent<PlayerMoveController>().b_inQuiz = false;
        GetComponent<QuizAnim>().enabled = true;
    }

    IEnumerator WaitForRequest(WWW www, bool _isHiragana)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            if (_isHiragana)
            {
                quiz_hiragana = JsonUtility.FromJson<Quiz>(www.text);

                activeQuiz = quiz_hiragana;

                activeQuestionGroupIndex = 0; // start from aiueo
                activeQuestionIndex = 0;
                questionIndexTaken = new bool[GetActiveQuestionGroup().questionData.Length];
                optionIndexTaken = new bool[questionIndexTaken.Length];
                newQuestionIndex = Random.Range(0, GetActiveQuestionGroup().questionData.Length);
            }
            else
                quiz_katakana = JsonUtility.FromJson<Quiz>(www.text);

            if (GameModeManager.instance.gameState == GameModeManager.GAME_STATE.PRE_GAME)
                Enable();

            //Debug.Log("WWW OK!");
            //gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }

    void Update()
    {
        if (MyNetwork.instance.IsOnlineGame())
        {
            if (!MyNetwork.instance.b_foundLocalPlayer)
                return;
        }

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
        return activeQuiz.languageData.questions[activeQuestionGroupIndex];
    }

    QuestionData GetActiveQuestion(int offset = 0)
    {
        return activeQuiz.languageData.questions[activeQuestionGroupIndex].questionData[questionIndexList[activeQuestionIndex + offset]];
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

    public int GetRandomIndex(ref int index)
    {
        if (questionIndexList.Count > 0)
        {
            if (index < questionIndexList.Count)
                return questionIndexList[index];
            index = questionIndexList.Count;
            return questionIndexList[questionIndexList.Count - 1];
        }
        else
            return questionIndexList.Count;
    }

    public void SetQuestion()
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

        //activeQuiz.languageData.questions[activeQuestionGroupIndex].questionData[questionIndexList[activeQuestionIndex + offset]];

        // Play the pronunciation for this japanese symbol
        audioPlayer.PlayJap(activeQuestionGroupIndex, questionIndexList[activeQuestionIndex]);

        if (activeQuestionIndex + 1 < questionIndexList.Count)
            GetQuestionText(2).text = System.Convert.ToString(GetActiveQuestion(1).symbol);
        else
            GetQuestionText(2).text = "";

        if (GetQuestionText(3))
            GetQuestionText(3).text = System.Convert.ToString(GetActiveQuestion(0).symbol);
        //GetQuestionText(4).text = System.Convert.ToString(GetActiveQuestion(0).symbol);
        //GetQuestionText(5).text = System.Convert.ToString(GetActiveQuestion(0).symbol);
    }

    public void SetQuestionText(int txtIndex, int playerIndex)
    {
        //GetQuestionText(3).text = System.Convert.ToString(GetActiveQuestion(0).symbol);
        ////if (txtIndex >= _quiz.languageData.questions[activeQuestionGroupIndex].questionData.Length)
        ////  txtIndex = _quiz.languageData.questions[activeQuestionGroupIndex].questionData.Length - 1;
        //if (_quiz != null && _quiz.languageData != null && _quiz.languageData.questions.Length > 0)
        //{
        //    QuestionData question = _quiz.languageData.questions[activeQuestionGroupIndex].questionData[questionIndexList[txtIndex]];
        //    if (question != null)
        //        GetQuestionText(playerIndex).text = System.Convert.ToString(question.symbol);
        //}
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

        // Plant minigame letters
        if (!GameModeManager.instance.isEventStarted && GameModeManager.instance.gameState != GameModeManager.GAME_STATE.PRE_GAME)
        {
            GameModeManager.instance.eventui_panel.SetActive(true);
            GameModeManager.instance.event_timer.executeFunction();
            GameModeManager.instance.isEventStarted = true;
            GameModeManager.instance.num_event = Random.Range(1, 3);
            GameModeManager.instance.event_music.Play();
            QuestionData[] data = GetActiveQuestionGroup().questionData;
            int randidx = Random.Range(0, data.Length);
            symbolTxt.text = data[randidx].symbol;
            TileRefManager.instance.plantLanguageTile(GameModeManager.instance.num_event, 10, data);
            GameModeManager.instance.eventui_panel.transform.GetChild(1).GetComponent<Text>().text = data[randidx].symbol;
        }

        // Change question group
        activeQuestionGroupIndex++;

        // Change from hiragana quiz to katakana quiz when hiragana is completed
        if (activeQuestionGroupIndex > activeQuiz.languageData.questions.Length - 1) // 2
        {
            activeQuiz = quiz_katakana;

            activeQuestionGroupIndex = 0; // start from aiueo
            activeQuestionIndex = 0;
        }

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
        {
            numCorrect++;

            if (GameModeManager.instance.gameState == GameModeManager.GAME_STATE.PRE_GAME)
                preGameNumCorrect++;
        }

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
            if (numCorrect == 3)
            {
                // Stop quiz once answered 3 correctly
                b_OnSecondTake = true;
                this.enabled = false;
            }
            else if (numCorrect == 6)
            {
                // Stop quiz and change hiragana group
                ChangeQuestionGroup(true);
                b_OnSecondTake = false;
            }
        }
    }
}
