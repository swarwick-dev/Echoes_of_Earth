using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsManager 
{
    public static ControlsManager instance;
    public InputSystem_Actions controls { get; private set; }

    public ControlsManager() {
        instance = this;
        controls = new InputSystem_Actions();
        SwitchToUIControls();
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


}
