﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour 
{
    PlayerSkill attachedSkill;

    bool b_setLocalPlayer;

    void Start()
    {
        if (MyNetwork.instance.IsOnlineGame())
            return;

        SetUp();
    }

    void SetUp()
    {
        PlayerSkillController skillController = MyNetwork.instance.localPlayer.GetComponent<PlayerSkillController>();

        AttachSkill(skillController.skills[1]);
        b_setLocalPlayer = true;
    }

    public void OnPress()
    {
        PlayerSkillController skillController = MyNetwork.instance.localPlayer.GetComponent<PlayerSkillController>();

        if (skillController.CanPerformSkill(attachedSkill))
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
        if (!b_setLocalPlayer)
        {
            if (!MyNetwork.instance.b_foundLocalPlayer)
                return;

            SetUp();
        }

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
