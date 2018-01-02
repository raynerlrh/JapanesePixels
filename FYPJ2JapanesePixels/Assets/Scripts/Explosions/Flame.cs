using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Flame : ObjectStats 
{
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
            if (val > 5f && val < 15f)
            {
                SpawnItem(transform.position, false, -5f); // give player a laughable chance to get a reward
            }
            else if (val > -5f && val < 5f)
                SpawnItem(transform.position, true, -1f, Item.ITEM_TYPE.SKILL, Item.EFFECT_TYPE.E_EXPLOSION);
            else if (val > -15f && val < -5f)
                SpawnItem(transform.position, true, -4f, Item.ITEM_TYPE.SKILL, Item.EFFECT_TYPE.E_HEALTH);

            // testing, need to set val condition later
            //SpawnItem(transform.position, false, 0f, Item.ITEM_TYPE.KEY);

            DestroySelf();
        }
    }

    void SpawnItem(Vector3 pos, bool chance = false, float chancenum = 2f, Item.ITEM_TYPE itype = Item.ITEM_TYPE.QUESTION, Item.EFFECT_TYPE etype = Item.EFFECT_TYPE.E_EXPLOSION)
    {
        if (chance)
        {
            float val = Random.Range(-5f, 5f);
            if (val > chancenum)
                return;
        }

        GameObject itemPrefab = Resources.Load("Prefabs/Item") as GameObject;
        GameObject obj = (GameObject)Instantiate(itemPrefab, pos, Quaternion.identity);

        Item item = obj.GetComponent<Item>();
        item.itemType = itype;
        item.effectType = etype;

        Sprite _sprite;
        switch (item.itemType)
        {
            case Item.ITEM_TYPE.SKILL:
                //{
                //    if (item.effectType == Item.EFFECT_TYPE.E_EXPLOSION)
                //        item.GetComponent<SpriteRenderer>().sprite = TileRefManager.instance.GetTileRef(TileRefManager.TILE_TYPE.TILE_PICKUP1).sprite;
                //    else
                //        item.GetComponent<SpriteRenderer>().sprite = TileRefManager.instance.GetTileRef(TileRefManager.TILE_TYPE.TILE_PICKUP2).sprite;
                //    Vector3 sizing = item.transform.localScale;
                //    item.transform.localScale = new Vector3(sizing.x * 10, sizing.y * 10, sizing.z * 10);
                //}
                break;
            case Item.ITEM_TYPE.QUESTION:
                break;
            case Item.ITEM_TYPE.KEY:
                _sprite = Resources.Load<Sprite>("Sprites/Key");
                item.GetComponent<SpriteRenderer>().sprite = _sprite;
                break;
        }
    }
}
