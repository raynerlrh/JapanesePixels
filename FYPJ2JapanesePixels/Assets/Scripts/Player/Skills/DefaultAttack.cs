using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultAttack : PlayerSkill {

    public bool b_needsUpdate { get; set; }
    public int numMoves { get; set; }
    public int cellsAffected { get; set; }
    public float damage { get; set; }
    public string skillName { get; set; }

    public DefaultAttack()
    {
        skillName = "DefaultAttack";
        numMoves = 1;
        b_needsUpdate = false;
    }

    public void ExecuteSkill()
    {
        //Debug.Log("Executed Skill: " + skillName);
        damage = PlayerMoveController.instance.GetPawn.GetComponent<CharacterStats>().attackVal;
        EnemyMoveController.instance.GetBossObj.GetComponent<CharacterStats>().decreaseHealth(damage);
    }

    public void Update()
    {
        if (!b_needsUpdate)
            return;
    }
}
