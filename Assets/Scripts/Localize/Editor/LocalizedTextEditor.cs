using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(LocalizedText))]
public class LocalizedTextEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LocalizedText localizedText = (LocalizedText)target;

        DrawDefaultInspector();

        LocalizationManager manager = FindObjectOfType<LocalizationManager>();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Localization Dropdown", EditorStyles.boldLabel);

        if (manager == null)
        {
            EditorGUILayout.HelpBox("Không tìm thấy LocalizationManager trong Scene.", MessageType.Warning);
            return;
        }

        if (manager.AllLocalizations == null || manager.AllLocalizations.Count == 0)
        {
            EditorGUILayout.HelpBox("LocalizationManager chưa có dữ liệu allLocalizations.", MessageType.Warning);
            return;
        }

        List<string> keys = manager.AllLocalizations
            .Where(x => !string.IsNullOrEmpty(x.key))
            .Select(x => x.key)
            .Distinct()
            .ToList();

        // Thêm option None ở đầu
        keys.Insert(0, "-- None (Keep Original Text) --");

        int currentIndex = 0;

        if (!string.IsNullOrEmpty(localizedText.key))
        {
            int foundIndex = keys.IndexOf(localizedText.key);
            currentIndex = foundIndex >= 0 ? foundIndex : 0;
        }

        int newIndex = EditorGUILayout.Popup("Key", currentIndex, keys.ToArray());

        if (newIndex == 0)
        {
            // None
            if (!string.IsNullOrEmpty(localizedText.key))
            {
                Undo.RecordObject(localizedText, "Set Localization Key None");
                localizedText.key = "";
                EditorUtility.SetDirty(localizedText);
            }
        }
        else
        {
            string newKey = keys[newIndex];

            if (newKey != localizedText.key)
            {
                Undo.RecordObject(localizedText, "Change Localization Key");
                localizedText.key = newKey;
                EditorUtility.SetDirty(localizedText);
            }
        }
    }
}