using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSkill : PlayerSkill 
{
    public bool b_needsUpdate { get; set; }
    public string skillName { get; set; }

    public HealSkill()
    {
        skillName = "Heal";
    }

    public void ExecuteSkill()
    {
        Debug.Log("Executed Skill: " + skillName);

        float playerHealth = PlayerMoveController.instance.GetPawn.hpSys.health;

        if (playerHealth == PlayerMoveController.instance.GetPawn.hpSys.MAX_HEALTH)
            return;

        playerHealth += 20f;
        PlayerMoveController.instance.GetPawn.hpSys.health = playerHealth;
    }

    public void Update()
    {
        if (!b_needsUpdate)
            return;
    }
}
