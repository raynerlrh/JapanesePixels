using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour 
{
    PlayerSkill attachedSkill;

    void Start()
    {
        AttachSkill(PlayerSkillController.instance.skills[1]);
    }

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
        if (attachedSkill.b_needsUpdate)
            attachedSkill.Update();
    }

    public void AttachSkill(PlayerSkill _skill)
    {
        attachedSkill = _skill;
        SetButtonText();
    }

    void SetButtonText()
    {
        //transform.GetChild(0).GetComponent<Text>().text = attachedSkill.skillName;
    }
}
