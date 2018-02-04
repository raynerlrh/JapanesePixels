using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageDataRetriever : MonoBehaviour 
{
    public Quiz quiz_hiragana { get; set; }
    public Quiz quiz_katakana { get; set; }

    IEnumerator WaitForRequest(WWW www, bool _isHiragana)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            if (_isHiragana)
                quiz_hiragana = JsonUtility.FromJson<Quiz>(www.text);
            else
                quiz_katakana = JsonUtility.FromJson<Quiz>(www.text);

            //Debug.Log("WWW OK!");
            //gameObject.SetActive(false);
        }
        else
        {
            //Debug.Log("WWW Error: " + www.error);
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

	void Start() 
    {
        // Set up hiragana and katakana quizzes
        string url_hiragana = "http://fyp2-japanese-pixels.appspot.com/jp_hiragana";
        WWW www_hiragana = new WWW(url_hiragana);
        StartCoroutine(WaitForRequest(www_hiragana, true));

        string url_katakana = "http://fyp2-japanese-pixels.appspot.com/jp_katakana";
        WWW www_katakana = new WWW(url_katakana);
        StartCoroutine(WaitForRequest(www_katakana, false));
	}
	
	void Update() 
    {
		
	}
}
