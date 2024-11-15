using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PrometeoCarController))]
public class PrometeoEditor : Editor
{
    private SerializedProperty jumpDistance;
    private SerializedProperty jumpCooldown;

    void OnEnable()
    {
        jumpDistance = serializedObject.FindProperty("jumpDistance");
        jumpCooldown = serializedObject.FindProperty("jumpCooldown");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Side Jump Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(jumpDistance, new GUIContent("Jump Distance"));
        EditorGUILayout.PropertyField(jumpCooldown, new GUIContent("Jump Cooldown"));

        serializedObject.ApplyModifiedProperties();
    }
}
