using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSkill : PlayerSkill 
{
    public bool b_needsUpdate { get; set; }
    public int numMoves { get; set; }
    public float damage { get; set; }
    public string skillName { get; set; }

    public int cellsAffected { get; set; }

    public HealSkill()
    {
        skillName = "Heal";
        numMoves = 1;
    }

    public void ExecuteSkill()
    {
        //Debug.Log("Executed Skill: " + skillName);

        PlayerMoveController.instance.GetPawn.charStat.increaseHealth(20);
    }

    public void Update()
    {
        if (!b_needsUpdate)
            return;
    }
}
