using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    private CinemachineCamera virtualCamera;
    private CinemachinePositionComposer transposer;


    [Header("Camera distance")]
    [SerializeField] private bool canChangeCameraDistance;
    [SerializeField] private float distanceChangeRate;
    [SerializeField] private float targetCameraDistance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("You had more than one Camera Manager");
            Destroy(gameObject);
        }


        virtualCamera = GetComponentInChildren<CinemachineCamera>();
        transposer = virtualCamera.GetComponent<CinemachinePositionComposer>();

    }

    private void Update()
    {
        UpdateCameraDistance();
    }

    private void UpdateCameraDistance()
    {
        if (canChangeCameraDistance == false)
            return;

        float currentDistnace = transposer.CameraDistance;

        if (Mathf.Abs(targetCameraDistance - currentDistnace) < .01f)
            return;
        
        transposer.CameraDistance =
            Mathf.Lerp(currentDistnace, targetCameraDistance, distanceChangeRate * Time.deltaTime);
    }

    public void ChangeCameraDistance(float distance) => targetCameraDistance = distance;

    public void ChangeCameraTarget(Transform target,float cameraDistance = 10,float newLookAheadTime = 0)
    {
        virtualCamera.Follow = target;
        transposer.Lookahead.Time = newLookAheadTime;
        ChangeCameraDistance(cameraDistance);
    }

}
