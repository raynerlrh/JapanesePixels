using UnityEngine;
using UnityEngine.UI;

public class testButton : MonoBehaviour {

    public Test globalBaby;
    public int buttonIndex { get; set; }

	// Use this for initialization
	void Start () {
        //Debug.Log(buttonIndex);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnClick()
    {
        //Debug.Log(globalBaby.theLetterIndex);
        //Debug.Log(buttonIndex);
        if (buttonIndex == globalBaby.theLetterIndex)
            Debug.Log("CORRECT");
        else
            Debug.Log("WRONG");
    }
}
