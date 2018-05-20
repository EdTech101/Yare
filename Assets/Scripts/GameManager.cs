using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    public GameObject CoinPrefab
    {
        get
        {
            return coinPrefab;
        }
    }

    private int CollectedCoins;

    public int collectedCoins
    {
        get
        {
            return CollectedCoins;
        }

        set
        {
            coinText.text = coinText.text[0] + " " + value.ToString();
            endcoins.text = value.ToString();
            CollectedCoins = value;
        }
    }

    private int CollectedCollectibles;

    public int collectedCollectibles
    {
        get
        {
            return CollectedCollectibles;
        }

        set
        {
            collectibles.text = collectibles.text.Replace("0", value.ToString());
            CollectedCollectibles = value;
        }
    }

    [SerializeField]
    private Text coinText;

    [SerializeField]
    private Text timePassed;

    [SerializeField]
    private Text endcoins;

    [SerializeField]
    private Text collectibles;

    [SerializeField]
    private Image key;

    [SerializeField]
    private GameObject coinPrefab;

    [SerializeField]
    private GameObject[] lives;

    [SerializeField]
    private GameObject gameOverMenu;

    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private GameObject quitMenu;

    [SerializeField]
    private GameObject optionsMenu;

    [SerializeField]
    private GameObject levelMenu;

    [SerializeField]
    private GameObject LoadingScreen;

    [SerializeField]
    private GameObject winMenu;

    private bool paused = false;

    public bool tutorial { get; set; }

    public bool gameOver { get; set; }

    public bool loadingScene = false;

    public void ShowKey()
    {
        key.enabled = true;
    }
    public void DeleteLive(int liveNumber)
    {
        if (!tutorial)
        {
            Destroy(lives[liveNumber]);
            if (liveNumber <= 0)
            {
                GameOver();
            }
        }
    }
    // Use this for initialization
    void Start()
    {
        gameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleEsc();
        LoadingScene();
    }

    private void HandleEsc()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!(SceneManager.GetActiveScene().name == "MainMenu"))
            {
                ShowPauseMenu();
            }
          
        }
    }
    public void GameOver()
    {
        if (!gameOver)
        {
            gameOver = true;
            gameOverMenu.SetActive(true);
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Resume()
    {
        if (!SoundManager.Instance.MusicSource.isPlaying)
        {
            SoundManager.Instance.ContinueMusic();
        }
        PlayClick();
        paused = false;
        Time.timeScale = 1;
        optionsMenu.SetActive(false);
        quitMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        levelMenu.SetActive(false);
        winMenu.SetActive(false);
    }

    public void Back()
    {
        PlayClick();
        paused = false;
        ShowPauseMenu();
    }

    public void ShowPauseMenu()
    {
        PlayClick();
        if (!paused)
        {
            SoundManager.Instance.PauseMusic();
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
            optionsMenu.SetActive(false);
            quitMenu.SetActive(false);
            gameOverMenu.SetActive(false);
            levelMenu.SetActive(false);
            winMenu.SetActive(false);
            paused = true;
        }
        else
        {
            Resume();
        }

    }
    public void ShowQuitConfirmMenu()
    {
        PlayClick();
        paused = true;
        Time.timeScale = 0;
        quitMenu.SetActive(true);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        optionsMenu.SetActive(false);
        levelMenu.SetActive(false);
        winMenu.SetActive(false);
    }

    public void ShowWinMenu()
    {
        PlayClick();
        timePassed.text = GetTimePassed();
        paused = true;
        Time.timeScale = 0;
        quitMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        optionsMenu.SetActive(false);
        levelMenu.SetActive(false);
        winMenu.SetActive(true);
    }

    public void ShowOptionsMenu()
    {
        PlayClick();
        Time.timeScale = 0;
        optionsMenu.SetActive(true);
        quitMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        levelMenu.SetActive(false);
        winMenu.SetActive(false);
    }

    public void ShowLevelMenu()
    {
        PlayClick();
        optionsMenu.SetActive(false);
        quitMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        levelMenu.SetActive(true);
        winMenu.SetActive(false);
    }

    public void LoadLevel1()
    {
        loadingScene = true;
        StartCoroutine(LoadNewScene("Level 1"));
        //
    }

    public void LoadTutorial()
    {
        loadingScene = true;
        StartCoroutine(LoadNewScene("Tutorial"));
        //
    }

    public void MainMenu()
    {
        loadingScene = true;
        StartCoroutine(LoadNewScene("MainMenu"));
        //
    }

    public void LoadingScene()
    {
        if (loadingScene == true)
        {
            LoadingScreen.SetActive(true);
        }
    }
    public string GetTimePassed()
    {
        var timeStamp = Time.timeSinceLevelLoad;
        var minutes = Mathf.RoundToInt(timeStamp / 60);
        var seconds = Mathf.RoundToInt(timeStamp % 60);
        string minutesString = minutes >= 10 ? minutes.ToString() : "0" + minutes.ToString();
        string secondsString = seconds >= 10 ? seconds.ToString() : "0" + seconds.ToString();
        return minutesString + ":" + secondsString;
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void PlayClick()
    {
        SoundManager.Instance.PlaySfx("rollover1");
    }

    private IEnumerator LoadNewScene(string level)
    {
        Time.timeScale = 1;
        AsyncOperation async = SceneManager.LoadSceneAsync(level);
        while (!async.isDone)
        {
            yield return null;
        }
        LoadingScreen.SetActive(false);
    }
}
