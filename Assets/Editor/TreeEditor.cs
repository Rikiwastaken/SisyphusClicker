#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TalentTreeScript))]
public class TalentTreeScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TalentTreeScript script = (TalentTreeScript)target;

        if (GUILayout.Button("Apply Node Changes & Save Scene"))
        {
            script.ApplyNodeChangesAndSave();
        }
    }
}
#endif