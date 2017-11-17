using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillActivation : MonoBehaviour
{
    public LanguageSystem languageSystem;
    // The char being targeted so that attacks are focused on target
    private DefaultCharacter attackTarget;

    public enum SKILL_TYPE
    {
        TYPE_DEFENSIVE,
        TYPE_OFFENSIVE,
    }

    public SKILL_TYPE skillType;

    public SkillMenu defensiveMenu;
    public SkillMenu offensiveMenu;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "AnswerOption")
        {
            if (col.gameObject.GetComponent<LanguageButton>().buttonIndex == languageSystem.theLetterIndex)
            {
                languageSystem.refreshQuestion();
                GameModeManager.instance.SendMessage("ReceivePlayerChoice", false);
                PlayerMoveController.instance.b_answeredCorrectly = true;
                PlayerMoveController.instance.ResetNumMovesWhenNoneLeft();

                if (skillType == SKILL_TYPE.TYPE_DEFENSIVE)
                    defensiveMenu.gameObject.SetActive(true);
                else
                    offensiveMenu.gameObject.SetActive(true);
            }
            else
            {
                col.gameObject.GetComponent<TouchDrag>().b_ReturnToOriginalPos = true;
                GameModeManager.instance.SendMessage("ReceivePlayerChoice", true);
            }
        }
    }
    
    void OnCollisionStay2D(Collision2D obj)
    {
        if (obj.gameObject.layer == 8)
        {
            if (attackTarget == null)
                attackTarget = obj.gameObject.GetComponent<Minions>().char_stat;
            attackTarget.decreaseHealth(10);

            if (attackTarget.checkIfDead())
                attackTarget = null;
        }
    }
}
