using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject playerPrefab;
    public Player player {get; private set;}
    public ControlsManager controlsManager { get; private set; }
    public InputSystem_Actions controls { get; private set; }
    public GameObject playerObj { get; private set; }
    int currentMissionIndex;

    [Header("Settings")]
    public bool friendlyFire;
    [Space]
    public bool quickStart;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        controlsManager = new ControlsManager();
        controls = controlsManager.controls;
        controls.UI.Enable();
    }

  
    public void GameStart(int missionIndex)
    {
        currentMissionIndex = missionIndex;
        controls.UI.Disable();
        controls.Character.Enable();
        controls.Car.Disable();

        UI.instance.ResetUIPath();

        StartCoroutine(WaitForSceneToLoad());
    }

    public void RestartScene() => GameStart(currentMissionIndex);//SceneManager.LoadScene(currentMissionIndex);

    public void GameCompleted()
    {
        UI.instance.ShowVictoryScreenUI();
        ControlsManager.instance.controls.Character.Disable();
        player.health.currentHealth += 99999; // So player won't die in last second.
    }
    public void GameOver()
    {
        TimeManager.instance.SlowMotionFor(1.5f);
        UI.instance.ShowGameOverUI();
        CameraManager.instance.ChangeCameraDistance(5);
    }

    private void SetDefaultWeaponsForPlayer()
    {
        List<Weapon_Data> newList = UI.instance.weaponSelection.SelectedWeaponData();
        player.weapon.SetDefaultWeapon(newList);
    }

    private IEnumerator WaitForSceneToLoad()
    {
        // Put up load screen

        var asyncLoadLevel = SceneManager.LoadSceneAsync(currentMissionIndex, LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }

        // Take down load screen
        
        player = FindFirstObjectByType<Player>();
        SetDefaultWeaponsForPlayer();
        controlsManager.SwitchToCharacterControls();
        if ( currentMissionIndex == 1)
            LevelGenerator.instance.InitializeGeneration();
    }
}
