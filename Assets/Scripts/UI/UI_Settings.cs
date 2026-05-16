using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Thin display layer for the Settings panel.
/// Reads from <see cref="SaveLoad"/> on open, writes to <see cref="SaveLoad"/> on change.
/// Never touches PlayerPrefs or the AudioMixer directly.
/// </summary>
public class UI_Settings : MonoBehaviour
{
    [Header("SFX Settings")]
    public Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxSliderText;

    [Header("BGM Settings")]
    public Slider bgmSlider;
    [SerializeField] private TextMeshProUGUI bgmSliderText;

    [Header("Toggle")]
    public Toggle friendlyFireToggle;

    [Header("Dropdown")]
    public TMP_Dropdown languageDropdown;

    /// <summary>Hooked to <see cref="Slider.onValueChanged"/> (dynamic float).</summary>
    public void SFXSliderValue(float value)
    {
        sfxSliderText.text = ToPercent(value);
        if (SaveLoad.instance != null)
            SaveLoad.instance.SaveSfx(value);
    }

    /// <summary>Hooked to <see cref="Slider.onValueChanged"/> (dynamic float).</summary>
    public void BGMSliderValue(float value)
    {
        bgmSliderText.text = ToPercent(value);
        if (SaveLoad.instance != null)
            SaveLoad.instance.SaveBgm(value);
    }

    /// <summary>Hooked to <see cref="Toggle.onValueChanged"/> as a parameterless call.</summary>
    public void OnFriendlyFireToggle()
    {
        if (SaveLoad.instance != null)
            SaveLoad.instance.SaveFriendlyFire(friendlyFireToggle.isOn);
    }

    /// <summary>Hooked to <see cref="TMP_Dropdown.onValueChanged"/> as a parameterless call.</summary>
    public void OnLanguageDropdown()
    {
        if (SaveLoad.instance != null)
            SaveLoad.instance.SaveLanguage(languageDropdown.value);
    }

    /// <summary>
    /// Pulls the current persisted values from <see cref="SaveLoad"/> and pushes them into the UI.
    /// Uses *WithoutNotify variants to avoid re-firing OnValueChanged → re-saving.
    /// </summary>
    public void LoadSettings()
    {
        if (SaveLoad.instance == null) return;

        float sfx = SaveLoad.instance.GetSfx();
        float bgm = SaveLoad.instance.GetBgm();
        bool friendlyFire = SaveLoad.instance.GetFriendlyFire();
        int languageIndex = SaveLoad.instance.GetLanguageIndex();

        sfxSlider.SetValueWithoutNotify(sfx);
        bgmSlider.SetValueWithoutNotify(bgm);
        sfxSliderText.text = ToPercent(sfx);
        bgmSliderText.text = ToPercent(bgm);

        friendlyFireToggle.SetIsOnWithoutNotify(friendlyFire);
        languageDropdown.SetValueWithoutNotify(languageIndex);
        languageDropdown.RefreshShownValue();
    }

    private static string ToPercent(float linearValue) => Mathf.RoundToInt(linearValue * 100) + "%";
}
