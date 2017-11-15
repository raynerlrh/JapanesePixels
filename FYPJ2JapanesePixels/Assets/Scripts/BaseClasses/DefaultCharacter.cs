using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HealthSystem
{
    public float health;
    public float MAX_HEALTH;
    public bool isHurt;
}

public class DefaultCharacter
{
    public HealthSystem hpSys;
    public enum CharacterState
    {
        E_ALIVE,
        E_DEAD,
        E_STATUSAFFECTED,
        E_MAX
    }

    public CharacterState e_charState;

    // Use this for initialization
    public virtual void InitChar(float maxhealthval = 100)
    {
        hpSys.MAX_HEALTH = maxhealthval;
        hpSys.health = hpSys.MAX_HEALTH;
        e_charState = CharacterState.E_ALIVE;
    }

    public void decreaseHealth(float dmg_value)
    {
        hpSys.health -= dmg_value;
        if (hpSys.health < 0)
        {
            Mathf.Clamp(hpSys.health, 0, 1);
            e_charState = CharacterState.E_DEAD;
        }
    }

    public bool checkIfDead()
    {
        if (Mathf.Approximately(hpSys.health, 0) || hpSys.health < 0)
        {
            e_charState = CharacterState.E_DEAD;
            return true;
        }
        return false;
    }

    public void resetCharacter()
    {
        hpSys.health = hpSys.MAX_HEALTH;
        e_charState = CharacterState.E_ALIVE;
    }
}
