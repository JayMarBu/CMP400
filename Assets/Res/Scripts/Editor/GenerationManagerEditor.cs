using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerationManager)), CanEditMultipleObjects]
public class GenerationManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(15);

        if (GUILayout.Button("Reset"))
        {
            GenerationManager item = (GenerationManager)target;
            item.Clear();
        }

        if (GUILayout.Button("Generate All"))
        {
            GenerationManager item = (GenerationManager)target;
            item.GenerateAll();
        }

        if (GUILayout.Button("Generate Method 1"))
        {
            GenerationManager item = (GenerationManager)target;
            item.GenerateJitterAndFork();
        }

        if (GUILayout.Button("Generate Method 2"))
        {
            GenerationManager item = (GenerationManager)target;
            item.GenerateBaileyEtAl();
        }
    }
}
