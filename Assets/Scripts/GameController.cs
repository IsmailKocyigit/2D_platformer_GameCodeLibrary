using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    int progressAmount;
    public Slider progressSlider;

    public GameObject player;
    public GameObject loadCanvas;
    public List<GameObject> levels;
    private int currentLevelIndex = 0;

    public GameObject gameOverScreen;
    public TMP_Text survivedText;
    private int survivedLevelsCount;

    public static event Action OnReset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        progressAmount = 0;
        progressSlider.value = 0;
        // subscribe to the event
        Gem.OnGemCollect += IncreaseProgressAmount;
        // "HoldToLoadLevel" is name we use to reach the other script
        // subscribe to the event "OnHoldToComplete" and activate function "LoadNextLevel" when the event happens
        HoldToLoadLevel.OnHoldComplete += LoadNextLevel;
        PlayerHealth.OnPlayerDied += GameOverScreen;
        loadCanvas.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    void GameOverScreen()
    {
        gameOverScreen.SetActive(true);
        survivedText.text = "YOU SURVIVED " + survivedLevelsCount + " LEVEL";
        if (survivedLevelsCount != 1) { survivedText.text += "S"; }

        Time.timeScale = 0;
    }

    public void ResetGame()
    {
        gameOverScreen.SetActive(false);
        survivedLevelsCount = 0;
        LoadLevel(0, false);
        OnReset.Invoke();

        Time.timeScale = 1;
    }

    void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;
        progressSlider.value = progressAmount;

        if (progressAmount >= 100)
        {
            // level complete code
            Debug.Log("Level Complete!!!");

            loadCanvas.SetActive(true);
        }
    }

    void LoadLevel(int level, bool wantSurvivedIncrease)
    {
        loadCanvas.SetActive(false);

        // disable current level and load next level
        levels[currentLevelIndex].gameObject.SetActive(false);
        levels[level].gameObject.SetActive(true);

        // reset player location
        player.transform.position = new Vector3(0, 0, 0);

        // update level index numbers and reset loading circle load amount
        currentLevelIndex = level;
        progressAmount = 0;
        progressSlider.value = progressAmount;

        if (wantSurvivedIncrease) survivedLevelsCount++;
    }

    void LoadNextLevel()
    {
        // set index numbers for level loading
        int nextLevelIndex = (currentLevelIndex == levels.Count - 1) ? 0 : currentLevelIndex + 1;
        LoadLevel(nextLevelIndex, true);
    }
}
