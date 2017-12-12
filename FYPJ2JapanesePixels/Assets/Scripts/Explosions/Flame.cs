using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : ObjectStats {
    TimerRoutine burnout;
	// Use this for initialization
	void Start () {
        burnout = gameObject.AddComponent<TimerRoutine>();
        burnout.initTimer(0.3f);
        burnout.executedFunction = DestroySelf;
        burnout.executeFunction();
	}

    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.gameObject.layer == 14)
        {
            if (obj.GetComponent<CharacterStats>())
            {
                obj.GetComponent<CharacterStats>().decreaseHealth(damage);
                DestroySelf();
            }
        }
    }
}
