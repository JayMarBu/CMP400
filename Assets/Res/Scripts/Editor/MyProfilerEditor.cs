using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MyProfiler)), CanEditMultipleObjects]
public class MyProfilerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MyProfiler item = (MyProfiler)target;

        if (!item.testInProgress)
        {
            base.OnInspectorGUI();

            if(Application.isPlaying)
            {
                if (GUILayout.Button("Run Test"))
                {
                    item.BeginTest();
                }
            }
        }
        else
        {
            GUILayout.Label("Test in progress");
        }


    }
}
