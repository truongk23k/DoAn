using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI instance;

    public UI_InGame inGameUI { get; private set; }
    public UI_WeaponSelection weaponSelectionUI { get; private set; }
    public GameObject pauseUI;


    [SerializeField] private GameObject[] UIElements;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        inGameUI = GetComponentInChildren<UI_InGame>(true);
        weaponSelectionUI = GetComponentInChildren<UI_WeaponSelection>(true);

    }

    private void Start()
    {
        AssignUIInputs();
    }

    public void SwitchTo(GameObject uiToSwitchTOn)
    {
        foreach (GameObject go in UIElements)
        {
            go.SetActive(false);
        }

        uiToSwitchTOn.SetActive(true);
    }

    public void StartTheGame()
    {
        SwitchTo(inGameUI.gameObject);
        GameManager.instance.GameStart();
    }

    public void RestartTheGame() => GameManager.instance.RestartScene();

    public void PauseSwitch()
    {
        bool gamePaused = pauseUI.activeSelf;

        if (gamePaused)
        {
            SwitchTo(inGameUI.gameObject);
            ControlsManager.instance.SwitchToCharacterControls();
            Time.timeScale = 1f;
        }
        else
        {
            SwitchTo(pauseUI);
            ControlsManager.instance.SwitchToUIControls();
            Time.timeScale = 0f;
        }
    }

    public void QuitTheGame() => Application.Quit();

    private void AssignUIInputs()
    {
        PlayerControlls controls = Player.instance.controls;

        controls.UI.UIPause.performed += context => PauseSwitch();
    }
}
