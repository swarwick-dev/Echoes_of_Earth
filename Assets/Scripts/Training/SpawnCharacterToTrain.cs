using System.Collections.Generic;
using UnityEngine;

public class SpawnCharacterToTrain : MonoBehaviour
{
    public GameObject playerPrefab;
    public Weapon_Data defaultWeaponData;
    private GameObject playerObj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        //Player player = null;

        //List<Weapon_Data> defaultWeaponDataList = new List<Weapon_Data>
        //{
        //    defaultWeaponData
        //};

        playerObj = Instantiate(playerPrefab, new Vector3(0, 0.5f, 0), Quaternion.identity);

        //player = playerObj.GetComponentInChildren<Player>();
        //player.weapon.SetDefaultWeapon(defaultWeaponDataList); 

        ControlsManager.instance.SwitchToCharacterControls();       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
