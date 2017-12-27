using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour 
{
    public PlayerSkill[] skills { get; set; }

    private PlayerMoveController moveController;

    void Awake()
    {
        moveController = GetComponent<PlayerMoveController>();

        skills = new PlayerSkill[3];
        LoadSkills();
    }

    void LoadSkills()
    {
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i] = new HealSkill();
        }

        skills[1] = new DefaultAttack();
    }

    public bool CanPerformSkill(PlayerSkill _skill)
    {
        // Check health stuff first
        if (_skill.skillName == "Heal")
        {
            float playerHealth = moveController.GetPawn.charStat.hpSys.health;
            if (playerHealth == moveController.GetPawn.charStat.hpSys.MAX_HEALTH)
                return false;
        }

        if ((moveController.numAvailableMoves - _skill.numMoves) >= 0)
        {
            moveController.numAvailableMoves -= _skill.numMoves;
            return true;
        }

        return false;
    }
}
