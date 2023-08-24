using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Test))]
[CanEditMultipleObjects]
public class TestEditor : Editor
{
    SerializedProperty core;
    SerializedProperty hover;
    SerializedProperty hoverTransform;
    SerializedProperty coreTransform;

    private void OnEnable()
    {
        core = serializedObject.FindProperty("core");
        coreTransform = serializedObject.FindProperty("coreTransform");
        hover = serializedObject.FindProperty("hover");
        hoverTransform = serializedObject.FindProperty("hoverTransform");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(coreTransform);
        EditorGUILayout.PropertyField(hoverTransform);
        EditorGUILayout.PropertyField(core);
        EditorGUILayout.PropertyField(hover);
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField(Tools.GetInstance().GetZXAtan2(
            core.vector3Value, hover.vector3Value).ToString()
            );
    }

    /*
    public void OnSceneGUI()
    {
        var t = (target as Test);

        EditorGUI.BeginChangeCheck();
        Vector3 pos = Handles.PositionHandle(t.core, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Move point");
            t.core = pos;
            t.Update();
            Debug.Log("asdf");
        }
    }
    */
}