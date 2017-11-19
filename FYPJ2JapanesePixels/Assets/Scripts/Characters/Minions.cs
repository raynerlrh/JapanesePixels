using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minions : MonoBehaviour
{
    bool isFighting; // is minion fighting?
    public DefaultCharacter character { get; set; }
    public Vector3Int cellDes;

    public enum MinionType
    {
        E_SKELETON,
        E_FLAMESKULL,
        E_PROJECTILE
    }

    public MinionType m_MinionType;

	void Start() 
    {
        isFighting = false;
        character = gameObject.AddComponent<DefaultCharacter>();
        if (m_MinionType == MinionType.E_SKELETON)
        {
            character.InitChar(20);
            character.charStat.attackVal = 5f;
        }
        else
        {
            character.InitChar(30);
            character.charStat.attackVal = 20f;
        }
	}
	
	void Update() 
    {
        if (!isFighting)
        {
            if (m_MinionType == MinionType.E_SKELETON || m_MinionType == MinionType.E_PROJECTILE)
                moveForward(-transform.right);
            else if (m_MinionType == MinionType.E_FLAMESKULL)
                moveForward(-transform.up);
            if (transform.position.x < GameModeManager.instance.gameGrid.mapWidthX)
            {
                Destroy(this.gameObject);
            }
            else if (m_MinionType == MinionType.E_FLAMESKULL && GameModeManager.instance.gameGrid.GetWorldFlToCellPos(transform.position) == cellDes)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            if (character.checkIfDead())
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
        if (m_MinionType == MinionType.E_FLAMESKULL || m_MinionType == MinionType.E_PROJECTILE)
        {
            CharacterStats stat = PlayerMoveController.instance.GetPawn.charStat;
            stat.decreaseHealth(character.charStat.attackVal);
            Destroy(this.gameObject);
        }
    }

    
    // detect if minion is fighting
    void OnCollisionStay2D(Collision2D obj)
    {
        if (isFighting && m_MinionType == MinionType.E_SKELETON)
        {
            PlayerMoveController.instance.decreasehealthbytime(2, character.charStat.attackVal);
        }
    }

    // detect if minion is fighting
    void OnCollisionExit2D(Collision2D obj)
    {
        isFighting = false;
    }
}
