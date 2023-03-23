using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : SingletonObject<GameManager>
{
 
    public GameObject player;
    public GameObject[] enemies;
    public GameObject[] weaponDrop;
    public GameObject potionDrop;
    private Camera mainCamera;

    public List<GameObject> clearList;
    private StatModifier currentBuff;

    private float timerPivot;
    public float breakTime = 5f;
    public float prepareTime = 5f;

    [Range(0,100f)]
    public float dropRate;


    public int waveNumber;
    public int currentWave;
    public BoxCollider2D playArea;
 
    public LayerMask wall;
    public int curerentEnemyNumber;
    public int enemyNumber;
    public int enemyAddEachWave;


    public  Action OnInitStateMachine;
    public GameStateMachine waveStateMachine { get; private set; }
    public GameStateMachine gameStateMachine { get; private set; }
   
    



    #region State
    public GameState pauseState;
    public GameState victoryState;
    public GameState gameOverState;
    public GameState playingState;
    public GameState statState;
   

    public GameState waveBreakState;
    public GameState wavePrepareState;
    public GameState wavePlayingState;
   
    #endregion
    public Camera MainCamera
    {
        get
        {
            if (mainCamera == null)
            {
                mainCamera = FindFirstObjectByType<Camera>();
            }
            return mainCamera;
        }

    }
    private void Awake()
    {
        Time.timeScale = 1;
        clearList = new List<GameObject>();
        playArea.gameObject.SetActive(false);

        /*
        if (hasSaved)
        {
            //TODO: SaveSystem;
        }
        */
        waveStateMachine = new();
        gameStateMachine = new();

        //State
        victoryState = new();
        statState = new();
        gameOverState = new();
        pauseState = new();
        playingState = new();

        //WaveState
        waveBreakState = new();
        wavePrepareState = new();
        wavePlayingState = new();


        //Playing
        playingState.OnStateExit += Pause;
        playingState.OnStateEnter += Resume;
        playingState.OnStateUpdate += waveStateMachine.OnStateUpdate;
        playingState.OnStateUpdate += ()
            => StateKeyDownTransition(KeyCode.Escape, pauseState, gameStateMachine);
        playingState.OnStateUpdate += ()
            => StateKeyDownTransition(KeyCode.Tab, statState, gameStateMachine);
        playingState.OnStateUpdate += WinStateTransition;
        

        //Pause
        pauseState.OnStateUpdate += ()
            => StateKeyDownTransition(KeyCode.Escape, playingState, gameStateMachine);

        //Stat
        statState.OnStateUpdate += ()
           => StateKeyDownTransition(KeyCode.Escape, playingState, gameStateMachine);
        statState.OnStateUpdate += ()
           => StateKeyDownTransition(KeyCode.Tab, playingState, gameStateMachine);
        
        //Wave
        {

            //WaveBreak
            waveBreakState.OnStateEnter += StateTimerStart;
            waveBreakState.OnStateUpdate += ()
                => StateTimerTransition(breakTime, wavePrepareState, waveStateMachine);
           

            //WavePrepare
            wavePrepareState.OnStateEnter += StateTimerStart;
            wavePrepareState.OnStateEnter += OnWavePrepare;
            wavePrepareState.OnStateUpdate += ()
                => StateTimerTransition(prepareTime, wavePlayingState, waveStateMachine);
           

            //WavePlaying
            wavePlayingState.OnStateEnter += WaveStart;
            wavePlayingState.OnStateUpdate += ()
                => StateForceTransition(IsClear(), waveBreakState, waveStateMachine);
        }

        //TODO: Convert to event
        player.GetComponent<PlayerController>().HP.OnStatReachMinValue += (DynamicStat HP)
           => gameStateMachine.SetState(gameOverState);

        OnInitStateMachine?.Invoke();


       

    }

    private void Start()
    {
        StarNewGame();
        waveStateMachine.Init(wavePrepareState);
        gameStateMachine.Init(playingState);
       
    }
    private void Update()
    {

        gameStateMachine.OnStateUpdate();

    }



    #region State Control General
    // TODO: Maybe move this to GameStateMachine as static 
    public void Pause()
    {
        Time.timeScale = 0f;
    }

    public void Resume()
    {

        Time.timeScale = 1f;
    }

    public float GetTimeToEndState(float targetTime) => 
        Mathf.Abs(targetTime - (Time.time-timerPivot));

    private void StateTimerStart()
    {
        timerPivot = Time.time;
    }
    private void StateTimerTransition(float time, GameState target, GameStateMachine stateMachine)
    {
        if (TimerCheck(time))
        {
            stateMachine.SetState(target);
        }
    }

    private void StateKeyDownTransition(KeyCode keyCode, GameState target, GameStateMachine stateMachine)
    {
        if(Input.GetKeyDown(keyCode))
        {
            stateMachine.SetState(target);
        }
    }
    private void StateForceTransition(bool condition, GameState target, GameStateMachine stateMachine)
    {
        if(condition)
        {
            stateMachine.SetState(target);
        }
    }

    //TODO: Convert to use after wave clear 
    private void WinStateTransition()
    {
        if(IsClear() && currentWave == waveNumber)
        {
            gameStateMachine.SetState(victoryState);
        }
    }
    
    #endregion


    #region Wave State Control


    private bool TimerCheck(float targetTime) => (Time.time - timerPivot >= targetTime);
    private bool IsClear() => curerentEnemyNumber <= 0;

    void OnWavePrepare()
    {
        currentWave++;
        enemyNumber = currentWave * enemyAddEachWave;
        curerentEnemyNumber = enemyNumber;

        foreach (var item in clearList)
        {
            if (item != null)
                Destroy(item);
        }
    }

    private void WaveStart()
    {
     
        GetWaveBuff();
        InstantiateEnemy();

    }
    void StarNewGame()
    {
        currentWave = 0;
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Menu");

    }
 

    

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
                float RandX = UnityEngine.Random.Range(playArea.transform.position.x - halfWidth, playArea.transform.position.x + halfWidth);
                float RandY = UnityEngine.Random.Range(playArea.transform.position.y - halfHeight, playArea.transform.position.y + halfHeight);
                spawnPos = new Vector2(RandX, RandY);
            }
            while (Physics2D.OverlapCircle(spawnPos, 0.3f, wall));


            int index = UnityEngine.Random.Range(0, enemies.Length);
            var obj = Instantiate(enemies[index], spawnPos, Quaternion.identity);
            var enemy = obj.GetComponent<Enemy>();
            enemy.onDead += OnEnemyDead;
            enemy.combatController.ATK.AddModifier(currentBuff);
            enemy.combatController.BulletSpeed.AddModifier(currentBuff);


        }
    }

 

    void OnEnemyDead(Enemy enemy)
    {
        curerentEnemyNumber--;
        if (curerentEnemyNumber <= 0)
        {
            WaveClear(enemy.transform.position);                 
        }
        float rate = UnityEngine.Random.Range(0f, 100f);
        if (rate <= dropRate)
            DropItem(enemy.transform.position);
    }

    private void WaveClear(Vector2 pos)
    {
        /*TODO: add Boss maybe
        if (currentWave % 5 == 0)
        {
           
        }
        */
       
        DropItem(pos);
    }

    private void DropItem(Vector2 pos)
    {
        GameObject dropItem;
        int dropRate = UnityEngine.Random.Range(0, 3);
        if (dropRate == 0)
        {
            //TODO : use ObjectPool instead
            dropItem = Instantiate(potionDrop, pos, Quaternion.identity);
            dropItem.GetComponent<Potion>().healValue = UnityEngine.Random.Range(5, 10);
        }
        else
        {
            //TODO : use ObjectPool instead
            int weaponIndex = UnityEngine.Random.Range(0, weaponDrop.Length);
            dropItem = Instantiate(weaponDrop[weaponIndex], pos, Quaternion.identity);
        }
        Debug.Log(dropItem);
        clearList.Add(dropItem);
    }






    #endregion

  



}


