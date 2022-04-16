using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LightningGenerator)), CanEditMultipleObjects]
public class LightningGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
        {
            LightningGenerator item = (LightningGenerator)target;
            item.GenerareLightning();
        }

        if (GUILayout.Button("Clear"))
        {
            LightningGenerator item = (LightningGenerator)target;
            item.Clear();
        }
    }
}
