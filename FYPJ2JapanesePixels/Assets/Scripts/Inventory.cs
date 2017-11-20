using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InventoryStruct
{
    public int size;
    public InventoryStruct(int setSize = 1)
    {
        size = setSize;
    }
}

public class Inventory : MonoBehaviour {
    public GameObject[] items;
    public InventoryStruct details;
    public int edit_size = 1;
	// Use this for initialization
	void Start () {
        details = new InventoryStruct(edit_size);
        items = new GameObject[details.size];
        refreshInventory();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void refreshInventory()
    {
        items = GameObject.FindGameObjectsWithTag("InventoryItem");
    }

    public GameObject equipItem(int itemInd)
    {
        if (items.Length > 0)
        {
            if (itemInd < items.Length && itemInd > 0)
                return items[itemInd];
        }
        //GameObject emptyhand = new GameObject("empty");
        //ObjectStats stat = emptyhand.AddComponent<ObjectStats>();
        //stat.damage = 0;
        return null;
    }
}
