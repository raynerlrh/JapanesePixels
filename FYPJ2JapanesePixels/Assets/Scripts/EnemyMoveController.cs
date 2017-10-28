using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveController : MonoBehaviour {
    public static EnemyMoveController instance = null;
    private GameObject parentEnemy;
    public GameObject GetBossObj
    {
        get { return parentEnemy.transform.GetChild(0).gameObject; }
    }
    public BossBehaviour currentBoss;
    public GameObject[] enemyPrefabs;

    // For most foremost operations
    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

	// Use this for initialization
	void Start () {
        parentEnemy = GameObject.Find("Enemies");
        currentBoss = new BossBehaviour();
        currentBoss.InitChar();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void RecievePlayerChoice(bool wrong = true)
    {
        if (wrong)
        {
            currentBoss.summonMinions = true;
            currentBoss.lightAttack();
        }
    }
}
