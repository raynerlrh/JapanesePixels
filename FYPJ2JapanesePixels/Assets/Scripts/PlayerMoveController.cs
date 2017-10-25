using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour 
{
    public static PlayerMoveController instance = null;
    private PlayerPawn pawn; // main character, can be an array of pawns for each character
    private GameObject pawn_sprite; // can be an array of sprites for each character
    public PlayerPawn GetPawn
    {
        get
        {
            return pawn;
        }
    }
    // Only for most foremost operations
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        pawn = new PlayerPawn();
        pawn.InitChar();
        pawn_sprite = GameObject.Find("PlayerHero");
    }

    // Update is called once per frame
    void Update()
    {
        if (pawn_sprite.activeSelf)
        {
            if (pawn.checkIfDead())
            {
                Debug.Log("dd");

                pawn_sprite.SetActive(false);
            }
        }
    }

    void RecievePlayerChoice()
    {
        Debug.Log("i fucked up!");
        pawn.decreaseHealth(10);
    }
}
