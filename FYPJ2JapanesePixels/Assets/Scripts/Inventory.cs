using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct OnHand
{
    // bomb type can be placed here
    public GameObject weapon;
    public int size;
    public int range;
}

public class Inventory : MonoBehaviour {
    public GameObject[] items;
    public int edit_size = 1;
    [SerializeField]
    private OnHand equipped;
    public int OnHandAmount
    {
        get { return equipped.size; }
        set { equipped.size = value; }
    }
    public GameObject OnHandItem
    {
        get { return equipped.weapon; }
        set { equipped.weapon = value; }
    }
    public int OnHandRange
    {
        get { return equipped.range; }
        set { equipped.range = value; }
    }
    public bool pendingReward;

    // Use this for initialization
    void Start () {
        items = new GameObject[equipped.size];
        refreshInventory();
        GetComponent<CharacterStats>().updateAttkStat();
        equipped.size = 100;
        equipped.range = 1;
        pendingReward = true;
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
            if (itemInd < items.Length && itemInd > -1)
                return items[itemInd];
            // successfully equipped
        }
        //GameObject emptyhand = new GameObject("empty");
        //ObjectStats stat = emptyhand.AddComponent<ObjectStats>();
        //stat.damage = 0;
        return null;
    }
}
