using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillActivation : MonoBehaviour 
{
    public LanguageSystem languageSystem;

    public enum SKILL_TYPE
    {
        TYPE_DEFENSIVE,
        TYPE_OFFENSIVE,
    }

    public SKILL_TYPE skillType;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "AnswerOption")
        {
            LanguageButton button = col.gameObject.GetComponent<LanguageButton>();

            if (button.buttonIndex == languageSystem.theLetterIndex)
            {
                Debug.Log("CORRECT");
            }
            else
            {
                button.b_ReturnToOriginalPos = true;
                Debug.Log("WRONG");
            }
        }
    }
}
