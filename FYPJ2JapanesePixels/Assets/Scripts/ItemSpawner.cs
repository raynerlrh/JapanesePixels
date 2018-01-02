using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Temporary definition
public class ItemSpawner : NetworkBehaviour 
{
    public GameObject itemPrefab;
    public int numberOfItems;

    [SerializeField]
    Transform gameCharacters;

    public GameObject enemyPrefab;
    public Transform[] t_spawnArr;
    public Sprite[] s_spriteList; 

    // This will be the start function since without the NetworkManager in single player mode, this game object will be set to inactive
    // For use for single player stuff only
    void OnDisable()
    {
        if (MyNetwork.instance.IsOnlineGame())
            return;

        var spawnPosition = new Vector3(-0.52f, 1.52f, 1f);

        var item = (GameObject)Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
    }

    // For use for multiplayer stuff only
    public override void OnStartServer()
    {
        //for (int i = 0; i < numberOfItems; i++)
        {
            //var spawnPosition = new Vector3(
            //    Random.Range(-3.0f, 3.0f),
            //    0.0f,
            //    Random.Range(-3.0f, 3.0f));

            var spawnPosition = new Vector3(-0.52f, 1.52f, 1f);

            //var item = (GameObject)Instantiate(itemPrefab, spawnPosition, Quaternion.identity);

            //NetworkServer.Spawn(item);
        }

        //        if (t_spawnArr == null)
        //        {
        //            GameObject enemy = (GameObject)Instantiate(enemyPrefab, new Vector3(5f, 3.51f, 0f), Quaternion.identity);
        //enemy.transform.SetParent(gameCharacters);            
        //NetworkServer.Spawn(enemy);
        //            enemy = (GameObject)Instantiate(enemyPrefab, new Vector3(-5f, -3.51f, 0f), Quaternion.identity);
        //enemy.transform.SetParent(gameCharacters);            
        //enemy.transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
        //            enemy.transform.GetChild(3).GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
        //            enemy.transform.GetChild(3).GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
        //            enemy.transform.GetChild(3).GetChild(0).GetChild(3).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
        //            enemy.transform.GetChild(3).GetChild(0).GetChild(4).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
        //            NetworkServer.Spawn(enemy);

        //            enemy = (GameObject)Instantiate(enemyPrefab, new Vector3(5f, -3.5f, 0f), Quaternion.identity);
        //enemy.transform.SetParent(gameCharacters);            
        //enemy.transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = new Color(100, 100, 100);
        //            enemy.transform.GetChild(3).GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().color = new Color(100, 100, 100);
        //            enemy.transform.GetChild(3).GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().color = new Color(100, 100, 100);
        //            enemy.transform.GetChild(3).GetChild(0).GetChild(3).GetComponent<SpriteRenderer>().color = new Color(100, 100, 100);
        //            enemy.transform.GetChild(3).GetChild(0).GetChild(4).GetComponent<SpriteRenderer>().color = new Color(100, 100, 100);
        //            NetworkServer.Spawn(enemy);
        //        }

        for (int j = 0; j < t_spawnArr.Length; ++j)
        {
            GameObject enemy = (GameObject)Instantiate(enemyPrefab, t_spawnArr[j].position, Quaternion.identity);
            enemy.transform.SetParent(gameCharacters);
            Color randCol = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            enemy.transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = randCol;
            enemy.transform.GetChild(3).GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().color = randCol;
            enemy.transform.GetChild(3).GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().color = randCol;
            enemy.transform.GetChild(3).GetChild(0).GetChild(3).GetComponent<SpriteRenderer>().color = randCol;
            enemy.transform.GetChild(3).GetChild(0).GetChild(4).GetComponent<SpriteRenderer>().color = randCol;
            NetworkServer.Spawn(enemy);
        }
    }

    [Command]
    public void CmdSpawnObject(GameObject _gameObject)
    {
        NetworkServer.Spawn(_gameObject);
    }

    public void spawnAnotherEnemy()
    {
        int r = Random.Range(0, t_spawnArr.Length - 1);
        GameObject enemy = (GameObject)Instantiate(enemyPrefab, t_spawnArr[r].position, Quaternion.identity);
        enemy.transform.SetParent(gameCharacters);
        Color randCol = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        enemy.transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = randCol;
        enemy.transform.GetChild(3).GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().color = randCol;
        enemy.transform.GetChild(3).GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().color = randCol;
        enemy.transform.GetChild(3).GetChild(0).GetChild(3).GetComponent<SpriteRenderer>().color = randCol;
        enemy.transform.GetChild(3).GetChild(0).GetChild(4).GetComponent<SpriteRenderer>().color = randCol;
        NetworkServer.Spawn(enemy);
    }
}
