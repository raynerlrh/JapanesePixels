using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour 
{
    public enum ITEM_TYPE
    {
        SKILL,
        QUESTION,
    }

    public ITEM_TYPE itemType;

    Vector3Int itemCellPos;

    void Start()
    {
        itemCellPos = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.localPosition);
    }

    void Update()
    {
        if (PlayerMoveController.instance.playerCellPos == itemCellPos)
            UseItem();
    }

    void UseItem()
    {
        switch (itemType)
        {
            case ITEM_TYPE.SKILL:
                // give a randomized skill? or already randomized at the start and just give a skill

                break;
            case ITEM_TYPE.QUESTION:
                // enables quiz menu
                GameModeManager.instance.languageSystem.gameObject.SetActive(true);
                GameModeManager.instance.languageSystem.Enable();
                break;
        }

        // Remove item
        GameObject.Destroy(gameObject);
    }
}
