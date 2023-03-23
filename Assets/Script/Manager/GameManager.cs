using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : SingletonObject<GameManager>
{
    public GameObject player;
    public GameObject pauseMenu;
    public HealthBar healthBar;
    public GameObject waveStatus;
    public GameObject waveNumber;
    public GameObject pickUpMenu;
    public GameObject gameOverPanel;
    public GameObject gameWinPanel;
    public GameObject statMenu;
    public bool isPaused;

    public GameObject[] enemies;
    public GameObject[] weaponDrop;
    public GameObject potionDrop;

    public List<GameObject> clearList;
    private StatModifier currentBuff;

    private float lastClearTime;
    private float breakTime = 10;

    public int currentWave;
    public bool hasSaved;
    public BoxCollider2D playArea;
    public bool StatSpecting;

    public LayerMask wall;
    public int curerentEnemyNumber;
    public int enemyNumber;
    
    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        
    }

    public void LoadMainMenu()
    {
        SceneManager.UnloadSceneAsync("Play");
        SceneManager.LoadScene("Menu");
        
    }
    private void StartNewWave()
    {
        currentWave++;
        enemyNumber = currentWave * 5;
        curerentEnemyNumber = enemyNumber;
        StartCoroutine(WaitNewWave(Condition));

    }

    IEnumerator WaitNewWave(Func<bool> con)
    {
        yield return new WaitUntil(con);
        foreach (var item in clearList)
        {
            if (item != null)
                Destroy(item);
        }
        var txt  = waveNumber.GetComponentInChildren<TextMeshProUGUI>();
        txt.text = "Wave " + currentWave;
        StartCoroutine(ShowUIByTime(3f, waveNumber));
        GetWaveBuff();
        InstantiateEnemy();
    }

    private bool Condition() => Time.time - lastClearTime > breakTime;
    private void GetWaveBuff()
    {
        currentBuff = new StatModifier();
        currentBuff.value = currentWave / 2;

    }
        
    void InstantiateEnemy()
    {
        float halfHeight = playArea.size.y / 2;
        float halfWidth = playArea.size.x / 2;
        for (int i = 0; i < curerentEnemyNumber; i++)
        {
            Vector2 spawnPos;
            
            do {
                float RandX = UnityEngine.Random.Range(playArea.transform.position.x- halfWidth, playArea.transform.position.x + halfWidth);
                float RandY = UnityEngine.Random.Range(playArea.transform.position.y - halfHeight, playArea.transform.position.y + halfHeight);
                spawnPos = new Vector2(RandX, RandY);
            }
            while (Physics2D.OverlapCircle(spawnPos, 0.3f,wall));


            int index = UnityEngine.Random.Range(0, enemies.Length);
            var obj = Instantiate(enemies[index], spawnPos, Quaternion.identity);
            var enemy = obj.GetComponent<Enemy>();
            enemy.onDead += OnEnemyDead;
            enemy.combatController.ATK.AddModifier(currentBuff);
            enemy.combatController.BulletSpeed.AddModifier(currentBuff);


        }
    }

    void GameWin()
    {
        isPaused = true;
        Time.timeScale = 0;
        gameOverPanel.SetActive(true);

    }
    
    void OnEnemyDead(Enemy enemy)
    {
        curerentEnemyNumber--;
        if (curerentEnemyNumber <= 0)
        {
            WaveClear(enemy.transform.position);
            StartNewWave();
        }
    }

    private void WaveClear(Vector2 pos)
    {
        if(currentWave==5)
        {

        }
        GameObject dropItem;
        StartCoroutine(ShowUIByTime(3f, waveStatus));
        int dropRate = UnityEngine.Random.Range(0, 2);
        if(dropRate == 0)
        {
            dropItem = Instantiate(potionDrop, pos, Quaternion.identity);
            dropItem.GetComponent<Potion>().healValue = UnityEngine.Random.Range(5, 10);
        }
        else
        {
            int weaponIndex = UnityEngine.Random.Range(0,weaponDrop.Length);
            dropItem = Instantiate( weaponDrop[weaponIndex],pos,Quaternion.identity);
        }
        clearList.Add(dropItem);
        lastClearTime = Time.time;
    }
    IEnumerator ShowUIByTime(float time,GameObject gameObject )
    {    
        gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
    private void Awake()
    {
        Time.timeScale = 1;
        clearList = new List<GameObject>();
        playArea.gameObject.SetActive(false);
        if (!hasSaved)
        {
            StarNewGame();
        }
            
    }

    void StarNewGame()
    {
        currentWave = 1;
        enemyNumber = 5;
        curerentEnemyNumber = 5;
         GetWaveBuff();
        InstantiateEnemy();
    }

    void NewWave()
    {

    }

    private Camera mainCamera;
    public Camera MainCamera
    {
        get
        {
            if(mainCamera==null)
            {
                mainCamera = FindFirstObjectByType<Camera>();
            }
            return mainCamera;
        }
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) )
        {
            Pause();
        }
        else if (Input.GetKeyDown(KeyCode.Tab) )
        {
            Stat();
        }
               
       
    }
    public void Stat()
    {
        if (!StatSpecting)
        {
            StatSpecting = true;
            statMenu.SetActive(true);
            Time.timeScale = 0;
            statMenu.GetComponent<StatMenu>().LoadStat(player.GetComponent<PlayerController>());
        }
        else
        {
            StatSpecting = false;
            statMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }
    public void Pause()
    {
        if(!isPaused)
        {
            isPaused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void Resume()
    {
        if (isPaused)
        {
            isPaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
    }





}
