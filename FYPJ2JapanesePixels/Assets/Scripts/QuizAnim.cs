using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizAnim : MonoBehaviour 
{
    bool b_showing;
    public bool b_showRewards { get; set; }

    Vector2 originalPos;
    public GameObject topBackground;
    public Text timeLeftText;
    public Text rewardsText;

    float endTimer = 0;

    void Awake()
    {
        originalPos = transform.localPosition;

        if (GameModeManager.instance.gameState == GameModeManager.GAME_STATE.PRE_GAME)
        {
            transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z);

            b_showing = true;
            topBackground.SetActive(true);
        }
    }
    
    void Update()
    {
        if (b_showRewards)
        {
            // Show rewards...
            // To do later

            endTimer += Time.deltaTime;

            if (endTimer < 2f)
            {
                timeLeftText.text = "Time's Up!";
                rewardsText.text = "";
            }
            else if (endTimer > 1f && endTimer < 5f)
                timeLeftText.text = "Rewards: ";
            else if (endTimer > 5f)
                b_showRewards = false;
        }
        else
            Animate();
    }

    void Animate()
    {
        float speed = 800f;

        if (!b_showing)
        {
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, new Vector2(0, transform.localPosition.y), speed * Time.deltaTime);

            if (transform.localPosition == new Vector3(0, transform.localPosition.y, transform.localPosition.z))
            {
                b_showing = true;
                this.enabled = false;
            }
        }
        else
        {
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, originalPos, speed * Time.deltaTime);

            if (transform.localPosition == new Vector3(originalPos.x, originalPos.y, transform.localPosition.z))
            {
                b_showing = false;

                this.enabled = false;
                GetComponent<LanguageSystem>().enabled = true;
                topBackground.SetActive(false);
                gameObject.SetActive(false);
            }
        }
    }
}
