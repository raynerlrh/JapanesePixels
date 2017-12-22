using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillMenu : MonoBehaviour 
{
    public Transform skillButtons;
    public Text availableMovesText;

    void Start()
    {
        PlayerSkillController playerSkills = PlayerSkillController.instance;

        // Set skills 
        for (int i = 0; i < skillButtons.childCount; i++)
        {
            skillButtons.GetChild(i).GetComponent<SkillButton>().AttachSkill(playerSkills.skills[i]);
        }
    }

    void Update()
    {
        availableMovesText.text = PlayerMoveController.instance.numAvailableMoves.ToString();
    }
}
