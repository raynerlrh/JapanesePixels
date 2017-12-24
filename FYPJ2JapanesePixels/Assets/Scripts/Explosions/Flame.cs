using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Flame : ObjectStats {
    TimerRoutine burnout;
	// Use this for initialization
	void Start () {
        burnout = gameObject.AddComponent<TimerRoutine>();
        burnout.initTimer(0.3f);
        burnout.executedFunction = DestroySelf;
        burnout.executeFunction();
	}

    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.gameObject.layer == 14)
        {
            if (obj.GetComponent<CharacterStats>())
            {
                obj.GetComponent<CharacterStats>().decreaseHealth(damage);
                DestroySelf();
            }
        }
        if (obj.gameObject.layer == 17)
        {
            Vector3Int cellpos = GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.position);
            TileRefManager.instance.SetTile(TileRefManager.TILEMAP_TYPE.TILEMAP_DESTRUCTIBLE, cellpos, null);
            DestroySelf();
        }
    }
}
