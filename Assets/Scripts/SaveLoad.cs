using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SaveLoad : MonoBehaviour
{
    public static SaveLoad instance;

    public AudioMixer audioMixer;
    [SerializeField] private float sliderMultiplier = 25;
    public string sfxParametr = "sfx";     // ← Đặt default value
    public string bgmParametr = "bgm";     // ← Đặt default value
    public string friendlyFireParametr = "FriendlyFire"; // ← Đặt default value
    public string indexLanguageParametr = "LanguageIndex"; // ← Đặt default value

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(instance.gameObject);
    }

    private void Start()
    {
        // Load language
        int languageIndex = PlayerPrefs.GetInt(indexLanguageParametr, 0);
        UI.instance.settingsUI.languageDropdown.value = languageIndex;
        LocalizationManager.ChangeLanguage((Language)languageIndex);

        // Load SFX
        float sfxValue = PlayerPrefs.GetFloat(sfxParametr, 0.7f);
        float newSFXValue = Mathf.Log10(sfxValue) * sliderMultiplier;
        audioMixer.SetFloat(sfxParametr, newSFXValue);

        // Load BGM
        float bgmValue = PlayerPrefs.GetFloat(bgmParametr, 0.7f);
        float newBGMValue = Mathf.Log10(bgmValue) * sliderMultiplier;
        audioMixer.SetFloat(bgmParametr, newBGMValue);

        // Load Friendly Fire
        int friendlyFireInt = PlayerPrefs.GetInt(friendlyFireParametr, 0);
        bool friendlyFire = friendlyFireInt == 1;
        GameManager.instance.friendlyFire = friendlyFire;
    }

    /*private void OnDisable()
    {
        SaveSFXBGM();

        SaveFriendlyFire();

        SaveLanguage();
    }*/

    public void SaveSFXBGM()
    {
        PlayerPrefs.SetFloat(sfxParametr, UI.instance.settingsUI.sfxSlider.value);
        PlayerPrefs.SetFloat(bgmParametr, UI.instance.settingsUI.bgmSlider.value);
    }

    public void SaveFriendlyFire()
    {
        bool friendlyFire = GameManager.instance.friendlyFire;
        int friendlyFireInt = friendlyFire ? 1 : 0;
        PlayerPrefs.SetInt(friendlyFireParametr, friendlyFireInt);
    }

    public void SaveLanguage()
    {
        int languageIndex = UI.instance.settingsUI.languageDropdown.value;
        PlayerPrefs.SetInt(indexLanguageParametr, languageIndex);
    }
}
