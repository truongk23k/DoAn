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

        controls = new PlayerControlls();
    }

    private void Start()
    {

        SwitchToCharacterControls();
    }

    public void SwitchToCharacterControls()
    {
        controls.Character.Enable();

        controls.UI.Disable();
        controls.Car.Disable();
        Player.instance.SetControlsEnabledTo(true);
        UI.instance.inGameUI.SwitchToCharacterUI();
    }

    public void SwitchToUIControls()
    {
        controls.UI.Enable();

        controls.Car.Disable();
        controls.Character.Disable();
        Player.instance.SetControlsEnabledTo(false);
    }

    public void SwitchToCarControls()
    {
        controls.Car.Enable();

        controls.UI.Disable();
        controls.Character.Disable();
        Player.instance.SetControlsEnabledTo(false);
        UI.instance.inGameUI.SwitchToCarUI();
    }
}
