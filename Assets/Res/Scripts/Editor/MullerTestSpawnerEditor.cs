using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MullerTestSpawner)), CanEditMultipleObjects]
public class MullerTestSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Generate"))
        {
            MullerTestSpawner item = (MullerTestSpawner)target;
            item.Spawn();
        }

        if (GUILayout.Button("Clear"))
        {
            MullerTestSpawner item = (MullerTestSpawner)target;
            item.Clear();
        }
    }
}
