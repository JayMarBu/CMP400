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

[CustomPropertyDrawer(typeof(GasProperties))]
public class GasPropertiesPropDrawer : PropertyDrawer
{
    float lineHeight { get { return EditorGUIUtility.singleLineHeight; } }
    float padding { get { return EditorGUIUtility.standardVerticalSpacing; } }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float trueLineHeight = lineHeight + padding;
        float baseHeight = trueLineHeight * 4;

        if (property.FindPropertyRelative("L").isExpanded)
            baseHeight += trueLineHeight * 2f;

        if (property.FindPropertyRelative("A").isExpanded)
            baseHeight += trueLineHeight * 2f;

        return (property.isExpanded)?baseHeight:trueLineHeight;
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

        SerializedProperty prop = property.FindPropertyRelative("L");
        EditorGUI.PropertyField(standardRect, prop, true);

        currentY += (prop.isExpanded)?(lineHeight + padding) * 3: (lineHeight + padding);
        standardRect.y = currentY;

        prop = property.FindPropertyRelative("A");
        EditorGUI.PropertyField(standardRect, prop, true);

        currentY += (prop.isExpanded) ? (lineHeight + padding) * 3 : (lineHeight + padding);

        float checkboxPos = position.width / 2;

        var airRect = new Rect(position.x, currentY, position.width/2, lineHeight);
        var n2Rect = new Rect(position.x + checkboxPos, currentY, position.width / 2, lineHeight);

        if (GUI.Button(airRect, new GUIContent("Air")))
        {
            property.FindPropertyRelative("L.mean").floatValue  = GasProperties.Air.L.mean;
            property.FindPropertyRelative("L.std").floatValue   = GasProperties.Air.L.std;

            property.FindPropertyRelative("A.mean").floatValue  = GasProperties.Air.A.mean;
            property.FindPropertyRelative("A.std").floatValue   = GasProperties.Air.A.std;
            property.serializedObject.ApplyModifiedProperties();
        }

        if (GUI.Button(n2Rect, new GUIContent("N2")))
        {
            property.FindPropertyRelative("L.mean").floatValue  = GasProperties.N2.L.mean;
            property.FindPropertyRelative("L.std").floatValue   = GasProperties.N2.L.std;

            property.FindPropertyRelative("A.mean").floatValue  = GasProperties.N2.A.mean;
            property.FindPropertyRelative("A.std").floatValue   = GasProperties.N2.A.std;
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }
}
