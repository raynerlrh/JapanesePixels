using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnClickSingleP()
    {
        PlayerPrefs.SetString("Game_Mode", "Single_Player");
        SceneManager.LoadScene("Level1Rayner_Multiplayer");
    }

    public void OnClickMultiP()
    {
        PlayerPrefs.SetString("Game_Mode", "Online_Versus");
        SceneManager.LoadScene("Lobby");
    }
}
