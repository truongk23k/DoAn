using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Cover))]
public class CoverEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Cover cover = (Cover)target;

  GUILayout.Space(10);

        if (GUILayout.Button("Create Cover Points", GUILayout.Height(30)))
        {
cover.SendMessage("CreateCoverPoints");
     }

        if (GUILayout.Button("Clear Cover Points", GUILayout.Height(30)))
        {
        cover.SendMessage("ClearCoverPoints");
  }
    }
}
