using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy1;
    public GameObject enemy2;
    public LevelManager levelManager;

    public bool enemy2enable = false; 

    private const float enemy2Yoffset = 1.25f;

    public bool Enemy2Enable
    {
        get => enemy2enable;
        set => enemy2enable = value;
    }

    // Update is called once per frame
    void Update()
    {
        int shouldSpawn = Random.Range(1, 200);
        if (shouldSpawn == 11 && !levelManager.levelOverLock)
        {
            GameObject enemy = Instantiate(enemy1, new Vector3(Random.Range(-9, 9), 0, Random.Range(-9, 29)),
                Quaternion.Euler(new Vector3(0,0,0)));
            enemy.transform.parent = transform; 
        }
        if (shouldSpawn == 12 && enemy2enable && !levelManager.levelOverLock)
        {
            GameObject enemy = Instantiate(enemy2, new Vector3(Random.Range(-9, 9), enemy2Yoffset, Random.Range(-9, 29)),
                Quaternion.Euler(new Vector3(0,0,0)));
            enemy.transform.parent = transform; 
        }
    }
}
