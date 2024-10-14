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
        controls.Character.Enable();
        controls.Car.Disable();
        controls.UI.Disable();
        UI.instance.inGameUI.SwitchToCharcaterUI();
    }

    public void SwitchToUIControls()
    {
        controls.UI.Enable();
        controls.Car.Disable();
        controls.Character.Disable();
    }

    public void SwitchToCarControls()
    {
        controls.Car.Enable();
        controls.UI.Disable();
        controls.Character.Disable();
        UI.instance.inGameUI.SwitchToCarUI();
    }


}
