using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LanguageSystem : MonoBehaviour 
{
    public struct Group
    {
        public string jap;
        public string[] letters;
    }

    Dictionary<int, Group> hiraganaMap;
    Group activeGroup;
    int activeGroupIndex;

    int numPlays; // a temp condition for prototype

    public Text text;
    public Transform buttons;

    public int theLetterIndex { get; set; }

    bool[] optionIndexTaken;

    bool firstOptionShown;
    int theOptionIndex;

    void Start()
    {
        hiraganaMap = new Dictionary<int, Group>();
        LoadHiragana();

        optionIndexTaken = new bool[activeGroup.letters.Length];

        DisplayQuiz();
    }

    void DisplayQuiz()
    {
        theLetterIndex = 0;//Random.Range(0, hiragana.Length);
        // get question ui and give it a hiragana character
        text.text = System.Convert.ToString(activeGroup.jap[theLetterIndex]);

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
            theOptionIndex = Random.Range(0, activeGroup.letters.Length);
            option = activeGroup.letters[theOptionIndex];
        }

        optionIndexTaken[theOptionIndex] = true;
        return option;
    }

    int GetAnswerIndexFromAnswer(string ans)
    {
        for (int i = 0; i < activeGroup.letters.Length; i++)
        {
            if (activeGroup.letters[i] == ans)
                return i;
        }

        return 0;
    }

    // Temp definition
    void LoadHiragana()
    {
        int numChar;

        Group h_0 = new Group();
        Group h_1 = new Group();
        Group h_2 = new Group();

        numChar = 5;
        for (int i = 0; i < numChar; ++i)
        {
            h_0.jap += System.Convert.ToChar(System.Convert.ToInt32('あ') + (i + i));
        }
        h_0.letters = new[] {"a", "i", "u", "e", "o"};

        numChar = 5;
        for (int i = 0; i < numChar; ++i)
        {
            h_1.jap += System.Convert.ToChar(System.Convert.ToInt32('か') + (i + i));
        }
        h_1.letters = new[] { "ka", "ki", "ku", "ke", "ko" };

        numChar = 5;
        for (int i = 0; i < numChar; ++i)
        {
            h_2.jap += System.Convert.ToChar(System.Convert.ToInt32('さ') + (i + i));
        }
        h_2.letters = new[] { "sa", "shi", "su", "se", "so" };

        /*numChar = 5;
        for (int i = 0; i < numChar; ++i)
        {
            h_3 += System.Convert.ToChar(System.Convert.ToInt32('た') + (i + i));
            //h_3 += System.Convert.ToChar(System.Convert.ToInt32('た') + 9);
        }

        numChar = 5;
        for (int i = 0; i < numChar; ++i)
        {
            h_4 += System.Convert.ToChar(System.Convert.ToInt32('な') + (i + i));
        }

        numChar = 5;
        for (int i = 0; i < numChar; ++i)
        {
            h_5 += System.Convert.ToChar(System.Convert.ToInt32('は') + (i + i));
        }

        numChar = 5;
        for (int i = 0; i < numChar; ++i)
        {
            h_6 += System.Convert.ToChar(System.Convert.ToInt32('ま') + (i + i));
        }

        numChar = 3;
        for (int i = 0; i < numChar; ++i)
        {
            h_7 += System.Convert.ToChar(System.Convert.ToInt32('や') + (i + i));
        }

        numChar = 5;
        for (int i = 0; i < numChar; ++i)
        {
            h_8 += System.Convert.ToChar(System.Convert.ToInt32('ら') + (i + i));
        }

        numChar = 2;
        for (int i = 0; i < numChar; ++i)
        {
            h_9 += System.Convert.ToChar(System.Convert.ToInt32('わ') + (i + i));
        }*/

        hiraganaMap.Add(0, h_0);
        hiraganaMap.Add(1, h_1);
        hiraganaMap.Add(2, h_2);

        activeGroupIndex = 0; // start from aiueo
        hiraganaMap.TryGetValue(activeGroupIndex, out activeGroup);

        //Group test;
        //myMap.TryGetValue(0, out test);
        //Debug.Log(test.letters[0]);
    }

    /// <summary>
    /// Give a new question in game
    /// </summary>
    public void refreshQuestion()
    {
        //if (numPlays >= 5)
        //{
        //    activeGroupIndex++;
        //    if (activeGroupIndex > 2)
        //        activeGroupIndex = 0;

        //    hiraganaMap.TryGetValue(activeGroupIndex, out activeGroup);
           
        //    firstOptionShown = false;
        //    theOptionIndex = 0;
        //    for (int i = 0; i < optionIndexTaken.Length; i++)
        //    {
        //        optionIndexTaken[i] = false;
        //    }

        //    DisplayQuiz();

        //    numPlays = 0;
        //}

        int newLetterIndex = Random.Range(0, activeGroup.letters.Length);
        while (theLetterIndex != newLetterIndex)
        {
            numPlays++;
            theLetterIndex = newLetterIndex;
            // get question ui and give it a hiragana character
            text.text = System.Convert.ToString(activeGroup.jap[theLetterIndex]);
        }
    }
}
