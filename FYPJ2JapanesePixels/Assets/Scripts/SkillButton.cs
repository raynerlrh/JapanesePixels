using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour 
{
    PlayerSkill attachedSkill;

    public void OnPress()
    {
        if (PlayerSkillController.instance.CanPerformSkill(attachedSkill))
        {
            attachedSkill.ExecuteSkill();
        }
        else
        {
            // Show text on screen "Not enough moves"
            Debug.Log("Cannot perform: " + attachedSkill.skillName);
        }
    }

    void Update()
    {
        //attachedSkill.Update();
    }

    public void AttachSkill(PlayerSkill _skill)
    {
        attachedSkill = _skill;
        SetButtonText();
    }

    void SetButtonText()
    {
        transform.GetChild(0).GetComponent<Text>().text = attachedSkill.skillName;
    }
}
