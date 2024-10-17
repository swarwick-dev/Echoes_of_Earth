using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class UI : MonoBehaviour
{
    public static UI instance;

    public UI_InGame inGameUI { get; private set; }
    public UI_WeaponSelection weaponSelection { get; private set; }
    public UI_GameOver gameOverUI { get; private set; }
    public UI_Settings settingsUI { get; private set; }
    public GameObject victoryScreenUI;
    public GameObject pauseUI;
    public GameObject mainMenuUI;
    public GameObject gameUI;
    public GameObject gameOver;


    [SerializeField] private GameObject[] UIElements;

    [Header("Fade Image")]
    [SerializeField] private Image fadeImage;

    private List<GameObject> UIPath;
    private void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("You had more than one UI Manager");
            Destroy(gameObject);
        }
        
    }
    private void Start()
    {
        UIPath = new List<GameObject> { mainMenuUI };
        inGameUI = GetComponentInChildren<UI_InGame>(true);
        weaponSelection = GetComponentInChildren<UI_WeaponSelection>(true);
        gameOverUI = GetComponentInChildren<UI_GameOver>(true);
        settingsUI = GetComponentInChildren<UI_Settings>(true);
        ControlsManager.instance.SwitchToUIControls();
        //StartCoroutine(ChangeImageAlpha(0, 1.5f, null));


        // Remove this if statement before build, it's for easier testing
        //if (GameManager.instance.quickStart)
        //{
        //    LevelGenerator.instance.InitializeGeneration();
        //    StartTheGame();
        //}
    }
    public void SwitchTo(GameObject uiToSwitchOn)
    {
        foreach (GameObject go in UIElements)
        {
            go.SetActive(false);
        }
         
        uiToSwitchOn.SetActive(true);

        UIPath.Add(uiToSwitchOn);

        if (uiToSwitchOn == settingsUI.gameObject)
            settingsUI.LoadSettings();
    }

    public void Back() {
        if ( UIPath.Count > 1) {
            SwitchTo(UIPath.Last());
            UIPath.RemoveAt(UIPath.Count - 1);
        }
    }

    public void ResetUIPath() {
        UIPath.Clear();
        UIPath.Add(mainMenuUI);
    }

    public void StartTheGame(int sceneNumber) => GameManager.instance.GameStart(sceneNumber);//StartCoroutine(StartGameSequence(sceneNumber));

    public void StartLevelGeneration() => LevelGenerator.instance.InitializeGeneration();

    public void RestartTheGame()
    {
        StartCoroutine(ChangeImageAlpha(1, 1f, GameManager.instance.RestartScene));
    }

    public void PauseSwitch()
    {
        bool gamePaused = pauseUI.activeSelf;
        if (gamePaused)
        {
            SwitchTo(inGameUI.gameObject);
            ControlsManager.instance.SwitchToCharacterControls();
            TimeManager.instance.ResumeTime();
        }
        else
        {
            SwitchTo(pauseUI);
            ControlsManager.instance.SwitchToUIControls();
            TimeManager.instance.PauseTime();
        }
    }

    public void ShowGameOverUI(string message = "GAME OVER!")
    {
        SwitchTo(gameOverUI.gameObject);
        gameOverUI.ShowGameOverMessage(message);
    }

    public void ShowVictoryScreenUI()
    {
        StartCoroutine(ChangeImageAlpha(1, 1.5f, SwitchToVictoryScreenUI));
    }

    private void SwitchToVictoryScreenUI()
    {
        SwitchTo(victoryScreenUI);

        Color color = fadeImage.color;
        color.a = 0;

        fadeImage.color = color;

    }

    public void QuitTheGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif

    }

    private IEnumerator StartGameSequence(int sceneNumber = 1)
    {
        bool quickStart = GameManager.instance.quickStart;

        //THIS SHOULD BE UNCOMMENTED BEFORE MAKING A BUILD
        if (quickStart == false)
        {
            fadeImage.color = Color.black;
            StartCoroutine(ChangeImageAlpha(1, 1, null));
            yield return new WaitForSeconds(1);

        }

        yield return null;
       
        GameManager.instance.GameStart(sceneNumber);

        if(quickStart)
            StartCoroutine(ChangeImageAlpha(0,.1f, null));
        else
            StartCoroutine(ChangeImageAlpha(0,1f, null));
    }

    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator ChangeImageAlpha(float targetAlpha, float duration,System.Action onComplete)
    {
        float time = 0;
        Color currentColor = fadeImage.color;
        float startAlpha = currentColor.a;

        while(time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);

            fadeImage.color = new Color(currentColor.r,currentColor.g, currentColor.b,alpha);
            yield return null;
        }

        fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);


        // Call the completion method if it exists
        onComplete?.Invoke();
    }

    public void SwitchAllUIOff() {
        foreach (GameObject go in UIElements)
            go.SetActive(false);

    }
    public void LoadMainMenu() {
        SceneManager.LoadScene(0);
    }

    private void AssignInputEvents()
    {
        InputSystem_Actions.UIActions controls = ControlsManager.instance.controls.UI;

        controls.Back.performed += context => Back();
    }

    [ContextMenu("Assign Audio To Buttons")]
    public void AssignAudioListenesrsToButtons()
    {
        UI_Button[] buttons = FindObjectsByType<UI_Button>(FindObjectsSortMode.None);

        foreach (var button in buttons)
        {
            button.AssignAudioSource();
        }
    }
}
