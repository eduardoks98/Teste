// using UnityEngine;
// using System.Collections;
// using UnityEditor;

// [CustomEditor(typeof(CharacterStatProperty))]
// public class CharacterStatEditor : Editor
// {
//     SerializedProperty inputName;

//     void OnEnable()
//     {
//         inputName = serializedObject.FindProperty("_inputStatName");
//     }

//     public override void OnInspectorGUI()
//     {
//         serializedObject.Update();

//         inputName.stringValue = EditorGUILayout.TextField("Input Name", inputName.stringValue);

//         serializedObject.ApplyModifiedProperties();
//     }
// }