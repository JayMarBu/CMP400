using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LightningGenerator2)), CanEditMultipleObjects]
public class LightningGenerator2Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Reset"))
        {
            LightningGenerator2 item = (LightningGenerator2)target;
            item.Reset();
        }

        if (GUILayout.Button("Generate"))
        {
            LightningGenerator2 item = (LightningGenerator2)target;
            item.Generate();
        }
    }
}
