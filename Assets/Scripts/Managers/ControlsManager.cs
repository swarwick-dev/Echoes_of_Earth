using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsManager : MonoBehaviour
{
    public static ControlsManager instance;
    
    public event EventHandler OnGameDeviceChanged;

    public enum GameDevice { KeyboardMouse, Gamepad }
    public GameDevice currentDevice { get; private set; }

    public InputSystem_Actions controls { get; private set; }
    public Vector2 mouseInput;
    private GamepadCursor gamepadCursor;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            controls = new InputSystem_Actions();
            SwitchToUIControls();
        }
        else
        {
            Debug.LogWarning("You had more than one Controls Manager");
            Destroy(gameObject);
        }

        controls.Character.Aim.performed += context => mouseInput = GetMousePosition();
        controls.Character.Aim.canceled += context => mouseInput = Vector2.zero;

        //InputSystem.onActionChange += InputSystem_OnActionChange;
        if ( Gamepad.current != null) {
            ChangeActiveGameDevice(GameDevice.Gamepad);
        } else {
            ChangeActiveGameDevice(GameDevice.KeyboardMouse);
        }

        gamepadCursor =  GameManager.instance.transform.GetComponent<GamepadCursor>();
        if (gamepadCursor == null)
        {
            Debug.LogError("No GamepadCursor found!");
            return;
        }
    }

    public void SwitchToCharacterControls()
    {
        Cursor.visible = false;
        controls.Character.Enable();
        controls.Car.Disable();
        controls.UI.Disable();
        UI.instance.inGameUI.SwitchToCharcaterUI();
        GameManager.instance.player.SetControlsEnabledTo(true);
    }

    public void SwitchToUIControls()
    {
        Cursor.visible = true;
        controls.UI.Enable();
        controls.Car.Disable();
        controls.Character.Disable();
    }

    public void SwitchToCarControls()
    {
        Cursor.visible = false;
        controls.Car.Enable();
        controls.UI.Disable();
        controls.Character.Disable();
        UI.instance.inGameUI.SwitchToCarUI();
    }

    // on controller change
    // switch from mouse to gamepad and vice versa
    public Vector2 GetMousePosition() {
        if ( currentDevice == GameDevice.KeyboardMouse )
            return mouseInput;
        else {
            return gamepadCursor.GetWarpPosition();
        }
    }

    private void ChangeActiveGameDevice(GameDevice newDevice) {
        currentDevice = newDevice;
        Debug.Log($"Switched to {currentDevice} controls");

        Cursor.visible = currentDevice == GameDevice.KeyboardMouse;

        OnGameDeviceChanged?.Invoke(this, EventArgs.Empty);
    }

    private void InputSystem_OnActionChange(object arg1, InputActionChange inputActionChange) {
        if ( inputActionChange == InputActionChange.ActionPerformed && arg1 is InputAction) {
            InputAction inputAction = (InputAction)arg1;
            if ( inputAction.activeControl.device.displayName == "Mouse" || 
                 inputAction.activeControl.device.displayName == "Keyboard") {
                if ( currentDevice != GameDevice.KeyboardMouse)
                    ChangeActiveGameDevice(GameDevice.KeyboardMouse);
            } else { // Due to gamepads all having different display name
                if ( currentDevice != GameDevice.Gamepad)
                    ChangeActiveGameDevice(GameDevice.Gamepad);
            }
        }
    }

    private void OnDestroy() {
        InputSystem.onActionChange -= InputSystem_OnActionChange;
    }
}
