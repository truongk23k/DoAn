using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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

    // Unity callback: chỉ chạy khi Add Component / nhấn Reset trong Inspector
    private void Reset()
    {
        if (tmpText == null) tmpText = GetComponentInChildren<TMP_Text>(true);
        if (uiText == null) uiText = GetComponentInChildren<Text>(true);
    }

    private void Awake()
    {
        // Không ghi đè field đã gán Inspector.
        // Chỉ auto-bind khi cả 2 đều chưa được gán.
        if (tmpText == null && uiText == null)
        {
            tmpText = GetComponentInChildren<TMP_Text>(true);
            uiText = GetComponentInChildren<Text>(true);
        }

        SaveOriginText();
    }

    private void OnEnable()
    {
        LocalizationManager.OnLocalizationChanged += UpdateLocalizedText;

        // Chỉ update khi LocalizationManager đã sẵn sàng,
        // còn không thì sẽ được cập nhật qua event hoặc Start().
        if (LocalizationManager.instance != null)
            UpdateLocalizedText();
    }

    private void Start()
    {
        if (!hasOriginSaved)
            SaveOriginText();

        UpdateLocalizedText();
        StartCoroutine(UpdateLocalizedTextCoroutine());
    }

    private IEnumerator UpdateLocalizedTextCoroutine()
    {
        yield return null;
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

    private void SaveOriginText()
    {
        if (hasOriginSaved) return;

        if (tmpText)
            originText = tmpText.text;
        else if (uiText)
            originText = uiText.text;

        hasOriginSaved = true;
    }

    private void UpdateLocalizedText()
    {
        // Nếu key rỗng => giữ nguyên text gốc (None)
        if (string.IsNullOrEmpty(key))
        {
            if (tmpText) tmpText.text = originText;
            if (uiText) uiText.text = originText;
            return;
        }

        // Nếu dynamic text thì dùng originText làm key
        string displayKey = isDynamicText ? originText : key;
        string value = LocalizationManager.GetLocalizedValue(displayKey);

        if (tmpText) tmpText.text = value;
        if (uiText) uiText.text = value;
    }
}