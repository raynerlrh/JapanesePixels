using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillActivation : MonoBehaviour
{
    // The char being targeted so that attacks are focused on target
    private DefaultCharacter attackTarget;
    
    /// <summary>
    /// Auto attack upon collision with enemy
    /// </summary>
    /// <param name="obj"></param>
    void OnCollisionStay2D(Collision2D obj)
    {
        if (obj.gameObject.layer == 8)
        {
            if (attackTarget == null)
            {
                if (obj.gameObject.GetComponent<Minions>())
                    attackTarget = obj.gameObject.GetComponent<Minions>().character;
                else
                    attackTarget = obj.gameObject.GetComponent<DefaultCharacter>();

            }
            attackTarget.charStat.decreaseHealth(GetComponent<CharacterStats>().attackVal);

            if (attackTarget.checkIfDead())
                attackTarget = null;
        }
    }
}
