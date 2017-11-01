using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minions : MonoBehaviour, EnemyPawn<Vector3> 
{
    bool isFighting; // is minion fighting?

	void Start() 
    {
        isFighting = false;
	}
	
	void Update() 
    {
        if (!isFighting)
        {
            moveForward(-transform.right);
            if (transform.position.x < GameModeManager.instance.gameGrid.mapWidthX)
            {
                Destroy(this.gameObject);
            }
        }
	}

    /// <summary>
    /// makes minion move forward
    /// </summary>
    /// <param name="forwardvec">facing vector</param>
    public void moveForward(Vector3 forwardvec)
    {
        transform.position = Vector2.MoveTowards(transform.position, transform.position + forwardvec, Time.deltaTime);
    }

    /// <summary>
    /// Minion has collided with something, check if it is an enemy to fight!
    /// </summary>
    /// <param name="collide"> collided object </param>
    void OnCollisionEnter2D(Collision2D collide)
    {
        isFighting = true;
    }

    
    // detect if minion is fighting
    void OnCollisionStay2D(Collision2D obj)
    {
        if (isFighting)
        {
            PlayerMoveController.instance.decreasehealthbytime(2, 5);
            Destroy(this.gameObject);
        }
    }

    // detect if minion is fighting
    void OnCollisionExit2D(Collision2D obj)
    {
        isFighting = false;
    }
}
