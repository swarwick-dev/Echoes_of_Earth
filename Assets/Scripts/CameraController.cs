using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    public float distance = 6.0f;
    public float x = 0.0f;
    public float y = 0.0f;
    public float z = 0.0f;
    public float fov = 60.0f;

    public Slider distanceSlider;
    public Slider xSlider;
    public Slider ySlider;
    public Slider zSlider;
    public Slider fovSlider;

    CinemachineFramingTransposer framingTransposer;
    CinemachineCamera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = GetComponent<CinemachineCamera>();
        framingTransposer = mainCamera.GetComponent<CinemachineFramingTransposer>();        
    }

    // Update is called once per frame
    void Update()
    {
        framingTransposer.m_CameraDistance = distance;
        framingTransposer.m_TrackedObjectOffset.x = x;
        framingTransposer.m_TrackedObjectOffset.y = y;
        framingTransposer.m_TrackedObjectOffset.z = z;       
    }
}
