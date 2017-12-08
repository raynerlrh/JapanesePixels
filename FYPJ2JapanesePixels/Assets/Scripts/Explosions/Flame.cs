using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : ObjectStats {
    TimerRoutine burnout;
    
	// Use this for initialization
	void Start () {
        burnout = gameObject.AddComponent<TimerRoutine>();
        burnout.initTimer(3);
        burnout.executedFunction = DestroySelf;
        burnout.executeFunction();
	}

    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.gameObject.layer == 10)
        {
            if (obj.GetComponent<CharacterStats>())
            {
                obj.GetComponent<CharacterStats>().decreaseHealth(50);
                DestroySelf();
            }
        }
    }

}
