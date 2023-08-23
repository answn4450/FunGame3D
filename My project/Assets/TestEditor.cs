using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Test))]
[CanEditMultipleObjects]
public class TestEditor : Editor
{
    SerializedProperty lookAtPoint;
    private void OnEnable()
    {
        lookAtPoint = serializedObject.FindProperty("core");
        
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(lookAtPoint);
        serializedObject.ApplyModifiedProperties();

        serializedObject.ApplyModifiedProperties();
        if (lookAtPoint. vector3Value.y > (target as Test).transform.position.y)
        {
            EditorGUILayout.LabelField("(Above this object)");
        }
        if (lookAtPoint.vector3Value.y < (target as Test).transform.position.y)
        {
            EditorGUILayout.LabelField("(Below this object)");
        }
    }

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
}