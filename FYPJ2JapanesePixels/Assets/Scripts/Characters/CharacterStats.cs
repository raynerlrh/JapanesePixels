using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HealthSystem
{
    public float health;
    public float MAX_HEALTH;
    public bool isHurt;
}

public class CharacterStats : MonoBehaviour {
    public HealthSystem hpSys;
    public float DefenseMultiplier = 1f;
    public float AttackMultiplier = 1f;
    public float HitMultiplier = 1f;
    public float attackVal;
    public float defVal;

    private GameObject hpBar;
    private GameObject defBar;
    public UIProp hpProp;
    public UIProp defProp; 
	// Use this for initialization
	void Start () {
        if (transform.childCount > 0 && transform.GetChild(0).gameObject.layer == 9)
        {
            hpBar = GameObject.Instantiate(transform.GetChild(0).GetChild(0), transform.GetChild(0), false).gameObject;
            hpBar.SetActive(true);
            defBar = GameObject.Instantiate(transform.GetChild(0).GetChild(1), transform.GetChild(0), false).gameObject;
            defBar.SetActive(true);
        }
        updateAttkStat();
	}
	
	// Update is called once per frame
	void Update () {
        if (hpBar != null)
        {
            //print("f " + hpProp.t_progbarwidth + " g " + hpSys.health);
            hpBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(hpProp.t_progbarwidth * hpSys.health, hpBar.transform.GetComponent<RectTransform>().sizeDelta.y);
        }
        if (defBar != null)
        {
            //print("f " + hpProp.t_progbarwidth + " g " + hpSys.health);
            defBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(defProp.t_progbarwidth * defVal, defBar.transform.GetComponent<RectTransform>().sizeDelta.y);
        }
    }

    public void SetStats(float attkMultiply = 1f, float defMultiply = 1f, float hitMultiply = 1f)
    {
        AttackMultiplier = attkMultiply;
        DefenseMultiplier = defMultiply;
        HitMultiplier = hitMultiply;
    }

    public void setHealth(float maxhealthval = 100)
    {
        hpSys.MAX_HEALTH = maxhealthval;
        hpSys.health = hpSys.MAX_HEALTH;
        updateDefStat();

        if (hpBar != null)
            hpProp.t_progbarwidth = hpBar.transform.GetComponent<RectTransform>().sizeDelta.x / hpSys.MAX_HEALTH;
    }

    public void decreaseHealth(float dmg_value)
    {
        float remainder = dmg_value;
        if (defVal > 0)
        {
            float defense = defVal - dmg_value;
            if (defense >= 0)
            {
                defVal = defense;
                remainder = 0;
            }
            else if (defense < 0)
            {
                remainder = Mathf.Abs(defense);
                defVal = 0;
            }
            //GetComponent<TextGenerator>().GenerateText(remainder.ToString(), true, transform.position);
        }
        hpSys.health -= remainder;
        if (GetComponent<TextGenerator>())
            GetComponent<TextGenerator>().GenerateText(remainder.ToString(), true, transform.position);
        if (hpSys.health < 0)
        {
            hpSys.health = 0;
        }
    }

    public void increaseHealth(float upvalue)
    {
        hpSys.health += upvalue;
        if (hpSys.health > hpSys.MAX_HEALTH)
            hpSys.health = hpSys.MAX_HEALTH;
        GetComponent<TextGenerator>().GenerateText(upvalue.ToString(), true, transform.position, 1, true, Color.green);
    }

    public void updateDefStat()
    {
        // multiply 0.01f == divide 100
        defVal = DefenseMultiplier * hpSys.MAX_HEALTH;
        if (defBar != null)
            defProp.t_progbarwidth = defBar.transform.GetComponent<RectTransform>().sizeDelta.x / defVal;
    }

    public void updateAttkStat()
    {
        attackVal *= AttackMultiplier;
        Inventory ivt = GetComponent<Inventory>();
        if (ivt)
        {
            //if (ivt.getEquipment)
              //  attackVal += ivt.getEquipment.GetComponent<ObjectStats>().damage;
        }
    }
}
