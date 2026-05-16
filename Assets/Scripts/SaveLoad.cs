using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Single source of truth for persistent settings.
/// Owns the AudioMixer + PlayerPrefs writes. UI components read/write through this class.
/// </summary>
public class SaveLoad : MonoBehaviour
{
    public static SaveLoad instance;

    // ---- PlayerPrefs keys (kept as fields so existing Inspector values keep working) ----
    public string sfxParametr = "sfx";
    public string bgmParametr = "bgm";
    public string friendlyFireParametr = "FriendlyFire";
    public string indexLanguageParametr = "LanguageIndex";

    // ---- Defaults ----
    private const float DefaultSfx = 0.7f;
    private const float DefaultBgm = 0.7f;
    private const int DefaultFriendlyFire = 0;
    private const int DefaultLanguage = 0;

    // log10(0) = -Infinity, so clamp slider input before converting to dB.
    private const float MinLinearVolume = 0.0001f;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float sliderMultiplier = 25f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        ApplySavedSettings();
    }

    /// <summary>Push every persisted setting into the runtime systems (mixer, GameManager, LocalizationManager).</summary>
    public void ApplySavedSettings()
    {
        LocalizationManager.ChangeLanguage((Language)GetLanguageIndex());

        ApplyVolumeToMixer(sfxParametr, GetSfx());
        ApplyVolumeToMixer(bgmParametr, GetBgm());

        if (GameManager.instance != null)
            GameManager.instance.friendlyFire = GetFriendlyFire();
    }

    // -------- Getters (UI uses these instead of touching PlayerPrefs directly) --------
    public float GetSfx() => PlayerPrefs.GetFloat(sfxParametr, DefaultSfx);
    public float GetBgm() => PlayerPrefs.GetFloat(bgmParametr, DefaultBgm);
    public bool GetFriendlyFire() => PlayerPrefs.GetInt(friendlyFireParametr, DefaultFriendlyFire) == 1;
    public int GetLanguageIndex() => PlayerPrefs.GetInt(indexLanguageParametr, DefaultLanguage);

    // -------- Setters (UI calls these on value change) --------
    public void SaveSfx(float linearValue)
    {
        ApplyVolumeToMixer(sfxParametr, linearValue);
        PlayerPrefs.SetFloat(sfxParametr, linearValue);
        PlayerPrefs.Save();
    }

    public void SaveBgm(float linearValue)
    {
        ApplyVolumeToMixer(bgmParametr, linearValue);
        PlayerPrefs.SetFloat(bgmParametr, linearValue);
        PlayerPrefs.Save();
    }

    public void SaveFriendlyFire(bool value)
    {
        if (GameManager.instance != null)
            GameManager.instance.friendlyFire = value;

        PlayerPrefs.SetInt(friendlyFireParametr, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SaveLanguage(int languageIndex)
    {
        LocalizationManager.ChangeLanguage((Language)languageIndex);
        PlayerPrefs.SetInt(indexLanguageParametr, languageIndex);
        PlayerPrefs.Save();
    }

    // -------- Legacy API kept so any external callers still compile --------
    public void SaveSFXBGM()
    {
        if (UI.instance == null || UI.instance.settingsUI == null) return;
        SaveSfx(UI.instance.settingsUI.sfxSlider.value);
        SaveBgm(UI.instance.settingsUI.bgmSlider.value);
    }

    public void SaveFriendlyFire()
    {
        SaveFriendlyFire(GameManager.instance != null && GameManager.instance.friendlyFire);
    }

    public void SaveLanguage()
    {
        if (UI.instance == null || UI.instance.settingsUI == null) return;
        SaveLanguage(UI.instance.settingsUI.languageDropdown.value);
    }

    // -------- Internals --------
    private void ApplyVolumeToMixer(string parameter, float linearValue)
    {
        if (audioMixer == null) return;

        float safe = Mathf.Max(linearValue, MinLinearVolume);
        audioMixer.SetFloat(parameter, Mathf.Log10(safe) * sliderMultiplier);
    }
}
