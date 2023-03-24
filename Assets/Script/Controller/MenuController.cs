using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MenuController : MonoBehaviour
{

    public GameObject pauseMenu;
    public HealthBar healthBar;
    public GameObject waveStatus;
    public GameObject waveNumber;
    public GameObject pickUpMenu;
    public GameObject gameOverPanel;
    public GameObject gameWinPanel;
    public GameObject statMenu;

    private StringBuilder waveText;
    private TMP_Text waveTextUI;
    private int currentTimeCounter;

    private GameStateMachine gameStateMachine;
    private void Awake()
    {
        GameManager.Instance.OnInitStateMachine += OnInitStateMachine;
        waveText = new StringBuilder();
        waveTextUI = waveNumber.GetComponentInChildren<TMP_Text>();
    }

    private void OnInitStateMachine()
    {
        GameManager gameManager = GameManager.Instance;
        gameStateMachine = GameManager.Instance.gameStateMachine;

        gameManager.pauseState.OnStateEnter += () => pauseMenu.SetActive(true);
        gameManager.pauseState.OnStateExit += () => pauseMenu.SetActive(false);

        gameManager.statState.OnStateEnter += () => { statMenu.SetActive(true); SetStat(); };
        gameManager.statState.OnStateExit += () => statMenu.SetActive(false);

        gameManager.gameOverState.OnStateEnter += () => gameOverPanel.SetActive(true);
        gameManager.gameOverState.OnStateExit += () => gameOverPanel.SetActive(false);

        gameManager.victoryState.OnStateEnter += () => gameWinPanel.SetActive(true);
        gameManager.victoryState.OnStateExit += () => gameWinPanel.SetActive(false);

        gameManager.wavePrepareState.OnStateEnter += () => waveNumber.SetActive(true);
        gameManager.wavePrepareState.OnStateExit += () => waveNumber.SetActive(false);
        gameManager.wavePrepareState.OnStateUpdate += NewWaveWarn;

        gameManager.gameOverState.OnStateEnter += () => gameOverPanel.SetActive(true);
        gameManager.gameOverState.OnStateExit += () => gameOverPanel.SetActive(false);

        //TODO: use timer for this maybe
        gameManager.waveBreakState.OnStateEnter += ()
            => StartCoroutine(ShowUITimer(waveStatus, new WaitForSeconds(3f)));

        var playerControl = gameManager.player.GetComponent<PlayerController>();
        
        playerControl.HPMax.OnStatChanged += SetHeathBarMax;
        playerControl.HP.OnValueChanged += SetHeathBar;

        SetHeathBarMax(playerControl.HPMax);
        SetHeathBar(playerControl.HP);
    }

    private void SetHeathBar(DynamicStat hp)
    {
    
        healthBar.SetHelth(hp.Value);
    }

    private void SetHeathBarMax(Stat hp)
    {
        healthBar.SetMaxHelth(hp.Value);
    }

    private void SetStat()
    {
        //TODO: Cache this
        var player = GameManager.Instance.player.GetComponent<PlayerController>();
        statMenu.GetComponent<StatMenu>().LoadStat(player);
    }
 
    private void NewWaveWarn()
    {
        var gameManager = GameManager.Instance;
        int timeCouter = Mathf.CeilToInt(gameManager.GetTimeToEndState(gameManager.prepareTime));
        if (timeCouter != currentTimeCounter)
        {
            currentTimeCounter = timeCouter ;
            waveText.Clear();
            waveText.AppendFormat("Wave {0} start in: {1}", gameManager.currentWave, timeCouter);
            waveTextUI.SetText(waveText);
        }

    }
    private IEnumerator ShowUITimer(GameObject UI,WaitForSeconds waitForSeconds)
    {
        UI.SetActive(true);
        yield return waitForSeconds;
        UI.SetActive(false);
    }

    public void Resume()
    {
        gameStateMachine.SetState(GameManager.Instance.playingState);
    }

    public void Menu()
    {
        GameManager.Instance.LoadMainMenu();
    }
  
}
