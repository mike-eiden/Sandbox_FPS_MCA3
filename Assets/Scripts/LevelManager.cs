using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{

    public float timerTime = 30f;
    public GameObject gameOverText;
    public Text timerText;
    public GameObject levelBeatText; 
    public Transform player;
    public GameObject level1;
    public GameObject level2;
    public GameObject level3;
    public AudioClip gameOverNoise;
    public AudioClip levelWinNoise;
    public GameObject enemySpawner;
    public Text scoreText;
    
    private int score; 
    private Vector3 playerStartPos; 
    private float countDown;

    private int currentLevel = 1;
    public bool levelOverLock = false; 
    
    void Start()
    {
        countDown = timerTime;
        timerText.text = countDown.ToString("0.00");
        scoreText.text = "Score = " + score; 
        playerStartPos = player.position; 
    }

    // Update is called once per frame
    void Update()
    {
        if (!levelOverLock){

            if (countDown > 0 && currentLevel < 4)
            {
                countDown -= Time.deltaTime;
                timerText.text = countDown.ToString("f2");
            }
            else if (countDown < 0)
            {
                levelWon(); 
            }
        }
    }
    
    public void levelLost()
    {
        levelOverLock = true;
        timerText.enabled = false; 
        gameOverText.SetActive(true);
        AudioSource.PlayClipAtPoint(gameOverNoise, GameObject.FindGameObjectWithTag("MainCamera").transform.position);
        StartCoroutine(ResetCoroutine());       
    }

    IEnumerator ResetCoroutine()
    {
        yield return new WaitForSeconds(2);
        score = 0; 
        scoreText.text = "Score = " + score; 
        countDown = timerTime;
        timerText.enabled = true;
        gameOverText.SetActive(false);
        levelBeatText.SetActive(false);
        timerText.text = countDown.ToString("0.00");
        player.position = playerStartPos;
        foreach (Transform enemy in enemySpawner.transform)
        {
           Destroy(enemy.gameObject);

        }

        if (currentLevel == 1)
        {
            // Need to make sure we don't load the same scene twice i.e. on a reset after death
            UnloadAllLevels();
            SceneManager.LoadScene("Level1", LoadSceneMode.Additive);
//            level1.SetActive(true);
//            level2.SetActive(false);
//            level3.SetActive(false);
            enemySpawner.GetComponent<EnemySpawner>().enemy2enable = false;
            foreach (Transform child in level1.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        if (currentLevel == 2)
        {
            // Need to make sure we don't load the same scene twice i.e. on a reset after death
            UnloadAllLevels();
            SceneManager.LoadScene("Level2", LoadSceneMode.Additive);
            
//            level1.SetActive(false);
//            level2.SetActive(true);
//            level3.SetActive(false);
            enemySpawner.GetComponent<EnemySpawner>().enemy2enable = true;
            foreach (Transform child in level2.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        if (currentLevel == 3)
        {
            // Need to make sure we don't load the same scene twice i.e. on a reset after death
            UnloadAllLevels();
            SceneManager.LoadScene("Level3", LoadSceneMode.Additive);
//            level1.SetActive(false);
//            level2.SetActive(false);
//            level3.SetActive(true);
            enemySpawner.GetComponent<EnemySpawner>().enemy2enable = true;
            foreach (Transform child in level3.transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        if (currentLevel == 4)
        {
            levelBeatText.GetComponent<Text>().text = "Game Complete!";
            levelBeatText.SetActive(true);
            timerText.enabled = false; 
            Destroy(enemySpawner);
        }
        
        player.GetComponent<PlayerHealth>().resetHelthBar();
        levelOverLock = false; 
    }
    

    public void levelWon()
    {
        levelOverLock = true;
        levelBeatText.SetActive(true);
        AudioSource.PlayClipAtPoint(levelWinNoise, GameObject.FindGameObjectWithTag("MainCamera").transform.position);
        timerText.enabled = false; 
        currentLevel++;
        StartCoroutine(ResetCoroutine()); 
    }

    public void incrementScore()
    {
        score++; 
        scoreText.text = "Score = " + score; 
    }

    private void UnloadAllLevels()
    {
        try
        {
            SceneManager.UnloadSceneAsync("Level1");
        }
        catch (ArgumentException e)
        {
            //Catch and Release 
            Debug.Log( "Handled -> " + e.Message);
        }
        try
        {
            SceneManager.UnloadSceneAsync("Level2");
        }
        catch (ArgumentException e)
        {
            //Catch and Release 
            Debug.Log( "Handled -> " + e.Message);
        }
        try
        {
            SceneManager.UnloadSceneAsync("Level3");
        }
        catch (ArgumentException e)
        {
            //Catch and Release 
            Debug.Log( "Handled -> " + e.Message);
        }
    }
    
}
