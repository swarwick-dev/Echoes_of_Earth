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
        
        //playerObj = Instantiate(playerPrefab, new Vector3(0, 0.5f, 0), Quaternion.identity);
        //DontDestroyOnLoad(playerObj);
        //player = playerObj.GetComponent<Player>();
    }

  
    public void GameStart(int missionIndex)
    {
        controls.UI.Disable();
        controls.Character.Enable();
        controls.Car.Disable();

        StartCoroutine(WaitForSceneToLoad(missionIndex));

       

        //LevelGenerator.instance.InitializeGeneration();
        // We start selected mission in a LevelGenerator script ,after we done with level creation.
    }

    public void RestartScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

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

    private IEnumerator WaitForSceneToLoad(int sceneNumber)
    {
        var asyncLoadLevel = SceneManager.LoadSceneAsync(sceneNumber, LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }
        
        player = FindFirstObjectByType<Player>();
        SetDefaultWeaponsForPlayer();
    }
}
