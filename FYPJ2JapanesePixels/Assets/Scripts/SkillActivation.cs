﻿using System.Collections;
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
            if (col.gameObject.GetComponent<LanguageButton>().buttonIndex == languageSystem.theLetterIndex)
            {
                Debug.Log("CORRECT");
            }
            else
            {
                col.gameObject.GetComponent<TouchDrag>().b_ReturnToOriginalPos = true;
                Debug.Log("WRONG");
            }
        }
    }
}
