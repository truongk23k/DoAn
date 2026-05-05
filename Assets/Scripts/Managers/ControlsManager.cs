using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsManager : MonoBehaviour
{
    public static ControlsManager instance;
    public PlayerControlls controls { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        controls = Player.instance.controls;

        SwitchToCharacterControls();
    }

    public void SwitchToCharacterControls()
    {
        controls.UI.Disable();
        controls.Character.Enable();
        Player.instance.SetControlsEnabledTo(true);
    }

    public void SwitchToUIControls()
    {
        controls.UI.Enable();
        controls.Character.Disable();
        Player.instance.SetControlsEnabledTo(false);
    }
}
