using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SaveLoad : MonoBehaviour
{
    public AudioMixer audioMixer;
    [SerializeField] private float sliderMultiplier = 25;
    public string sfxParametr = "sfx";     // ← Đặt default value
    public string bgmParametr = "bgm";     // ← Đặt default value

    private void Start()
    {
        Debug.Log("SaveLoad.Start() - Loading audio settings");

        // Load SFX
        float sfxValue = PlayerPrefs.GetFloat(sfxParametr, 0.7f);
        float newSFXValue = Mathf.Log10(sfxValue) * sliderMultiplier;
        audioMixer.SetFloat(sfxParametr, newSFXValue);
        Debug.Log($"Set SFX: {sfxValue} → {newSFXValue}dB");

        // Load BGM
        float bgmValue = PlayerPrefs.GetFloat(bgmParametr, 0.7f);
        float newBGMValue = Mathf.Log10(bgmValue) * sliderMultiplier;
        audioMixer.SetFloat(bgmParametr, newBGMValue);
        Debug.Log($"Set BGM: {bgmValue} → {newBGMValue}dB");
    }
}
