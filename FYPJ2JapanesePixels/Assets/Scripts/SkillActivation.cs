using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillActivation : MonoBehaviour
{
    bool once = false;
    CharacterStats charstat;

    private void Start()
    {
        charstat = GetComponent<CharacterStats>();
    }
    /// <summary>
    /// Auto attack upon collision with enemy
    /// </summary>
    /// <param name="obj"></param>
    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.gameObject.layer == 14 && !once)
        {
            obj.GetComponent<CharacterStats>().decreaseHealth(charstat.attackVal);
            once = true;
        }
    }

    private void LateUpdate()
    {
        if (once)
            once = false;
    }
}
