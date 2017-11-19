using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour 
{
    public static PlayerSkillController instance = null;

    public PlayerSkill[] defensiveSkills { get; set; }
    public PlayerSkill[] offensiveSkills { get; set; }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        defensiveSkills = new PlayerSkill[3];
        offensiveSkills = new PlayerSkill[3];

        LoadSkills();
    }

    void Update()
    {
        for (int i = 0; i < defensiveSkills.Length; i++)
        {
            if (defensiveSkills[i].b_needsUpdate)
                defensiveSkills[i].Update();
        }

        for (int i = 0; i < offensiveSkills.Length; i++)
        {
            if (offensiveSkills[i].b_needsUpdate)
                offensiveSkills[i].Update();
        }
    }

    void LoadSkills() // going to load skills from some file later
    {
        for (int i = 0; i < defensiveSkills.Length; i++)
        {
            defensiveSkills[i] = new HealSkill();
        }

        for (int i = 0; i < offensiveSkills.Length; i++)
        {
            offensiveSkills[i] = new ShurikenAttack();
        }
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

        if ((playerMove.numMovesMade + _skill.numMoves) <= PlayerMoveController.MAX_MOVES)
        {
            playerMove.numMovesMade += _skill.numMoves;
            return true;
        }

        return false;
    }
}
