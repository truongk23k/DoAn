using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float sliderMultiplier = 25;

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

    public void SFXSliderValue(float value)
    {
        sfxSliderText.text = Mathf.RoundToInt(value * 100) + "%";
        float newValue = Mathf.Log10(value) * sliderMultiplier;
        audioMixer.SetFloat(SaveLoad.instance.sfxParametr, newValue);
        SaveLoad.instance.SaveSFXBGM();
    }

    public void BGMSliderValue(float value)
    {
        bgmSliderText.text = Mathf.RoundToInt(value * 100) + "%";
        float newValue = Mathf.Log10(value) * sliderMultiplier;
        audioMixer.SetFloat(SaveLoad.instance.bgmParametr, newValue);
        SaveLoad.instance.SaveSFXBGM();
    }

    public void OnFriendlyFireToggle()
    {
        bool friendlyFire = GameManager.instance.friendlyFire;
        GameManager.instance.friendlyFire = !friendlyFire;
        SaveLoad.instance.SaveFriendlyFire();
    }

    public void OnLanguageDropdown()
    {
        int languageIndex = languageDropdown.value;
        LocalizationManager.ChangeLanguage((Language)languageIndex);
        SaveLoad.instance.SaveLanguage();
    }

    public void LoadSettings()
    {
        sfxSlider.value = PlayerPrefs.GetFloat(SaveLoad.instance.sfxParametr, .7f);
        bgmSlider.value = PlayerPrefs.GetFloat(SaveLoad.instance.bgmParametr, .7f);

        int friendlyFireInt = PlayerPrefs.GetInt(SaveLoad.instance.friendlyFireParametr, 0);
        bool newFriendlyFire = false;

        if (friendlyFireInt == 1)
            newFriendlyFire = true;

        friendlyFireToggle.isOn = newFriendlyFire;
    }

}
