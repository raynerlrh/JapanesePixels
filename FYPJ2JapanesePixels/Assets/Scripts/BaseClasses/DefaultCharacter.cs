using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultCharacter 
{
    protected float health;
    protected const float MAX_HEALTH = 100;

    public enum CharacterState
    {
        E_ALIVE,
        E_DEAD,
        E_STATUSAFFECTED
    }

    public CharacterState e_charState;

    // Use this for initialization
    public virtual void InitChar()
    {
        health = MAX_HEALTH;
        e_charState = CharacterState.E_ALIVE;
    }

    public void decreaseHealth(float dmg_value)
    {
        health -= dmg_value;
        if (health < 0)
        {
            Mathf.Clamp(health, 0, 1);
            e_charState = CharacterState.E_DEAD;
        }
    }

    public bool checkIfDead()
    {
        if (Mathf.Approximately(health, 0))
        {
            e_charState = CharacterState.E_DEAD;
            return true;
        }
        return false;
    }

    public void resetCharacter()
    {
        health = MAX_HEALTH;
        e_charState = CharacterState.E_ALIVE;
    }
}
