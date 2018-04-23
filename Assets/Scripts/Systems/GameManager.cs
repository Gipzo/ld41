using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<GameManager>();
            return _instance;
        }
    }


    public bool Paused = true;


    public GameObject MainMenu;
    public GameObject PauseMenu;
    public GameObject MessageMenu;
    public GameObject EndMenu;
    public InputField NickInput;
    public Text MessageText;
    public GameObject ScoreText;
    public GameObject HighscoreSender;
    public GameObject HighscoreSent;
    public float MessageDelay = 2.3f;
    public PalleteSwap Pallete;

    public PlayerController Player;
    public float EnemyDestroyInterval = 0.1f;

    public Color HurtColor;

    // Use this for initialization
    void Start()
    {
        MainMenu.SetActive(true);
        InitGame();
    }

    public void StartGame()
    {
        MainMenu.SetActive(false);
        InitGame();
        Paused = false;
        ScoreText.SetActive(true);

        GameObject.FindObjectOfType<Spawner>().StartSpawner();
    }


    public void OpenMenu()
    {
        MainMenu.SetActive(true);
        PauseMenu.SetActive(false);
        MessageMenu.SetActive(false);
        EndMenu.SetActive(false);
        ScoreText.SetActive(false);
        Paused = true;
    }

    public void TogglePallete()
    {
        Pallete.TogglePallette();
    }

    public void ShowMessage(string text, bool hide = true)
    {
        CancelInvoke("HideMessage");
        MessageMenu.SetActive(true);
        MessageText.text = text;
        if (hide)
            Invoke("HideMessage", MessageDelay);
    }

    void HideMessage()
    {
        MessageMenu.SetActive(false);
    }

    public void InitGame()
    {
        EndMenu.SetActive(false);
        MessageMenu.SetActive(false);
        PauseMenu.SetActive(false);
        EndMenu.SetActive(false);

        foreach (var enemy in GameObject.FindObjectsOfType<Enemy>())
        {
            enemy.DestroyEnemy();
        }
        Player.Init();
    }

    void DestroyEnemies()
    {
        var enemies = GameObject.FindObjectsOfType<Enemy>();
        if (enemies.Length > 0)
        {
            enemies[0].Explode();
            Invoke("DestroyEnemies", EnemyDestroyInterval);
        }
    }

    public void Pause()
    {
        Paused = true;
        PauseMenu.SetActive(true);
    }

    public void UnPause()
    {
        Paused = false;
        PauseMenu.SetActive(false);
    }

    public void SendScores()
    {
        if (NickInput.text == "")
            return;
            
        HighscoreSender.SetActive(false);
        HighscoreSent.SetActive(true);
        Highscores.Instance.SendScore(NickInput.text, Player.Score);
    }

    public void RestartGame()
    {
        StartGame();
    }

    public void WavePassed(int wave_number)
    {
        ShowMessage("WAVE " + (wave_number + 1).ToString() + " PASSED!");
    }

    public void GameEnded(bool player_died)
    {
        Paused = true;
        if (player_died)
        {
            ShowMessage("YOU DIED!\n\nYOUR SCORE: " + Player.Score.ToString(), false);
        }
        else
        {
            ShowMessage("YOU WON!\n\nYOUR SCORE: " + Player.Score.ToString(), false);
        }
        DestroyEnemies();

        HighscoreSender.SetActive(true);
        HighscoreSent.SetActive(false);
        EndMenu.SetActive(true);

    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
            if (!Paused)
            {
                Pause();
            }
            else
            {
                UnPause();
            }
    }
}
