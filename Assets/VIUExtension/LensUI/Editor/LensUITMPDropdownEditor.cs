using HTC.Triton.LensUI;
using UnityEditor;
using UnityEditor.UI;
using UnityEditorInternal;

[CustomEditor(typeof(LensUITMPDropdown), true)]
[CanEditMultipleObjects]
public class LensUITMPDropdownEditor : LensUISelectableEditor
{
    SerializedProperty m_Template;
    SerializedProperty m_CaptionText;
    //SerializedProperty m_CaptionImage;
    SerializedProperty m_Placeholder;
    SerializedProperty m_ItemText;
    //SerializedProperty m_ItemImage;
    SerializedProperty m_OnSelectionChanged;
    SerializedProperty m_Value;
    SerializedProperty m_AlphaFadeSpeed;
    SerializedProperty m_Options;
    SerializedProperty m_OnClicked;

    //private ReorderableList list;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_Template = serializedObject.FindProperty("m_Template");
        m_CaptionText = serializedObject.FindProperty("m_CaptionText");
        //m_CaptionImage = serializedObject.FindProperty("m_CaptionImage");
        m_Placeholder = serializedObject.FindProperty("m_Placeholder");
        m_ItemText = serializedObject.FindProperty("m_ItemText");
        //m_ItemImage = serializedObject.FindProperty("m_ItemImage");
        m_OnSelectionChanged = serializedObject.FindProperty("m_OnValueChanged");
        m_Value = serializedObject.FindProperty("m_Value");
        m_AlphaFadeSpeed = serializedObject.FindProperty("m_AlphaFadeSpeed");
        m_Options = serializedObject.FindProperty("m_Options");
        m_OnClicked = serializedObject.FindProperty("m_OnClick");

        //list = new ReorderableList(serializedObject, m_Options, true, true, true, true);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        serializedObject.Update();
        EditorGUILayout.PropertyField(m_Template);
        EditorGUILayout.PropertyField(m_CaptionText);
        //EditorGUILayout.PropertyField(m_CaptionImage);
        EditorGUILayout.PropertyField(m_Placeholder);
        EditorGUILayout.PropertyField(m_ItemText);
        //EditorGUILayout.PropertyField(m_ItemImage);
        EditorGUILayout.PropertyField(m_Value);
        EditorGUILayout.PropertyField(m_AlphaFadeSpeed);
        EditorGUILayout.PropertyField(m_Options);
        EditorGUILayout.PropertyField(m_OnSelectionChanged);
        EditorGUILayout.PropertyField(m_OnClicked);
        //list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
