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

                // Turn on player movement grid
                TileRefManager.instance.GetTilemap(TileRefManager.TILEMAP_TYPE.TILEMAP_GRIDCELLS).gameObject.SetActive(true);
                //PlayerMoveController.instance.e_playstate = PlayerMoveController.PlayState.E_COMBAT;
                // Display respective skill menu
                if (skillType == SKILL_TYPE.TYPE_DEFENSIVE)
                    defensiveMenu.gameObject.SetActive(true);
                else
                    offensiveMenu.gameObject.SetActive(true);

                GameModeManager.instance.GetComponent<AudioPlayer>().PlayOnceTrack(1, 1);
            }
            else
            {
                // Turn off player movement grid
                TileRefManager.instance.GetTilemap(TileRefManager.TILEMAP_TYPE.TILEMAP_GRIDCELLS).gameObject.SetActive(false);

                col.gameObject.GetComponent<TouchDrag>().b_ReturnToOriginalPos = true;
                GameModeManager.instance.SendMessage("ReceivePlayerChoice", true);
                GameModeManager.instance.GetComponent<AudioPlayer>().PlayOnceTrack(0, 1);
                //languageSystem.text.gameObject.GetComponent<LanguageButton>().highlightCorrect();
            }
        }
    }
    
    /// <summary>
    /// Auto attack upon collision with enemy
    /// </summary>
    /// <param name="obj"></param>
    void OnCollisionStay2D(Collision2D obj)
    {
        if (obj.gameObject.layer == 8)
        {
            if (attackTarget == null)
            {
                if (obj.gameObject.GetComponent<Minions>())
                    attackTarget = obj.gameObject.GetComponent<Minions>().character;
                else
                    attackTarget = obj.gameObject.GetComponent<DefaultCharacter>();

            }
            attackTarget.charStat.decreaseHealth(GetComponent<CharacterStats>().attackVal);

            if (attackTarget.checkIfDead())
                attackTarget = null;
        }
    }
}
