using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultCharacter : MonoBehaviour
{
    public enum CharacterState
    {
        E_ALIVE,
        E_DEAD,
        E_STATUSAFFECTED,
        E_MAX
    }

    public CharacterState e_charState;
    public CharacterStats charStat;

    // Use this for initialization
    public virtual void InitChar(float maxhealthval = 100)
    {
        e_charState = CharacterState.E_ALIVE;
        charStat = GetComponent<CharacterStats>();
        charStat.setHealth(maxhealthval);
    }

    public bool checkIfDead()
    {
        if (Mathf.Approximately(charStat.hpSys.health, 0) || charStat.hpSys.health < 0)
        {
            e_charState = CharacterState.E_DEAD;
            return true;
        }
        return false;
    }

    public void resetCharacter()
    {
        charStat.hpSys.health = charStat.hpSys.MAX_HEALTH;
        e_charState = CharacterState.E_ALIVE;
    }
}
