using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Test : MonoBehaviour 
{
    char[] hiragana;
    char[] answers;

    public Text text;
    public Transform buttons;

    public int theLetterIndex { get; set; }

    bool[] optionIndexTaken;

    bool firstOptionShown;
    int theOptionIndex;

    void Start()
    {
        hiragana = new char[5];
        //hiragana[0] = System.Convert.ToChar("3041");
        //char newchar = System.Convert.ToChar(3041);
        //Debug.Log(newchar);
        //hiragana[0] = 'あ';
        //hiragana[1] = 'い';
        //hiragana[2] = 'う';
        //hiragana[3] = 'え';
        //hiragana[4] = 'お';

        //Debug.Log(System.Convert.ToInt32('あ'));
        
        for (int i = 0; i < 5; ++i )
        {
            hiragana[i] = System.Convert.ToChar(System.Convert.ToInt32('あ') + (i + i));
        }

        answers = new char[5];
        answers[0] = 'a';
        answers[1] = 'i';
        answers[2] = 'u';
        answers[3] = 'e';
        answers[4] = 'o';

        optionIndexTaken = new bool[answers.Length];

        //for (int i = 0; i < hiragana.Length; i++)
        //{
        //    text.text += hiragana[i] + " ";
        //}

        theLetterIndex = Random.Range(0, hiragana.Length);
        // get question panel and give it a hiragana character
        text.text = System.Convert.ToString(hiragana[theLetterIndex]);

        // Display randomized options on buttons
        for (int i = 0; i < buttons.transform.childCount; i++)
        {
            char theOption = GetRandomizedOption();
            buttons.GetChild(i).GetChild(0).GetComponent<Text>().text = System.Convert.ToString(theOption);
            buttons.GetChild(i).GetComponent<testButton>().buttonIndex = GetAnswerIndexFromAnswer(theOption);
        }
    }

    char GetRandomizedOption()
    {
        char option = ' ';

        while (!firstOptionShown || optionIndexTaken[theOptionIndex])
        {
            firstOptionShown = true;
            theOptionIndex = Random.Range(0, answers.Length);
            option = answers[theOptionIndex];
        }

        optionIndexTaken[theOptionIndex] = true;
        return option;
    }

    int GetAnswerIndexFromAnswer(char ans)
    {
        for (int i = 0; i < answers.Length; i++)
        {
            if (answers[i] == ans)
                return i;
        }

        return 0;
    }
}
