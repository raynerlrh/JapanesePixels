using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private GameObject b1;
    private GameObject m1;
    private GameObject b2;
    private GameObject m2;

    private void Start()
    {
        b1 = transform.GetChild(0).gameObject;
        m1 = transform.GetChild(1).gameObject;
        b2 = transform.GetChild(3).gameObject;
        m2 = transform.GetChild(4).gameObject;
    }

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

    public void OnClickLvl1()
    {
        PlayerPrefs.SetString("Game_Mode", "Online_Versus");
        SceneManager.LoadScene("Level1Sam 1");
    }

    public void OnClickLvl2()
    {
        PlayerPrefs.SetString("Game_Mode", "Online_Versus");
        SceneManager.LoadScene("Level2");
    }

    public void OnClickSelectLv()
    {
        b1.SetActive(false);
        b2.SetActive(true);
        m1.SetActive(false);
        m2.SetActive(true);
    }

    public void BackTrack()
    {
        b1.SetActive(true);
        b2.SetActive(false);
        m1.SetActive(true);
        m2.SetActive(false);
    }

    public void OnClickCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
