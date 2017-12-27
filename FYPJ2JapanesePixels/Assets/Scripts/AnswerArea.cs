using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerArea : MonoBehaviour 
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "AnswerOption")
        {
            if (col.gameObject.GetComponent<LanguageButton>().b_answer)
            {
                GameModeManager.instance.languageSystem.RefreshQuestion(true);
                GameModeManager.instance.SendMessage("ReceivePlayerChoice", false);

                PlayerMoveController moveController = MyNetwork.instance.localPlayer.GetComponent<PlayerMoveController>();

                moveController.b_answeredCorrectly = true;
                moveController.numAvailableMoves++;

                // Turn on player movement grid
                //TileRefManager.instance.GetTilemap(TileRefManager.TILEMAP_TYPE.TILEMAP_GRIDCELLS).gameObject.SetActive(true);
                moveController.e_playstate = PlayerMoveController.PlayState.E_COMBAT;

                col.gameObject.GetComponent<TouchDrag>().b_ReturnToOriginalPos = true;
                GameModeManager.instance.GetComponent<AudioPlayer>().PlayOnceTrack(1, 1);
            }
            else
            {
                col.gameObject.GetComponent<LanguageButton>().highlightWrong();
                GameModeManager.instance.languageSystem.theAnswerButton.highlightCorrect();
                GameModeManager.instance.languageSystem.RefreshQuestion(false);

                // Turn off player movement grid
                //TileRefManager.instance.GetTilemap(TileRefManager.TILEMAP_TYPE.TILEMAP_GRIDCELLS).gameObject.SetActive(false);

                col.gameObject.GetComponent<TouchDrag>().b_ReturnToOriginalPos = true;
                GameModeManager.instance.SendMessage("ReceivePlayerChoice", true);
                GameModeManager.instance.GetComponent<AudioPlayer>().PlayOnceTrack(0, 1);
            }
        }
    }
}
