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

    public GameObject hpBar;
    public UIProp hpProp; 
	// Use this for initialization
	void Start () {
        attackVal *= AttackMultiplier;
        if (transform.childCount > 0 && transform.GetChild(0).gameObject.layer == 9)
        {
            hpBar = GameObject.Instantiate(transform.GetChild(0).GetChild(0), transform.GetChild(0), false).gameObject;
            hpBar.SetActive(true);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (hpBar != null)
        {
            //print("f " + hpProp.t_progbarwidth + " g " + hpSys.health);
            hpBar.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(hpProp.t_progbarwidth * hpSys.health, hpBar.transform.GetComponent<RectTransform>().sizeDelta.y);
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
        GetComponent<TextGenerator>().GenerateText(remainder.ToString(), true, transform.position);
        if (hpSys.health < 0)
        {
            Mathf.Clamp(hpSys.health, 0, 1);
        }
    }

    public void updateDefStat()
    {
        // multiply 0.01f == divide 100
        defVal = DefenseMultiplier * hpSys.MAX_HEALTH;
    }
}
