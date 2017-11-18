using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour 
{
    PlayerSkill attachedSkill;

    public void OnPress()
    {
        attachedSkill.ExecuteSkill();
        PlayerMoveController.instance.MadeMove();
    }

    public void SetSkill(PlayerSkill _skill)
    {
        attachedSkill = _skill;
        SetButtonText();
    }

    void SetButtonText()
    {
        transform.GetChild(0).GetComponent<Text>().text = attachedSkill.skillName;
    }
}
