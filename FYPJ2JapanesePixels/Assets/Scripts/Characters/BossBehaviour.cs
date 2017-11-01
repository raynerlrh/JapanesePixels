using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : DefaultCharacter 
{
    float numMinions;
    public bool summonMinions;
    TimerRoutine dehighlightTimer;

	public override void InitChar() 
    {
        base.InitChar();
        numMinions = 4;
        summonMinions = false;
        dehighlightTimer = EnemyMoveController.instance.gameObject.AddComponent<TimerRoutine>();
        dehighlightTimer.initTimer(2);
        dehighlightTimer.executedFunction = new TimerRoutine.Delegated(dehighlightAttackGrid);
	}
	
    public void highlightAttackGrid()
    {
        GameObject highlightMap = GameModeManager.instance.gameGrid.getTilemap(1).gameObject;
        for (int i = 0; i < numMinions; ++i)
        {
            Vector3Int cellpos = GameModeManager.instance.gameGrid.GetTopRightCell();
            cellpos = new Vector3Int(cellpos.x, cellpos.y - i * 2, cellpos.z);
            for (int j = cellpos.x; j >= cellpos.x - 7; --j)
            {
                Vector3Int highlightCell = new Vector3Int(j, cellpos.y, cellpos.z);
                GameModeManager.instance.gameGrid.SetTile(highlightMap, highlightCell, TileRefManager.instance.GetTileRef(1));
            }
        }
    }

    public void dehighlightAttackGrid()
    {
        GameObject highlightMap = GameModeManager.instance.gameGrid.getTilemap(1).gameObject;
        for (int i = 0; i < numMinions; ++i)
        {
            Vector3Int cellpos = GameModeManager.instance.gameGrid.GetTopRightCell();
            cellpos = new Vector3Int(cellpos.x, cellpos.y - i * 2, cellpos.z);
            for (int j = cellpos.x; j >= cellpos.x - 7; --j)
            {
                Vector3Int highlightCell = new Vector3Int(j, cellpos.y, cellpos.z);
                GameModeManager.instance.gameGrid.SetTile(highlightMap, highlightCell, null);
            }
        }
    }

	public void lightAttack() 
    {
		if (summonMinions)
        {
            for (int i = 0; i < numMinions; ++i)
            {
                Vector3Int cellpos = GameModeManager.instance.gameGrid.GetTopRightCell();
                cellpos = new Vector3Int(cellpos.x, cellpos.y - i * 2, cellpos.z);
                Vector3 temp = GameModeManager.instance.gameGrid.GetCellWPOS(cellpos);
                GameObject summon = GameObject.Instantiate(EnemyMoveController.instance.enemyPrefabs[0], EnemyMoveController.instance.GetBossObj.transform, false);
                summon.transform.position = temp;
                dehighlightTimer.executeFunction();
                summon.SetActive(true);
            }
            summonMinions = false;
        }
	}
}
