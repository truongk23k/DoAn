using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalizedText : MonoBehaviour
{
    [Header("Key (leave empty = keep original text)")]
    public string key = "";

    [Header("Component")]
    [SerializeField] private TMP_Text tmpText;
    [SerializeField] private Text uiText;

    [Header("Options")]
    [SerializeField] private bool isDynamicText = false;

    private string originText = "";
    private bool hasOriginSaved = false;

    private void Awake()
    {
        Reset();

        SaveOriginText();
    }

    private void OnEnable()
    {
        LocalizationManager.OnLocalizationChanged += UpdateLocalizedText;

        // Update when enabled
        UpdateLocalizedText();
    }

    private void Start()
    {
        // đảm bảo originText có giá trị
        if (!hasOriginSaved)
            SaveOriginText();

        UpdateLocalizedText();
    }

    private void OnDisable()
    {
        LocalizationManager.OnLocalizationChanged -= UpdateLocalizedText;
    }

    private void OnDestroy()
    {
        LocalizationManager.OnLocalizationChanged -= UpdateLocalizedText;
    }

    private void Reset()
    {
        tmpText = GetComponent<TMP_Text>();
        uiText = GetComponent<Text>();
    }

    private void SaveOriginText()
    {
        if (hasOriginSaved) return;

        if (tmpText)
            originText = tmpText.text;

        if (uiText)
            originText = uiText.text;

        hasOriginSaved = true;
    }

    private void UpdateLocalizedText()
    {
        // Nếu key rỗng => giữ nguyên text gốc (None)
        if (string.IsNullOrEmpty(key))
        {
            if (tmpText)
                tmpText.text = originText;

            if (uiText)
                uiText.text = originText;

            return;
        }

        // Nếu dynamic text thì dùng originText làm key
        string displayKey = isDynamicText ? originText : key;

        if (tmpText)
            tmpText.text = LocalizationManager.GetLocalizedValue(displayKey);

        if (uiText)
            uiText.text = LocalizationManager.GetLocalizedValue(displayKey);
    }
}