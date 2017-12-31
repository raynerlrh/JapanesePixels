using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Temporary definition
public class ItemSpawner : NetworkBehaviour 
{
    public GameObject itemPrefab;
    public int numberOfItems;

    // This will be the start function since without the NetworkManager in single player mode, this game object will be set to inactive
    // For use for single player stuff only
    void OnDisable()
    {
        if (MyNetwork.instance.IsOnlineGame())
            return;

        var spawnPosition = new Vector3(-0.52f, 1.52f, 1f);

        var item = (GameObject)Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
    }

    // For use for multiplayer stuff only
    public override void OnStartServer()
    {
        //for (int i = 0; i < numberOfItems; i++)
        {
            //var spawnPosition = new Vector3(
            //    Random.Range(-3.0f, 3.0f),
            //    0.0f,
            //    Random.Range(-3.0f, 3.0f));

            var spawnPosition = new Vector3(-0.52f, 1.52f, 1f);

            var item = (GameObject)Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
            NetworkServer.Spawn(item);
        }
    }

    [Command]
    public void CmdSpawnObject(GameObject _gameObject)
    {
        NetworkServer.Spawn(_gameObject);
    }

    public void SpawnItem(Vector3 pos, bool chance = false, float chancenum = 2f, Item.ITEM_TYPE itype = Item.ITEM_TYPE.QUESTION, Item.EFFECT_TYPE etype = Item.EFFECT_TYPE.E_EXPLOSION)
    {
        if (chance)
        {
            float val = Random.Range(-5f, 5f);
            if (val > chancenum)
                return;
        }

        GameObject obj = (GameObject)Instantiate(itemPrefab, pos, Quaternion.identity);
        Item i = obj.GetComponent<Item>();
        i.itemType = itype;
        i.effectType = etype;
        //NetworkServer.Spawn(item);
    }
}
