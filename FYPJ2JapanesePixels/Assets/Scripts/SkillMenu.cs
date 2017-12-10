using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillMenu : MonoBehaviour 
{
    public Transform defensiveSkills;
    public Transform offensiveSkills;
    public Text availableMovesText;

    void Start()
    {
        PlayerSkillController playerSkills = PlayerSkillController.instance;

        // Set skills 
        for (int i = 0; i < defensiveSkills.childCount; i++)
        {
            defensiveSkills.GetChild(i).GetComponent<SkillButton>().AttachSkill(playerSkills.defensiveSkills[i]);
        }
        for (int i = 0; i < offensiveSkills.childCount; i++)
        {
            offensiveSkills.GetChild(i).GetComponent<SkillButton>().AttachSkill(playerSkills.offensiveSkills[i]);
        }
    }

    void Update()
    {
        availableMovesText.text = PlayerMoveController.instance.numAvailableMoves.ToString();
    }
}
