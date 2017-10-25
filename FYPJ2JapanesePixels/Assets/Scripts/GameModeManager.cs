using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : MonoBehaviour {
    public static GameModeManager instance = null;
    private PlayerPawn mainchar;

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
	void Start () {
        mainchar = PlayerMoveController.instance.GetPawn;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
