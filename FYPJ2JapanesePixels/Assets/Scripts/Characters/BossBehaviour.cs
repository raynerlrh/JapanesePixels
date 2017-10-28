using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : DefaultCharacter {
    float numMinions;
    public bool summonMinions;

	public override void InitChar () {
        base.InitChar();
        numMinions = 4;
        summonMinions = false;
	}
	
	public void lightAttack() {
		if (summonMinions)
        {
            for (int i = 0; i < numMinions; ++i)
            {
                Vector3Int cellpos = GameModeManager.instance.GetTopRightCell();
                cellpos = new Vector3Int(cellpos.x, cellpos.y - i * 2, cellpos.z);

                Vector3 temp = GameModeManager.instance.GetCellWPOS(cellpos);
                GameObject summon = GameObject.Instantiate(EnemyMoveController.instance.enemyPrefabs[0], EnemyMoveController.instance.GetBossObj.transform, false);
                summon.transform.position = temp;
                summon.SetActive(true);
            }
            summonMinions = false;
        }
	}
}
