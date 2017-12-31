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
            float val = Random.Range(-15f, 15f);
            val = -2f;
            if (val > 5f && val < 15f)
            {
                GameModeManager.instance.itemSpawner.SpawnItem(transform.position, false, -5f); // give player a laughable chance to get a reward
            }
            else if (val > -5f && val < 5f)
                GameModeManager.instance.itemSpawner.SpawnItem(transform.position, true, -1f, Item.ITEM_TYPE.SKILL, Item.EFFECT_TYPE.E_EXPLOSION);
            else if (val > -15f && val < -5f)
                GameModeManager.instance.itemSpawner.SpawnItem(transform.position, true, -4f, Item.ITEM_TYPE.SKILL, Item.EFFECT_TYPE.E_HEALTH);
            DestroySelf();
        }
    }
}
