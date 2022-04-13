using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BaileyetalGen)), CanEditMultipleObjects]
public class BaileyetalEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Reset"))
        {
            BaileyetalGen item = (BaileyetalGen)target;
            item.Clear();
        }

        if (GUILayout.Button("Generate"))
        {
            BaileyetalGen item = (BaileyetalGen)target;
            item.Generate();
        }
    }
}

[CustomPropertyDrawer(typeof(BaileyetalGen.GasProperties))]
public class GasPropertiesPropDrawer : PropertyDrawer
{
    float lineHeight { get { return EditorGUIUtility.singleLineHeight; } }
    float padding { get { return EditorGUIUtility.standardVerticalSpacing; } }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (property.isExpanded)?((lineHeight + padding) * 4):(lineHeight + padding);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float currentY = position.position.y;

        var dropdown = new Rect(position.position.x, currentY, position.size.x, lineHeight);
        property.isExpanded = EditorGUI.Foldout(dropdown, property.isExpanded, new GUIContent("Gas Properties"), true);
        if (!property.isExpanded)
        {
            EditorGUI.EndProperty();
            return;
        }

        EditorGUI.indentLevel++;
        currentY += lineHeight + padding;

        var standardRect = new Rect(position.x, currentY, position.width, lineHeight);

        EditorGUI.PropertyField(standardRect, property.FindPropertyRelative("L_mean"));

        currentY += lineHeight + padding;
        standardRect.y = currentY;

        EditorGUI.PropertyField(standardRect, property.FindPropertyRelative("L_std"));

        currentY += lineHeight + padding;

        float checkboxPos = position.width / 2;

        var airRect = new Rect(position.x, currentY, position.width/2, lineHeight);
        var n2Rect = new Rect(position.x + checkboxPos, currentY, position.width / 2, lineHeight);

        if (GUI.Button(airRect, new GUIContent("Air")))
        {
            property.FindPropertyRelative("L_mean").floatValue  = BaileyetalGen.GasProperties.Air.L_mean;
            property.FindPropertyRelative("L_std").floatValue   = BaileyetalGen.GasProperties.Air.L_std;

            property.FindPropertyRelative("A_mean").floatValue  = BaileyetalGen.GasProperties.Air.A_mean;
            property.FindPropertyRelative("A_std").floatValue   = BaileyetalGen.GasProperties.Air.A_std;
            property.serializedObject.ApplyModifiedProperties();
        }

        if (GUI.Button(n2Rect, new GUIContent("N2")))
        {
            property.FindPropertyRelative("L_mean").floatValue  = BaileyetalGen.GasProperties.N2.L_mean;
            property.FindPropertyRelative("L_std").floatValue   = BaileyetalGen.GasProperties.N2.L_std;

            property.FindPropertyRelative("A_mean").floatValue  = BaileyetalGen.GasProperties.N2.A_mean;
            property.FindPropertyRelative("A_std").floatValue   = BaileyetalGen.GasProperties.N2.A_std;
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }
}
