using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;

public class VirtualMouseController : MonoBehaviour
{
    private VirtualMouseInput virtualMouseInput;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private RectTransform virtualMouse;
    private void Awake()
    {
        virtualMouseInput = GetComponent<VirtualMouseInput>();
    }

    private void Start() {
        ControlsManager.instance.OnGameDeviceChanged += VM_OnGameDeviceChanged;
        ResetMouseToCenter();
        UpdateVisibility();
    }

    private void Update() {
        transform.localScale = Vector3.one * (1f / canvasRectTransform.localScale.x);
        transform.SetAsLastSibling();
    }
    
    private void LateUpdate() {
        Vector2 vmPos = virtualMouseInput.virtualMouse.position.value;
        vmPos.x = Mathf.Clamp(vmPos.x, 0, Screen.width);
        vmPos.y = Mathf.Clamp(vmPos.y, 0, Screen.height);
        InputState.Change(virtualMouseInput.virtualMouse.position, vmPos);
    }

    private void VM_OnGameDeviceChanged(object sender, System.EventArgs e) {
        UpdateVisibility();
    }

    private void UpdateVisibility() {
        if (ControlsManager.instance.currentDevice == ControlsManager.GameDevice.KeyboardMouse) {
            gameObject.SetActive(false);
            Debug.Log("Virtual Mouse disabled");
        } else {
            ResetMouseToCenter();
            gameObject.SetActive(true);
            Debug.Log("Virtual Mouse enabled");
        }
    }

    private void ResetMouseToCenter() {
        virtualMouse.anchoredPosition = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    private void OnDestroy() {
        ControlsManager.instance.OnGameDeviceChanged -= VM_OnGameDeviceChanged;
    }
}
