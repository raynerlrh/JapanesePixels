using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempMenu : MonoBehaviour 
{
	void Start() 
    {
        // Modes:
        // "Single_Player"
        // "Online_Versus"
        // "Online_Coop"
        PlayerPrefs.SetString("Game_Mode", "Online_Versus");




        SceneManager.LoadScene("Level1Rayner_Multiplayer");
	}
}
