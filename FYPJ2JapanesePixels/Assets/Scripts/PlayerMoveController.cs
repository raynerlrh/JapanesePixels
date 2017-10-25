using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private float health;
    private const float MAX_HEALTH = 100;
    enum CharacterState
    {
        E_ALIVE,
        E_DEAD,
        E_STATUSAFFECTED
    }
    public CharacterState e_charState;

	// Use this for initialization
	void Start () {
        health = MAX_HEALTH;
        e_charState = CharacterState.E_ALIVE;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void decreaseHealth(float dmg_value)
    {
        health -= dmg_value;
        if (health < 0)
        {
            Mathf.Clamp(health, 0, 1);
            e_charState = CharacterState.E_DEAD;
        }
    }

    bool checkIfDead()
    {
        if (Mathf.Approximately(health, 0))
        {
            e_charState = CharacterState.E_DEAD;
            return true;
        }
        return false;
    }

    void resetCharacter()
    {
        health = MAX_HEALTH;
        e_charState = CharacterState.E_ALIVE;
    }
}
