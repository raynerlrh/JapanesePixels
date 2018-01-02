using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillMenu : MonoBehaviour 
{
    public Transform skillButtons;
    public Text availableMovesText;

    PlayerMoveController moveController;

    bool b_setLocalPlayer;

    void Start()
    {
        if (MyNetwork.instance.IsOnlineGame())
            return;
        if (MyNetwork.instance.localPlayer)
            SetUp();
    }

    void SetUp()
    {
        PlayerSkillController skillController = MyNetwork.instance.localPlayer.GetComponent<PlayerSkillController>();
        moveController = skillController.GetComponent<PlayerMoveController>();

        // Set skills 
        for (int i = 0; i < skillButtons.childCount; i++)
        {
            skillButtons.GetChild(i).GetComponent<SkillButton>().AttachSkill(skillController.skills[i]);
        }

        b_setLocalPlayer = true;
    }

    void Update()
    {
 
        if (!b_setLocalPlayer)
        {
            if (!MyNetwork.instance.b_foundLocalPlayer)
                return;
            if (MyNetwork.instance.localPlayer)
                SetUp();
        }

        availableMovesText.text = moveController.numAvailableMoves.ToString();
    }
}
