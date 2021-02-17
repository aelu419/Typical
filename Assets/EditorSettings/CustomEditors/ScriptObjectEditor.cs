using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScriptObjectScriptable))]
public class ScriptObjectEditor : Editor
{
    SerializedProperty TA;
    SerializedProperty TW;
    SerializedProperty ST;

    public void OnEnable()
    {
        ST = serializedObject.FindProperty("source");
        TA = serializedObject.FindProperty("text_asset");
        TW = serializedObject.FindProperty("text_writer");
    }
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        serializedObject.Update();

        ScriptObjectScriptable sos = (ScriptObjectScriptable)target;

        sos.name_ = EditorGUILayout.TextField("Script Name", sos.name_);
        EditorGUILayout.PropertyField(ST);
        sos.source = (ScriptTextSource)ST.enumValueIndex;

        //text is from computer generated script
        if (sos.source == ScriptTextSource.SCRIPT)
        {
            sos.text_asset = null;
            EditorGUILayout.PropertyField(TW);
        }
        //text is from manually written script (text asset)
        else
        {
            sos.text_writer = null;
            EditorGUILayout.PropertyField(TA);
        }
        serializedObject.ApplyModifiedProperties();
        
        if (GUI.changed)
        {
            EditorUtility.SetDirty(sos);
        }
    }
}
