using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI instance;

    public UI_InGame inGameUI { get; private set; }
    public UI_WeaponSelection weaponSelectionUI { get; private set; }
    public UI_GameOver gameOverUI { get; private set; }
    public UI_Settings settingsUI { get; private set; }
    public GameObject victoryScreenUI;
    public GameObject pauseUI;


    [SerializeField] private GameObject[] UIElements;

    [Header("Fade image")]
    [SerializeField] private Image fadeImage;

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
        gameOverUI = GetComponentInChildren<UI_GameOver>(true);
        settingsUI = GetComponentInChildren<UI_Settings>(true);

    }

    private void Start()
    {
        AssignUIInputs();
        StartCoroutine(ChangeImageAlpha(0, 1.5f, null));

        //Remove this if statement before build, it's only for testing purposes
        /*if (GameManager.instance.quickStart)
        {
            StartTheGame();
        }*/

    }

    public void SwitchTo(GameObject uiToSwitchTOn)
    {
        foreach (GameObject go in UIElements)
        {
            go.SetActive(false);
        }

        uiToSwitchTOn.SetActive(true);

        if(uiToSwitchTOn == settingsUI.gameObject)
        {
            settingsUI.LoadSettings();
        }
    }

    public void StartTheGame()
    {
        if(weaponSelectionUI.IsNoWeapon())
            return;

        StartCoroutine(StartGameSequence());
    }

    public void RestartTheGame()
    {
        TimeManager.instance.ResumeTime();
        StartCoroutine(ChangeImageAlpha(1, 1, () =>
        {
            GameManager.instance.RestartScene();
        }));
    }

    public void PauseSwitch()
    {
        bool gamePaused = pauseUI.activeSelf;

        if (gamePaused)
        {
            SwitchTo(inGameUI.gameObject);
            ControlsManager.instance.SwitchToCharacterControls();
            TimeManager.instance.ResumeTime();
        }
        else
        {
            SwitchTo(pauseUI);
            ControlsManager.instance.SwitchToUIControls();
            TimeManager.instance.PauseTime();
        }
    }

    public void StartLevelGeneration() => LevelGenerator.instance.InitializedGeneration();

    public void QuitTheGame() => Application.Quit();

    public void ShowGameOverUI(string message = "GAME OVER!")
    {
        SwitchTo(gameOverUI.gameObject);
        gameOverUI.ShowGameOverMessage(message);
    }

    public void ShowVictoryScreen()
    {
        StartCoroutine(ChangeImageAlpha(1, 1.5f, SwitchToVictoryScreenUI));
    }

    private void SwitchToVictoryScreenUI()
    {
        SwitchTo(victoryScreenUI);
        
        Color color = fadeImage.color;
        color.a = 0;
        fadeImage.color = color;
    }

    private void AssignUIInputs()
    {
        PlayerControlls controls = Player.instance.controls;

        controls.UI.UIPause.performed += context => PauseSwitch();
    }

    private IEnumerator StartGameSequence()
    {
        //THIS SHOULD BE UNCOMMENTED BEFORE BUILD, IT'S ONLY FOR TESTING PURPOSES
        StartCoroutine(ChangeImageAlpha(1, 1, null));

        yield return new WaitForSeconds(1);
        yield return null;
        SwitchTo(inGameUI.gameObject);
        GameManager.instance.GameStart();
        /*StartCoroutine(ChangeImageAlpha(0, 0.1f, null));*/
        StartCoroutine(ChangeImageAlpha(0, 1, null));
    }

    private IEnumerator ChangeImageAlpha(float targetAlpha, float duration, System.Action onComplete)
    {
        float time = 0;
        Color currentColor = fadeImage.color;
        float startAlpha = currentColor.a;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);

        onComplete?.Invoke();
    }

    [ContextMenu("Assign Audio To Buttons")]
    public void AssignAudioListenesrsToButtons()
    {
        UI_Button[] buttons = FindObjectsOfType<UI_Button>(true);

        foreach (var button in buttons)
        {
            button.AssignAudioSource();
        }
    }
}
