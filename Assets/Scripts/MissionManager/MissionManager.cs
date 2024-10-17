using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager instance;


    public Mission currentMission;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("You had more than one Mission Manager");
            Destroy(gameObject);
        }
    }


    private void Update()
    {
        currentMission?.UpdateMission();
    }

    public void SetCurrentMission(Mission newMission)
    {
        currentMission = newMission;
    }

    public void StartMission() => currentMission.StartMission();

    public bool MissionCompleted() => currentMission.MissionCompleted();


}
