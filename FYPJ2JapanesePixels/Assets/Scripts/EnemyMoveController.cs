using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveController : MonoBehaviour 
{
    public static EnemyMoveController instance = null;
    private GameObject parentEnemy;
    public GameObject GetBossObj
    {
        get { return parentEnemy.transform.GetChild(0).gameObject; }
    }
    public Boss.BossBehaviour currentBoss;
    public GameObject[] enemyPrefabs;

    // For most foremost operations
    void Awake() 
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //DontDestroyOnLoad(gameObject);
    }

	void Start() 
    {
        parentEnemy = GameObject.Find("Enemies");
        currentBoss = GetBossObj.GetComponent<Boss.BossBehaviour>();
        currentBoss.InitChar(250);
	}
	
	void Update() 
    {
	}

    void ReceivePlayerChoice(bool wrong)
    {
        //if (wrong)
        //{
        //    EnemyMoveController.instance.currentBoss.doAttack();
        //}
    }
}
