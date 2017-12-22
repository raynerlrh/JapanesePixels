using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour 
{
    public static PlayerSkillController instance = null;

    public PlayerSkill[] skills { get; set; }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

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
            float playerHealth = PlayerMoveController.instance.GetPawn.charStat.hpSys.health;
            if (playerHealth == PlayerMoveController.instance.GetPawn.charStat.hpSys.MAX_HEALTH)
                return false;
        }

        PlayerMoveController playerMove = PlayerMoveController.instance;

        if ((playerMove.numAvailableMoves - _skill.numMoves) >= 0)
        {
            playerMove.numAvailableMoves -= _skill.numMoves;
            return true;
        }

        return false;
    }
}
